using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public sealed class Projectile : Entity
    {
        public const float Speed = 5;

        public Mesh Mesh;
        public Material Material;

        private float lifeTime;
        private int damage;
        private float direction;

        private bool isPlayer;

        public Projectile(int damage, float dir, bool isPlayer)
        {
            Mesh = Mesh.FromFile("projectile.md2");
            Material.AlphaValue = 0.5f;
            Material.Texture = Texture2D.FromFile("projectile.png");

            this.damage = damage;
            this.direction = dir;
            this.isPlayer = isPlayer;
            lifeTime = 1;

            Bounds = new BoundingBox(-5, -5, -5, 5, 5, 5);
        }

        public override void Draw()
        {
            base.Draw();

            Engine.Current.Graphics.DrawMesh(Mesh, GetTransform(), Material);
        }

        

        public override void Update()
        {
            base.Update();

            foreach (Entity ent in Game.Current.World.EntityList)
            {
                if (ent.GetBounds().Intersects(GetBounds()))
                {
                    if (ent is Enemy)
                    {
                        ((Enemy)ent).Health -= damage;
                        Game.Current.World.Destroy(this);
                    }
                }
            }

            if (lifeTime < 0)
                Game.Current.World.Destroy(this);
            lifeTime -= Engine.Current.DeltaTime;

            Position.Z += direction * Speed;
        }
    }
}
