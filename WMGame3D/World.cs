using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public abstract class Entity
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public BoundingBox Bounds;

        public Entity()
        {
            Scale = new Vector3(1, 1, 1);
        }

        public BoundingBox GetBounds()
        {
            return new BoundingBox(Position.X + Bounds.X, Position.Y + Bounds.Y, Position.Z + Bounds.Z, Bounds.X2, Bounds.Y2, Bounds.Z2);
        }

        protected Transform GetTransform()
        {
            return new Transform(Position, Rotation, Scale);
        }

        public virtual void Update()
        {

        }

        public virtual void Draw()
        {

        }
    }

    public sealed class SpawnDirector
    {
        private List<Type> entityTypes;
        private List<Type> pickupTypes;

        private float nextSpawn;
        private float nextPickupSpawn;
        public float TotalWorldTime;

        private void RegisterEntityTypes()
        {
            entityTypes = new List<Type>();
            pickupTypes = new List<Type>();

            entityTypes.Add(typeof(Asteroid));
            pickupTypes.Add(typeof(HealthPickup));
            pickupTypes.Add(typeof(BonusPickup));
        }

        public SpawnDirector()
        {
            RegisterEntityTypes();

            nextSpawn = 5.0f;
        }

        public void Update()
        {
            nextSpawn -= Engine.Current.DeltaTime;
            nextPickupSpawn -= Engine.Current.DeltaTime;
            

            if (nextSpawn < 0)
            {
                int rand = new Random().Next(0, entityTypes.Count - 1);
                Enemy ent = (Enemy)entityTypes[rand].GetConstructor(new Type[] { }).Invoke(null);
                ent.Position = new Vector3((float)new Random().Next(-World.Bounds, World.Bounds), -5, 150);
                Game.Current.World.Spawn(ent);
                nextSpawn = 1.0f;
            }

            if (nextPickupSpawn < 0)
            {
                int rand = new Random().Next(0, pickupTypes.Count);
                Pickup ent = (Pickup)pickupTypes[rand].GetConstructor(new Type[] { }).Invoke(null);
                ent.Position = new Vector3((float)new Random().Next(-World.Bounds, World.Bounds), -5, 150);
                Game.Current.World.Spawn(ent);
                nextPickupSpawn = 20.0f;
            }

            TotalWorldTime += Engine.Current.DeltaTime;
        }
    }

    public sealed class World
    {
        public const int Bounds = 21;

        public List<Entity> EntityList;
        private List<Entity> spawnList;
        private List<Entity> removalList;

        public Player Player;

        private HUD hud;
        private Mesh backQuad;
        private Material backMat;
        public float Time;

        public SpawnDirector spawnDirector;

        public World()
        {
            EntityList = new List<Entity>();
            spawnList = new List<Entity>();
            removalList = new List<Entity>();

            backQuad = Mesh.FromFile("skybox.md2");
            backMat.Texture = Texture2D.FromFile("bg.png");
            backMat.ZWrite = true;

            hud = new HUD();

            spawnDirector = new SpawnDirector();
        }

        public void StartGame()
        {
            Player = new Player();
            Spawn(Player);
        }

        public void Spawn(Entity ent)
        {
            if (ent != null)
                spawnList.Add(ent);
        }

        public void Destroy(Entity ent)
        {
            if (ent != null)
                removalList.Add(ent);
        }

        public void Update()
        {
            foreach(Entity ent in EntityList)
                ent.Update();

            foreach (Entity ent in spawnList)
                EntityList.Add(ent);

            foreach (Entity ent in removalList)
                EntityList.Remove(ent);

            spawnList.Clear();
            removalList.Clear();
            spawnDirector.Update();

            Time += Engine.Current.DeltaTime;
        }

        private void DrawBackground()
        {
            Engine.Current.Graphics.DrawMesh(backQuad, new Transform(new Vector3(0, 0, 0), new Vector3(90, 0, Time), Vector3.One), backMat);
        }

        public void Draw()
        {
            DrawBackground();

            foreach (Entity ent in EntityList)
                ent.Draw();
        }

        public void DrawUI()
        {
            hud.Draw();
        }
    }
}
