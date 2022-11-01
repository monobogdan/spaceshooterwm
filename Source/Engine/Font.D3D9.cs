using System;
using System.Collections.Generic;
using System.Text;
using SharpDX;
using SharpDX.Direct3D9;

namespace WMGame3D
{
    /// <summary>
    /// It doesn't do anything on Win32, since font manager is D3DX
    /// </summary>
    public sealed class FontManager
    {
        public FontManager()
        {

        }

        public void Begin()
        {

        }

        public void End()
        {

        }

    }

    public sealed class NativeFont
    {
        internal Font fontObject;
        public float Size;

        public NativeFont(string typeFace, float size)
        {
            System.Drawing.Font fnt = new System.Drawing.Font(typeFace, size, System.Drawing.GraphicsUnit.Pixel);
            fontObject = new Font(Engine.Current.Graphics.device, fnt);

            Size = size;
        }

        public int MeasureString(string str)
        {
            return (int)fontObject.MeasureText(null, str, FontDrawFlags.Top).Right;
        }

        public void DrawString(string str, int x, int y, int r, int g, int b, int a)
        {
            fontObject.DrawText(null, str, x, y, new ColorBGRA((byte)r, (byte)g, (byte)b, 255));
        }
    }
}
