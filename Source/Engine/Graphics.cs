using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.WindowsMobile.DirectX;
using Microsoft.WindowsMobile.DirectX.Direct3D;

namespace WMGame3D
{
    public struct Transform
    {
        public Matrix Matrix;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public Transform(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            Scale = scale;

            Matrix = Matrix.Scaling(scale.X, scale.Y, scale.Z) *
                Matrix.RotationY(rot.Y * Mathf.DegToRad) *
                Matrix.RotationZ(rot.Z * Mathf.DegToRad) *
                Matrix.RotationX(rot.X * Mathf.DegToRad) *
                Matrix.Translation(pos.X, pos.Y, pos.Z);
        }
    }

    public sealed class Texture2D
    {
        internal Texture handle;
        public int Width;
        public int Height;

        public Texture2D(Texture handle)
        {
            if (handle == null)
                throw new ArgumentException("Native texture handle can't be null");

            this.handle = handle;
        }

        public static Texture2D FromFile(string fileName)
        {
            Stream asset = Engine.Current.OpenAsset(fileName);

            if (asset != null)
                return new Texture2D(TextureLoader.FromStream(Engine.Current.Graphics.device, asset));

            return null;
        }
    }

    public struct Material
    {
        public bool UseColor;
        public Vector4 Color;
        public Texture2D Texture;

        public bool ZWrite;
        //public bool AlphaBlend;
        public float AlphaValue;
    }

    public struct Vertex
    {
        public Vector3 Position;
        public Vector2 UV;

        public Vertex(Vector3 p, Vector2 uv)
        {
            Position = p;
            UV = uv;
        }
    }

    public sealed class Camera
    {
        

        public float FOV;
        public float Aspect;
        public float Near;
        public float Far;

        public Vector3 Position;
        public Vector3 Rotation;

        public Camera()
        {
            FOV = 75.0f;
            Aspect = (float)Engine.Current.Window.ViewportWidth / Engine.Current.Window.ViewportHeight;
            Near = 0.1f;
            Far = 150.0f;
        }

        public Matrix GetProjection()
        {
            return Matrix.PerspectiveFovLH(FOV * Mathf.DegToRad, Aspect, Near, Far);
        }

        public Matrix GetView()
        {
            return Matrix.Translation(-Position.X, -Position.Y, -Position.Z) * Matrix.RotationY(-Rotation.Y * Mathf.DegToRad) * Matrix.RotationZ(-Rotation.Z * Mathf.DegToRad) * Matrix.RotationX(-Rotation.X * Mathf.DegToRad);
        }
    }

    public struct Line
    {
        public Vector3 From;
        public Vector3 To;

        public Line(Vector3 from, Vector3 to)
        {
            From = from;
            To = to;
        }
    }

    public sealed class Mesh
    {
        internal VertexBuffer vertBuf;

        public int VertexCount;
        public int TriCount; // ARMv5 lacks hardware division, just small optimization

        public unsafe Mesh(Vertex[] vertices)
        {
            vertBuf = new VertexBuffer(Engine.Current.Graphics.device, sizeof(Vertex) * vertices.Length, Usage.None, VertexFormats.Position | VertexFormats.Texture1, Pool.SystemMemory);
            GraphicsStream strm = vertBuf.Lock(0, vertBuf.SizeInBytes, LockFlags.None);
            strm.Write(vertices);
            vertBuf.Unlock();

            VertexCount = vertices.Length;
            TriCount = VertexCount / 3;
        }

        public static Mesh FromFile(string fileName)
        {
            Stream strm = Engine.Current.OpenAsset(fileName);

            if (strm != null)
            {
                MD2Model mdl = new MD2Model(strm);

                Vertex[] vert = new Vertex[mdl.Triangles.Length * 3];

                for (int i = 0; i < mdl.Triangles.Length; i++)
                {
                    MD2Triangle t = mdl.Triangles[i];

                    vert[i * 3].Position = new Vector3(mdl.Frames[0].verts[t.verts[0]].x, mdl.Frames[0].verts[t.verts[0]].y, mdl.Frames[0].verts[t.verts[0]].z);
                    vert[i * 3].UV = new Vector2((float)mdl.Coords[t.uv[0]].u / mdl.Header.skinwidth, (float)mdl.Coords[t.uv[0]].v / mdl.Header.skinheight);

                    vert[i * 3 + 2].Position = new Vector3(mdl.Frames[0].verts[t.verts[1]].x, mdl.Frames[0].verts[t.verts[1]].y, mdl.Frames[0].verts[t.verts[1]].z);
                    vert[i * 3 + 2].UV = new Vector2((float)mdl.Coords[t.uv[1]].u / mdl.Header.skinwidth, (float)mdl.Coords[t.uv[1]].v / mdl.Header.skinheight);

                    vert[i * 3 + 1].Position = new Vector3(mdl.Frames[0].verts[t.verts[2]].x, mdl.Frames[0].verts[t.verts[2]].y, mdl.Frames[0].verts[t.verts[2]].z);
                    vert[i * 3 + 1].UV = new Vector2((float)mdl.Coords[t.uv[2]].u / mdl.Header.skinwidth, (float)mdl.Coords[t.uv[2]].v / mdl.Header.skinheight);
                }

                return new Mesh(vert);
            }

            return null;
        }
    }

    public sealed class Graphics
    {
        internal Device device;

        public Camera Camera;
        private Mesh spriteMesh;

        internal void LoadResources()
        {
            Vertex[] sprite = new Vertex[6];
            sprite[0] = new Vertex(new Vector3(0, 0, 0), new Vector2(0, 0));
            sprite[1] = new Vertex(new Vector3(1, 0, 0), new Vector2(1, 0));
            sprite[2] = new Vertex(new Vector3(1, 1, 0), new Vector2(1, 1));
            sprite[3] = new Vertex(new Vector3(1, 1, 0), new Vector2(1, 1));
            sprite[4] = new Vertex(new Vector3(0, 1, 0), new Vector2(0, 1));
            sprite[5] = new Vertex(new Vector3(0, 0, 0), new Vector2(0, 0));
            spriteMesh = new Mesh(sprite);
        }

        internal Graphics()
        {
            Engine.Current.Log("Graphics::ctor");

            PresentParameters pp = new PresentParameters();
            pp.AutoDepthStencilFormat = DepthFormat.D16;
            pp.BackBufferCount = 1;
            pp.BackBufferFormat = Format.R5G6B5;
            pp.BackBufferWidth = Engine.Current.Window.ViewportWidth;
            pp.BackBufferHeight = Engine.Current.Window.ViewportHeight;
            pp.EnableAutoDepthStencil = true;
            pp.PresentFlag = PresentFlag.None;
            pp.SwapEffect = SwapEffect.Discard;
            pp.MultiSample = MultiSampleType.None;
            pp.FullScreenPresentationInterval = PresentInterval.Default;
            pp.Windowed = true;  

            device = new Device(0, DeviceType.Default, Engine.Current.Window.Handle, CreateFlags.None, pp);
            Engine.Current.Log("Device created");
            
            Engine.Current.Log("Device width: {0}, height: {1}", Engine.Current.Window.ViewportWidth, Engine.Current.Window.ViewportHeight);
            AdapterInformation info = Manager.Adapters.Default;
            Engine.Current.Log("Renderer info");
            Engine.Current.Log("  Driver desc: {0}", info.Information.Description);
            Engine.Current.Log("  Driver name: {0}", info.Information.DriverName);

            device.RenderState.Lighting = false;
            device.RenderState.TexturePerspective = false;
            
            device.RenderState.CullMode = Cull.None;
            device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            device.RenderState.SourceBlend = Blend.SourceAlpha;

            Camera = new Camera();
        }

        internal void Begin()
        {
            device.RenderState.ZBufferEnable = true;

            device.SetTransform(TransformType.View, Camera.GetView());
            device.SetTransform(TransformType.Projection, Camera.GetProjection());

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.Blue, 1.0f, 0);
            device.BeginScene();
        }

        public void DrawMesh(Mesh mesh, Transform matrix, Material mat)
        {
            if (mesh != null)
            {
                if (mat.Texture != null)
                    device.SetTexture(0, mat.Texture.handle);

                device.RenderState.ZBufferWriteEnable = !mat.ZWrite;
                device.SetTransform(TransformType.World, matrix.Matrix);

                device.SetStreamSource(0, mesh.vertBuf, 0);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, mesh.TriCount);
            }
        }

        internal void BeginHUD()
        {
            device.SetTransform(TransformType.View, Matrix.Identity);
            device.SetTransform(TransformType.Projection, Matrix.OrthoLH(Engine.Current.Window.ViewportWidth, Engine.Current.Window.ViewportHeight, 0, 1));

            device.RenderState.ZBufferEnable = false;
        }

        public void DrawSprite(Material mat, Vector3 position, Vector3 size)
        {
            if (mat.Texture != null)
            {
                if (size.X == 0 || size.Y == 0)
                    size = new Vector3(mat.Texture.Width, mat.Texture.Height, 1);

                Transform t = new Transform(new Vector3(-(Engine.Current.Window.ViewportWidth / 2) + position.X, (Engine.Current.Window.ViewportHeight / 2) - position.Y - mat.Texture.Height, 0),
                    new Vector3(0, 0, 0), size);

                DrawMesh(spriteMesh, t, mat);
            }
        }

        internal void End()
        {
            device.EndScene();
            device.Present();
        }
    }
}
