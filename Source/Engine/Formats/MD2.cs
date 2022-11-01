using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace WMGame3D
{
    public struct MD2Header
    {
        public int ident;                  /* magic number: "IDP2" */
        public int version;                /* version: must be 8 */

        public int skinwidth;              /* texture width */
        public int skinheight;             /* texture height */

        public int framesize;              /* size in bytes of a frame */

        public int num_skins;              /* number of skins */
        public int num_vertices;           /* number of vertices per frame */
        public int num_st;                 /* number of texture coordinates */
        public int num_tris;               /* number of triangles */
        public int num_glcmds;             /* number of opengl commands */
        public int num_frames;             /* number of frames */

        public int offset_skins;           /* offset skin data */
        public int offset_st;              /* offset texture coordinate data */
        public int offset_tris;            /* offset triangle data */
        public int offset_frames;          /* offset frame data */
        public int offset_glcmds;          /* offset OpenGL command data */
        public int offset_end;             /* offset end of file */
    }

    public struct MD2TexCoord
    {
        public ushort u, v;
    }

    public struct MD2Triangle
    {
        public int[] verts;
        public int[] uv;
    }

    public struct MD2Vertex
    {
        public float x, y, z;
        public byte normalIndex;
    }

    public struct MD2Frame
    {
        public Vector3 scale;
        public Vector3 translate;

        public string name;
        public MD2Vertex[] verts;
    }

    public sealed class MD2Model
    {
        public MD2Header Header;
        public MD2Triangle[] Triangles;
        public MD2TexCoord[] Coords;
        public MD2Frame[] Frames;

        public MD2Model(Stream strm)
        {
            BinaryReader reader = new BinaryReader(strm, Encoding.ASCII);

            Header = new MD2Header();
            Header.ident = reader.ReadInt32();
            Header.version = reader.ReadInt32();
            Header.skinwidth = reader.ReadInt32();
            Header.skinheight = reader.ReadInt32();
            Header.framesize = reader.ReadInt32();
            Header.num_skins = reader.ReadInt32();
            Header.num_vertices = reader.ReadInt32();
            Header.num_st = reader.ReadInt32();
            Header.num_tris = reader.ReadInt32();
            Header.num_glcmds = reader.ReadInt32();
            Header.num_frames = reader.ReadInt32();
            Header.offset_skins = reader.ReadInt32();
            Header.offset_st = reader.ReadInt32();
            Header.offset_tris = reader.ReadInt32();
            Header.offset_frames = reader.ReadInt32();
            Header.offset_glcmds = reader.ReadInt32();
            Header.offset_end = reader.ReadInt32();

            Triangles = new MD2Triangle[Header.num_tris];
            Coords = new MD2TexCoord[Header.num_st];
            Frames = new MD2Frame[Header.num_frames];

            reader.BaseStream.Seek(Header.offset_tris, SeekOrigin.Begin);

            for (int i = 0; i < Header.num_tris; i++)
            {
                Triangles[i] = new MD2Triangle();
                Triangles[i].verts = new int[] { reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() };
                Triangles[i].uv = new int[] { reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() };
            }

            reader.BaseStream.Seek(Header.offset_st, SeekOrigin.Begin);

            for (int i = 0; i < Header.num_st; i++)
            {
                Coords[i] = new MD2TexCoord();
                Coords[i].u = reader.ReadUInt16();
                Coords[i].v = reader.ReadUInt16();
            }

            reader.BaseStream.Seek(Header.offset_frames, SeekOrigin.Begin);

            for (int i = 0; i < Header.num_frames; i++)
            {
                Frames[i] = new MD2Frame();
                Frames[i].scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Frames[i].translate = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Frames[i].name = new string(reader.ReadChars(16));
                Frames[i].verts = new MD2Vertex[Header.num_vertices];

                for(int j = 0; j < Header.num_vertices; j++)
                {
                    Frames[i].verts[j].x = ((float)reader.ReadByte() * Frames[i].scale.X) + Frames[i].translate.X;
                    Frames[i].verts[j].y = ((float)reader.ReadByte() * Frames[i].scale.Y) + Frames[i].translate.Y;
                    Frames[i].verts[j].z = ((float)reader.ReadByte() * Frames[i].scale.Z) + Frames[i].translate.Z;
                    Frames[i].verts[j].normalIndex = reader.ReadByte();
                }
            }
        }
    }
}
