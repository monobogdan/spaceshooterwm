using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public enum PlayerBonus
    {
        None,
        DoubleDamage,
        QuickDick,
        DoubleTheFun,
        Invincible,
        MaxBonus
    }

    public sealed class Player : Entity
    {
        public const float Speed = 15;
        public const float RotationEffect = 5;

        private bool fovState;
        private float fovFactor;
        private float nextAttack;

        public float BonusTime;

        public float Health;
        public PlayerBonus Bonus;

        private Mesh model;
        private Material material;
        private SoundInstance sndFire;
        private SoundInstance sndPowerup;

        public Player()
        {
            Position = new Vector3(0, -5, 20);

            model = Mesh.FromFile("player.md2");
            material.Texture = Texture2D.FromFile("player.png");
            sndFire = new SoundInstance(SoundStream.FromWave("sounds/fire.wav"));
            sndPowerup = new SoundInstance(SoundStream.FromWave("sounds/powerup.wav"));

            Health = 10;

            Bounds = new BoundingBox(-10, -10, -10, 10, 10, 10);
            Scale = new Vector3(1, -1, 1);

            TakeBonus(PlayerBonus.DoubleTheFun);
        }

        private void UpdateFOVEffect()
        {
            fovFactor = (float)Math.Sin(Game.Current.World.Time) / 3.14f;

            Engine.Current.Graphics.Camera.FOV = Mathf.Lerp(75, 90, fovFactor);
        }

        public void TakeBonus(PlayerBonus bonus)
        {
            Bonus = bonus;
            BonusTime = 10;

            sndPowerup.Play();
            Engine.Current.Log("Got bonus: {0}", bonus);
        }

        private void UpdateInput()
        {
            float strafe = 0;

            if (Engine.Current.Input.IsPressed(Key.Left))
                strafe = -1;

            if (Engine.Current.Input.IsPressed(Key.Right))
                strafe = 1;

            if (Engine.Current.Input.Touch.IsTouching)
            {
                if (Engine.Current.Input.Touch.X < Engine.Current.Window.ViewportWidth / 2)
                    strafe = -1;
                else
                    strafe = 1;
            }

            Position.X += strafe * (Speed * Engine.Current.DeltaTime);
            Rotation.Z = strafe * -RotationEffect;
        }

        private void SpawnProjectile(Vector3 pos)
        {
            Projectile proj = new Projectile(65, 1, false);
            proj.Position = pos;

            Game.Current.World.Spawn(proj);
        }

        public override void Update()
        {
            base.Update();
            
            UpdateFOVEffect();

            UpdateInput();

            if (nextAttack < 0)
            {
                if (Bonus == PlayerBonus.DoubleTheFun)
                {
                    Vector3 p1 = Position;
                    p1.X -= 5;
                    Vector3 p2 = Position;
                    p2.X += 5;

                    SpawnProjectile(p1);
                    SpawnProjectile(p2);
                    nextAttack = 0.5f;
                }
                else
                {
                    SpawnProjectile(Position);

                    if (Bonus == PlayerBonus.QuickDick)
                        nextAttack = 0.3f;
                    else
                        nextAttack = 0.5f;
                }
                sndFire.Play();
            }

            if (BonusTime < 0)
                Bonus = PlayerBonus.None;

            Position.X = Mathf.Clamp(Position.X, -World.Bounds, World.Bounds);
            nextAttack -= Engine.Current.DeltaTime;
            BonusTime -= Engine.Current.DeltaTime;
        }

        public override void Draw()
        {
            base.Draw();

            if(Health > 0)
                Engine.Current.Graphics.DrawMesh(model, GetTransform(), material);
        }
    }
}
