using System;
using System.Collections.Generic;
using System.Text;

using Android;
using Android.Graphics;

namespace WMGame3D
{
    public sealed class FontManager
    {
        private Bitmap fontBitmap;
        private Canvas canvas;
        private Material material;

        internal Paint paint;

        public FontManager()
        {
            fontBitmap = Bitmap.CreateBitmap(Engine.Current.Window.ViewportWidth, Engine.Current.Window.ViewportHeight, Bitmap.Config.Argb8888);
            canvas = new Canvas(fontBitmap);

            material = new Material();
            material.Texture = new Texture2D();

            paint = new Paint();
        }

        internal void Begin()
        {
            fontBitmap.EraseColor(Color.Transparent.ToArgb());
        }

        internal void DrawString(NativeFont font, string str, int x, int y, int r, int g, int b, int a)
        {
            paint.SetTypeface(font.typeface);
            paint.Color = new Color(r, g, b, a);
            paint.TextSize = font.Size;
            canvas.DrawText(str, (float)x, (float)y - paint.GetFontMetrics().Top, paint);
        }

        internal void End()
        {
            IntPtr ptr = fontBitmap.LockPixels();
            material.Texture.Upload(ptr, fontBitmap.Width, fontBitmap.Height);
            fontBitmap.UnlockPixels();
            
            Engine.Current.Graphics.DrawSprite(material, Vector3.Zero, new Vector3(fontBitmap.Width, fontBitmap.Height, 0));
        }
    }

    public sealed class NativeFont
    {
        internal Typeface typeface;
        public float Size;

        public NativeFont(string typeFace, float size)
        {
            Size = size;
            typeface = Typeface.Create(typeFace, TypefaceStyle.Normal);
        }

        public int MeasureString(string str)
        {
            // A bit hacky
            Engine.Current.FontManager.paint.SetTypeface(typeface);
            Engine.Current.FontManager.paint.TextSize = Size;

            return (int)Engine.Current.FontManager.paint.MeasureText(str);
        }

        public void DrawString(string str, int x, int y, int r, int g, int b, int a)
        {
            Engine.Current.FontManager.DrawString(this, str, x, y, r, g, b, a);
        }
    }
}
