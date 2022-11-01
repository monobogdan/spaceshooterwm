using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public enum GameState
    {
        MainMenu,
        InGame
    }

    public sealed class Game
    {
        public World World;
        public MainMenu MainMenu;

        public float TotalTime;

        public GameState State;

        private SoundInstance ambience;

        public Save Save;

        private static Game current;
        public static Game Current
        {
            get
            {
                return current;
            }
            set
            {
                current = value;
            }
        }

        public Game()
        {
            current = this;

            ambience = new SoundInstance(SoundStream.FromWave("sounds/theme0.wav"));
            ambience.Play();

            Save = new Save();

            MainMenu = new MainMenu();
            MainMenu.PostInitialize();
        }

        public void StartMatch()
        {
            State = GameState.InGame;
            World = new World();
            World.StartGame();
        }

        public void Update()
        {
            TotalTime += Engine.Current.DeltaTime;

            if(State == GameState.MainMenu)
                MainMenu.Update();
            else
                World.Update();
        }

        public void Draw()
        {
            if (State == GameState.MainMenu)
                MainMenu.Draw();
            else
                World.Draw();
        }

        public void DrawUI()
        {
            if (State == GameState.MainMenu)
                MainMenu.DrawUI();
            else
                World.DrawUI();
        }

        public void Quit()
        {

        }
    }
}
