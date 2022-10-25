using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public sealed class Asteroid : Enemy
    {
        public const float Speed = 15;
        public const float Rotate = 56;
        public const int HPAmount = 20;

        public Asteroid()
        {
            Mesh = Mesh.FromFile("asteroid.md2");
            Material.Texture = Texture2D.FromFile("as0.jpg");

            Bounds = new BoundingBox(-10, -10, -10, 10, 10, 10);
        }

        public override void Update()
        {
            base.Update();

            if (Game.Current.World.Player.GetBounds().Intersects(GetBounds()))
            {
                Game.Current.World.Player.Health -= HPAmount;
                Game.Current.World.Destroy(this);
            }

            Rotation.X += Rotate * Engine.Current.DeltaTime;
            Position.Z -= Speed * Engine.Current.DeltaTime;
        }
    }
}
