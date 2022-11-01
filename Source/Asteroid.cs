using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public sealed class Asteroid : Enemy
    {
        private static Mesh mesh;
        private static Material material;

        public const float BaseSpeed = 25;
        public const float Rotate = 56;
        public const int HPAmount = 20;

        private float speed;

        public static void Preload()
        {
            PreloadBaseResources();

            if(mesh == null)
            {
                mesh = Mesh.FromFile("asteroid.md2");
                material.Texture = Texture2D.FromFile("as0.jpg");
                
            }
        }

        public Asteroid()
        {
            Preload();

            Mesh = mesh;
            Material = material;

            speed = BaseSpeed * (1.0f + (float)new Random().NextDouble());
            Bounds = new BoundingBox(-10, -10, -10, 10, 10, 10);
        }

        public override void Update()
        {
            base.Update();

            if (Game.Current.World.Player.GetBounds().Intersects(GetBounds()))
            {
                if(Game.Current.World.Player.Bonus != PlayerBonus.Invincible)
                    Game.Current.World.Player.Health -= HPAmount;
                Game.Current.World.Destroy(this);

                Game.Current.World.HUD.ShakeCamera();
            }

            Rotation.X += Rotate * Engine.Current.DeltaTime;
            Position.Z -= (speed * Game.Current.World.Info.Stage) * Engine.Current.DeltaTime;
        }
    }
}
