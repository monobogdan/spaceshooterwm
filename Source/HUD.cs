using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public sealed class HUD
    {
        private string[] Phrases = { "Похоже и вам прострелили колено!", "Хилки за донат тут нет!", "Ну что, будем меряться с другими!?" };

        private static Vector4 UIColor = new Vector4(255, 0, 0, 255);
        private NativeFont textRenderer;

        private Material uiHealth;
        private Material uiBonus;

        private string alertText;
        private Vector4 alertColor;
        private bool alertColorBlend;
        private float alertTime;
        private int endGamePhrase;

        private float shakeTime;

        public HUD()
        {
            textRenderer = new NativeFont("Arial", GUI.GetAbsFontSize(12.0f));

            uiHealth.Texture = Texture2D.FromFile("hud/health.png");
            uiHealth.UseColor = true;
            uiHealth.AlphaValue = 0.5f;

            endGamePhrase = new Random().Next(0, Phrases.Length);
        }

        public void ShowAlert(string str, Vector4 color, float time, bool colorBlend)
        {
            alertText = str;
            alertColor = color;
            alertTime = time;
            alertColorBlend = false;

            alertColorBlend = colorBlend;
        }

        public void ShakeCamera()
        {
            shakeTime = 1.0f;
        }

        public void DrawSpriteRelative(Material mat, Rectf rectf)
        {
            Vector3 pos = new Vector3(rectf.X * Engine.Current.Window.ViewportWidth, rectf.Y * Engine.Current.Window.ViewportHeight, 0);
            Vector3 size = new Vector3(rectf.W * Engine.Current.Window.ViewportWidth / Engine.Current.Graphics.Camera.Aspect, rectf.H * Engine.Current.Window.ViewportHeight, 1);

            Engine.Current.Graphics.DrawSprite(mat, pos, size);
        }

        public void DrawTextRelative(string text, Vector4 rgb, float relX, float relY)
        {
            Rectf abs = GUI.RelativeToAbs(new Rectf(relX, relY, 0, 0));

            textRenderer.DrawString(text, (int)abs.X, (int)abs.Y, (int)rgb.X, (int)rgb.Y, (int)rgb.Z, (int)rgb.W);
        }

        private void UpdateInput()
        {
            if (Engine.Current.Input.Touch.IsTouching || Engine.Current.Input.IsPressed(Key.Fire1))
            {
                Game.Current.State = GameState.MainMenu;

                if (Game.Current.World.Info.Score > Game.Current.Save.HighScore)
                {
                    Game.Current.Save.HighScore = Game.Current.World.Info.Score;
                    Game.Current.Save.SaveChanges();
                }
            }
        }

        public void Update()
        {
            alertTime -= Engine.Current.DeltaTime;
            shakeTime -= Engine.Current.DeltaTime;

            if(Game.Current.World.Player.Health <= 0)
                UpdateInput();

            if (shakeTime > 0)
                Engine.Current.Graphics.Camera.Position = new Vector3(1.5f * (float)Math.Sin(Game.Current.World.Time * 55), 0, 0);
            else
                Engine.Current.Graphics.Camera.Position = new Vector3(0.5f * (float)Math.Sin(Game.Current.World.Time * 5), 0, 0);
        }

        public void Draw()
        {
            if(Game.Current.World.Player.Health > 0)
            {
                if (alertTime > 0)
                {
                    int aX = Engine.Current.Window.ViewportWidth / 2 - (textRenderer.MeasureString(alertText) / 2);
                    int aY = Engine.Current.Window.ViewportHeight / 2;

                    float blendFactor = ((float)Math.Sin(Game.Current.World.Time * 15) + (3.14f / 2));
                    blendFactor /= 3.14f;
                    blendFactor *= 255;

                    Engine.Current.Log(blendFactor.ToString());

                    if (alertColorBlend)
                        alertColor = new Vector4(255, 0, 0, 255);

                    textRenderer.DrawString(alertText, aX, aY, (int)alertColor.X, (int)alertColor.Y, (int)alertColor.Z, (int)alertColor.W);
                }

                DrawTextRelative(Game.Current.World.Player.Health.ToString(), UIColor, 0.01f, 0.01f);
                DrawTextRelative("Счёт: " + Game.Current.World.Info.Score.ToString(), UIColor, 0.01f, 0.05f);
                DrawTextRelative("Фраги: " + Game.Current.World.Info.NumKilled.ToString(), UIColor, 0.01f, 0.1f);

                uiHealth.Color = new Vector4(0.9f, 0, 0, 1.0f);
            }
            else
            {
                textRenderer.DrawString(Phrases[endGamePhrase],
                    Engine.Current.Window.ViewportWidth / 2 - (textRenderer.MeasureString(Phrases[endGamePhrase]) / 2), 
                    Engine.Current.Window.ViewportHeight / 2, 255, 0, 0, 255);

                DrawTextRelative("Счёт: " + Game.Current.World.Info.Score.ToString(), UIColor, 0.01f, 0.7f);
                DrawTextRelative("Стадия: " + ((int)Game.Current.World.Info.Stage).ToString(), UIColor, 0.01f, 0.75f);
            }
        }

            
    }
}
