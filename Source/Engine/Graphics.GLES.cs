using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using Android.Graphics;

namespace WMGame3D
{

    public struct Transform
    {
        public Matrix4 Matrix;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public Transform(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            Scale = scale;

            Matrix = Matrix4.Scale(scale.X, scale.Y, -scale.Z) *
                Matrix4.RotateX(rot.X * Mathf.DegToRad) *
                Matrix4.RotateZ(rot.Z * Mathf.DegToRad) *
                Matrix4.RotateY(rot.Y * Mathf.DegToRad) *
                Matrix4.Translation(pos.X, pos.Y, -pos.Z);
        }
    }

    public enum TextureAddress
    {
        Repeat,
        Mirror
    }

    public sealed class Texture2D
    {
        internal int handle;
        public int Width;
        public int Height;

        public TextureAddress AddressMode;

        // Assuming RGBA pixel format
        public Texture2D()
        {
            GL.GenTextures(1, out handle);
        }

        public void Upload(IntPtr pointer, int width, int height)
        {
            if (width < 8 || height < 8 || pointer == IntPtr.Zero)
                throw new ArgumentException("Failed to create texture");

            
            GL.BindTexture(All.Texture2D, handle);
            GL.TexImage2D(All.Texture2D, 0, (int)All.Rgba, width, height, 0, All.Rgba, All.UnsignedByte, pointer);
            GL.TexParameter(All.Texture2D, All.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(All.Texture2D, All.TextureMagFilter, (int)All.Linear);

            Width = width;
            Height = height;
        }

        public static Texture2D FromFile(string fileName)
        {
            Stream asset = Engine.Current.OpenAsset(fileName);

            if (asset != null)
            {
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InPreferredConfig = Bitmap.Config.Argb8888;
                Bitmap bitmap = BitmapFactory.DecodeStream(asset, new Rect(), options);
                
                if(bitmap != null)
                {
                    Texture2D ret = new Texture2D();
                    IntPtr ptr = bitmap.LockPixels();
                    ret.Upload(ptr, bitmap.Width, bitmap.Height);
                    bitmap.UnlockPixels();

                    return ret;
                }

                Engine.Current.Log("BitmapFactory failed: {0}", fileName);
            }

            return null;
        }
    }

    public struct Material
    {
        public bool UseColor;
        public Vector4 Color;
        public Texture2D Texture;

        public bool ZWrite;
        public bool AlphaBlend;
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
            Near = 0.1f;
            Far = 150.0f;
        }

        public Matrix4 GetProjection()
        {
            Aspect = (float)Engine.Current.Window.ViewportWidth / Engine.Current.Window.ViewportHeight;
            return Matrix4.Perspective(FOV * Mathf.DegToRad, Aspect, Near, Far);
        }

        public Matrix4 GetView()
        {
            return Matrix4.Translation(-Position.X, -Position.Y, Position.Z) * Matrix4.RotateY(-Rotation.Y * Mathf.DegToRad) * Matrix4.RotateZ(-Rotation.Z * Mathf.DegToRad) * Matrix4.RotateX(-Rotation.X * Mathf.DegToRad);
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
        internal int vbo;

        public int VertexCount;
        public int TriCount;

        public unsafe Mesh(Vertex[] vertices)
        {
            GL.GenBuffers(1, out vbo);
            GL.BindBuffer(All.ArrayBuffer, vbo);
            GL.BufferData(All.ArrayBuffer, new IntPtr(vertices.Length * sizeof(Vertex)), vertices, All.StaticDraw);

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
                    vert[i * 3].UV = new Vector2((float)mdl.Coords[t.uv[0]].u / mdl.Header.skinwidth, 1 - ((float)mdl.Coords[t.uv[0]].v / mdl.Header.skinheight));

                    vert[i * 3 + 2].Position = new Vector3(mdl.Frames[0].verts[t.verts[1]].x, mdl.Frames[0].verts[t.verts[1]].y, mdl.Frames[0].verts[t.verts[1]].z);
                    vert[i * 3 + 2].UV = new Vector2((float)mdl.Coords[t.uv[1]].u / mdl.Header.skinwidth, 1 - ((float)mdl.Coords[t.uv[1]].v / mdl.Header.skinheight));

                    vert[i * 3 + 1].Position = new Vector3(mdl.Frames[0].verts[t.verts[2]].x, mdl.Frames[0].verts[t.verts[2]].y, mdl.Frames[0].verts[t.verts[2]].z);
                    vert[i * 3 + 1].UV = new Vector2((float)mdl.Coords[t.uv[2]].u / mdl.Header.skinwidth, 1 - ((float)mdl.Coords[t.uv[2]].v / mdl.Header.skinheight));
                }

                return new Mesh(vert);
            }

            return null;
        }
    }

    public sealed class Graphics
    {

        public Camera Camera;
        private Mesh spriteMesh;

        public void LoadResources()
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

        private void InitState()
        {
            GL.EnableClientState(All.VertexArray);
            GL.EnableClientState(All.TextureCoordArray);

            GL.BlendFunc(All.SrcAlpha, All.OneMinusSrcAlpha);
            GL.Enable(All.Texture2D);
        }

        // The context is created by Android GLSurfaceView, or iOS GLKit
        internal Graphics()
        {
            Engine.Current.Log("Graphics::ctor");

            InitState();

            Engine.Current.Log("Device width: {0}, height: {1}", Engine.Current.Window.ViewportWidth, Engine.Current.Window.ViewportHeight);
            Engine.Current.Log("Renderer info");
            Engine.Current.Log("  Driver desc: {0}", GL.GetString(All.Renderer));
            Engine.Current.Log("  Driver name: {0}", GL.GetString(All.Vendor));

            Camera = new Camera();
        }

        internal unsafe void Begin()
        {
            Matrix4 proj = Camera.GetProjection();
            GL.MatrixMode(All.Projection);
            GL.LoadMatrix(&proj.Row0.X);

            GL.Enable(All.DepthTest);
            GL.Disable(All.Blend);

            GL.ClearColor(0, 0, 1, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        }

        private void SetState(All state, bool enabled)
        {
            if (enabled)
                GL.Enable(state);
            else
                GL.Disable(state);
        }

   
        public unsafe void DrawMesh(Mesh mesh, Transform matrix, Material mat)
        {
            if (mesh != null)
            {
                if (mat.Texture != null)
                {
                    GL.BindTexture(All.Texture2D, mat.Texture.handle);
                }

                GL.DepthMask(!mat.ZWrite);
                //SetState(All.AlphaTest, mat.AlphaValue > 0);

                if (mat.AlphaValue > 0)
                    GL.AlphaFunc(All.Lequal, mat.AlphaValue);

                Matrix4 modelView = matrix.Matrix * Camera.GetView();
                GL.MatrixMode(All.Modelview);
                GL.LoadMatrix(&modelView.Row0.X);
                
                GL.BindBuffer(All.ArrayBuffer, mesh.vbo);
                GL.VertexPointer(3, All.Float, sizeof(Vertex), new IntPtr(0));
                GL.TexCoordPointer(2, All.Float, sizeof(Vertex), new IntPtr(12));
                GL.DrawArrays(All.Triangles, 0, mesh.VertexCount);
            }
        }

        internal void BeginUI()
        {
            GL.MatrixMode(All.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(All.Projection);
            GL.LoadIdentity();
            GL.Ortho(-(Engine.Current.Window.ViewportWidth / 2), Engine.Current.Window.ViewportWidth / 2, Engine.Current.Window.ViewportHeight / 2, -(Engine.Current.Window.ViewportHeight / 2), 0, 1);

            GL.Disable(All.DepthTest);
            GL.Enable(All.Blend);
        }

        public void DrawSprite(Material mat, Vector3 position, Vector3 size)
        {
            if (mat.Texture != null)
            {
                if (size.X == 0 || size.Y == 0)
                    size = new Vector3(mat.Texture.Width, mat.Texture.Height, 1);

                Transform t = new Transform(new Vector3(-(Engine.Current.Window.ViewportWidth / 2) + position.X, -(Engine.Current.Window.ViewportHeight / 2) + position.Y, 0),
                    new Vector3(0, 0, 0), size);

                DrawMesh(spriteMesh, t, mat);
            }
        }

        

        internal void End()
        {
            GL.Flush();
            // GLSurfaceView will automatically call SwapBuffers
        }
    }
}
