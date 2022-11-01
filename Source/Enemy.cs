using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public class Enemy : Entity
    {
        public Mesh Mesh;
        public Material Material;

        protected static SoundStream strmExplode;
        private SoundInstance sndExplode;

        public int Health;

        protected static void PreloadBaseResources()
        {

        }

        public Enemy()
        {
            Health = 100;

            if(strmExplode == null)
            {
                strmExplode = SoundStream.FromWave("sounds/expl.wav");
            }

            sndExplode = new SoundInstance(strmExplode);
        }

        public override void Update()
        {
            base.Update();

            if (Position.Z < -10)
                Game.Current.World.Destroy(this);

            if(Health < 0)
            {
                sndExplode.Play();
                Game.Current.World.Destroy(this);
            }
        }

        public override void Draw()
        {
            base.Draw();

            Engine.Current.Graphics.DrawMesh(Mesh, GetTransform(), Material);
        }
    }
}
