using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public static class Mathf
    {
        public const float DegToRad = 0.0174533f;

        public static float Lerp(float a, float b, float f)
        {
            return (a * (1.0f - f)) + (b * f);
        }

        public static float Clamp(float val, float min, float max)
        {
            return val < min ? min : (val > max ? max : val);
        }
    }

    public struct BoundingBox
    {
        public float X, Y, Z;
        public float X2, Y2, Z2;

        public BoundingBox(float x, float y, float z, float x2, float y2, float z2)
        {
            X = x;
            Y = y;
            Z = z;
            X2 = x2;
            Y2 = y2;
            Z2 = z2;
        }

        public bool Intersects(BoundingBox box)
        {
            return (X < box.X + box.X2 && Y < box.Y + box.Y2 && Z < box.Z + box.Z2 && box.X < X + X2 && box.Y < Y + Y2 && box.Z < Z + Z2);
        }
    }

    public struct Vector3
    {
        public static Vector3 Zero = new Vector3(0, 0, 0);
        public static Vector3 One = new Vector3(1, 1, 1);

        public float X, Y, Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public struct Vector4
    {

        public float X, Y, Z, W;

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }

    public struct Rectf
    {
        public float X, Y, W, H;

        public Rectf(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }
}
