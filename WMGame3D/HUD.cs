using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public sealed class HUD
    {
        private TextRenderer textRenderer;

        private Material uiHealth;
        private Material uiBonus;

        public HUD()
        {
            textRenderer = new TextRenderer("Arial", 12.0f);

            uiHealth.Texture = Texture2D.FromFile("hud/health.png");
            uiHealth.UseColor = true;
            uiHealth.AlphaValue = 0.5f;
        }

        public Rectf RelativeToAbs(Rectf rel)
        {
            Vector3 pos = new Vector3(rel.X * Engine.Current.Window.ViewportWidth, rel.Y * Engine.Current.Window.ViewportHeight, 0);
            Vector3 size = new Vector3(rel.W * Engine.Current.Window.ViewportWidth, rel.H * Engine.Current.Window.ViewportHeight, 1);

            return new Rectf(pos.X, pos.Y, size.X, size.Y);
        }

        public void DrawSpriteRelative(Material mat, Rectf rectf)
        {
            Vector3 pos = new Vector3(rectf.X * Engine.Current.Window.ViewportWidth, rectf.Y * Engine.Current.Window.ViewportHeight, 0);
            Vector3 size = new Vector3(rectf.W * Engine.Current.Window.ViewportWidth / Engine.Current.Graphics.Camera.Aspect, rectf.H * Engine.Current.Window.ViewportHeight, 1);

            Engine.Current.Graphics.DrawSprite(mat, pos, size);
        }

        public void Draw()
        {
            textRenderer.DrawString("HP: " + Game.Current.World.Player.Health, 5, 5, 255, 0, 0, 255);

            if(Game.Current.World.Player.BonusTime > 5)
                textRenderer.DrawString("Бонус: " + Game.Current.World.Player.Bonus.ToString(), 5, 17, 255, 0, 0, 255);

            uiHealth.Color = new Vector4(0.9f, 0, 0, 1.0f);
            DrawSpriteRelative(uiHealth, new Rectf(0.05f, 0.9f, 0.1f, 0.1f));
        }
    }
}
