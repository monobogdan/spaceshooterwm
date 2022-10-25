using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public sealed class Game
    {
        public World World;

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

            World = new World();
            World.StartGame();
        }

        public void Update()
        {
            World.Update();
        }

        public void Draw()
        {
            World.Draw();
        }

        public void DrawUI()
        {
            World.DrawUI();
        }

        public void Quit()
        {

        }
    }
}
