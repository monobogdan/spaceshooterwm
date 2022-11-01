using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WMGame3D
{
    [Flags]
    public enum Perks
    {
        None,
        Bonus = 1, // Give bonus at game start
        IncreaseHPLimit = 2, // Increases HP limit for healthkits
        HeadStart = 4, // Gives some score initially
        SecondChance = 8, // This will revive player after death
        Cheater = 16 // Secret perk :)
    }

    public sealed class Save
    {
        private const int Header = 0x5454;
        public static string[] PerkDescription =
        {
            "",
            "Этот перк даст вам случайный бонус в начале игры. Удобно ведь?"
        }; // Temporary solution

        private const string FileName = "/player.bin";
        public struct Statistics
        {
            public int TotalGames;
            public int TotalMoneyEarned;
            public int TotalMissions;
            public bool UsedCheats;
        }

        public Statistics Stats;
        public int Level;
        public int Experience;
        public int Money;
        public int HighScore;
        public Perks Perks;

        public Save()
        {
            Engine.Current.Log("Save::.ctor");

            Load();
        }

        public void StartNewGame()
        {
            Stats = new Statistics();
            Level = 1;
            Experience = 0;
            Money = 1000;
            Perks = Perks.None;
            
            SaveChanges();
        }

        private string GetSaveFilePath()
        {
            return Engine.Current.Window.Info.AppPath + FileName;
        }

        public void Load()
        {
            if (!File.Exists(GetSaveFilePath()))
            {
                StartNewGame();

                return;
            }

            Stream stream = File.OpenRead(GetSaveFilePath());
            BinaryReader reader = new BinaryReader(stream);
            int version = reader.ReadInt16();

            if(version != Header)
            {
                Engine.Current.Log("Incorrect save file version, current is {0}, while this one is {1}", Header, version);

                StartNewGame();
                return;
            }

            Stats.TotalGames = reader.ReadInt16();
            Stats.TotalMissions = reader.ReadInt16();
            Stats.TotalMoneyEarned = reader.ReadInt16();
            Stats.UsedCheats = reader.ReadBoolean();
            Level = reader.ReadInt16();
            Experience = reader.ReadInt16();
            Money = reader.ReadInt16();
            HighScore = reader.ReadInt32();
            Perks = (Perks)reader.ReadInt16();

            reader.Close();
        }

        public void SaveChanges()
        {
            try
            {
                Directory.CreateDirectory(Engine.Current.Window.Info.AppPath);
            }
            catch
            {

            }

            Stream stream = File.OpenWrite(GetSaveFilePath());
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write((short)Header);
            writer.Write((short)Stats.TotalGames);
            writer.Write((short)Stats.TotalMissions);
            writer.Write((short)Stats.TotalMoneyEarned);
            writer.Write(Stats.UsedCheats);
            writer.Write((short)Level);
            writer.Write((short)Experience);
            writer.Write((short)Money);
            writer.Write(HighScore);
            writer.Write((short)Perks);

            writer.Close();
        }

        public int GetExperienceToNextLevel()
        {
            return Level * 1000;
        }

        public void GivePerk(Perks perk)
        {
            Perks |= perk;
        }
        
        public bool HasPerk(Perks perk)
        {
            byte p = (byte)this.Perks;
            byte p2 = (byte)perk;

            return (p & p2) == 1; // Old C# lacks HasFlag
        }
    }
}
