using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsMobile.DirectX;
using Microsoft.WindowsMobile.DirectX.Direct3D;

namespace WMGame3D
{
    public sealed class TextRenderer
    {
        private Font fontObject;

        public TextRenderer(string typeFace, float size)
        {
            System.Drawing.Font fnt = new System.Drawing.Font(typeFace, size, System.Drawing.FontStyle.Regular);
            fontObject = new Font(Engine.Current.Graphics.device, fnt);
        }

        public void DrawString(string str, int x, int y, int r, int g, int b, int a)
        {
#if WIN32
            fontObject.DrawText(null, str, x, y, new ColorBGRA(r, g, b, 255));
#else
            fontObject.DrawText(null, str, x, y, System.Drawing.Color.FromArgb(r, g, b));
#endif
        }
    }
}
