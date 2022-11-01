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

                if (Game.Current.World.Info.Stage < 2)
                    nextSpawn = 1.0f;
                else
                    nextSpawn = 0.7f;
            }

            if (nextPickupSpawn < 0)
            {
                int rand = new Random().Next(0, pickupTypes.Count);

                if (pickupTypes[rand] == typeof(HealthPickup) && Game.Current.World.Player.Health > 50)
                    return;

                Pickup ent = (Pickup)pickupTypes[rand].GetConstructor(new Type[] { }).Invoke(null);
                ent.Position = new Vector3((float)new Random().Next(-World.Bounds, World.Bounds), -5, 150);
                Game.Current.World.Spawn(ent);
                nextPickupSpawn = 20.0f;
            }

            TotalWorldTime += Engine.Current.DeltaTime;
        }
    }

    public struct MatchState
    {
        public int Score;
        public int NumKilled;
        public int NumBonuses;

        public float Stage;
    }

    public sealed class World
    {
        public const int Bounds = 21;
        public const float StageTime = 30.0f;

        public List<Entity> EntityList;
        private List<Entity> spawnList;
        private List<Entity> removalList;

        public MatchState Info;
        public Player Player;

        public HUD HUD;
        private Background backGround;

        public float Time;

        public SpawnDirector spawnDirector;

        public World()
        {
            EntityList = new List<Entity>();
            spawnList = new List<Entity>();
            removalList = new List<Entity>();

            Info = new MatchState();
            backGround = new Background();
            HUD = new HUD();

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

        private void UpdateScore()
        {
            Info.Score += 1 * Game.Current.Save.Level;
        }

        public void Update()
        {
            if (Player.Health > 0)
            {
                foreach (Entity ent in EntityList)
                    ent.Update();

                foreach (Entity ent in spawnList)
                    EntityList.Add(ent);

                foreach (Entity ent in removalList)
                    EntityList.Remove(ent);

                spawnList.Clear();
                removalList.Clear();
                spawnDirector.Update();

                float prevStage = Info.Stage;
                Info.Stage = Mathf.Clamp(Time / StageTime, 0, 5);

                if ((int)Math.Round(prevStage) < (int)Math.Round(Info.Stage))
                    HUD.ShowAlert("Вы перешли на этап " + (int)Info.Stage, new Vector4(0, 255, 0, 255), 3, true);

                UpdateScore();
            }

            
            HUD.Update();
            Time += Engine.Current.DeltaTime;
        }

        public void Draw()
        {
            backGround.Draw();

            foreach (Entity ent in EntityList)
                ent.Draw();
        }

        public void DrawUI()
        {
            HUD.Draw();
        }
    }
}
