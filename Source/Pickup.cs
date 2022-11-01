using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public abstract class Pickup : Entity
    {
        public const float Speed = 25.0f;

        public Mesh Mesh;
        public Material Material;

        public Pickup()
        {
            Mesh = Mesh.FromFile("pickup.md2");
            Bounds = new BoundingBox(-15, -15, -15, 15, 15, 15);
        }

        public override void Draw()
        {
            base.Draw();

            Engine.Current.Graphics.DrawMesh(Mesh, GetTransform(), Material);
        }

        public override void Update()
        {
            base.Update();

            if(GetBounds().Intersects(Game.Current.World.Player.GetBounds()))
            {
                Pick();

                Game.Current.World.Destroy(this);
            }

            Rotation.Y += (Speed * 3) * Engine.Current.DeltaTime;
            Position.Z -= Speed * Engine.Current.DeltaTime;
        }

        public abstract void Pick();
    }

    public class HealthPickup : Pickup
    {

        public HealthPickup()
            : base()
        {
            Material.Texture = Texture2D.FromFile("health.jpg");
        }

        public override void Pick()
        {
            Game.Current.World.Player.Health = Mathf.Clamp(Game.Current.World.Player.Health + 40, 0, 120);
        }
    }

    public class BonusPickup : Pickup
    {

        public BonusPickup()
            : base()
        {
            Material.Texture = Texture2D.FromFile("upgrade.jpg");
        }

        public override void Pick()
        {
            PlayerBonus bonus = (PlayerBonus)new Random().Next(1, (int)PlayerBonus.MaxBonus);

            Game.Current.World.Player.TakeBonus(bonus);
            Game.Current.World.HUD.ShowAlert(bonus.ToString(), new Vector4(0, 0, 0, 0), 3, true);
        }
    }
}
