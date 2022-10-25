using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace WMGame3D
{
    

    public sealed class Engine
    {
#if WIN32
        public const string SearchPath = "./data/";
#else
        public const string SearchPath = "/data/";
#endif

        private static Engine _current;
        public static Engine Current
        {
            get { return _current; }
            set { _current = value; }
        }

        public Graphics Graphics;
        public SoundManager Sound;
        public Input Input;

        public Game Game;

        public float DeltaTime;
        public Window Window;

        private System.IO.StreamWriter logWriter;

        public static void Init()
        {
            Current = new Engine();
            Current.PostInitialize();
        }

        private Engine()
        {
            logWriter = new StreamWriter("./log.txt");
            
            Window = new Window();   
        }

        private void PostInitialize()
        {
            Graphics = new Graphics();
            Graphics.LoadResources();
            Sound = new SoundManager();
            Input = new Input();

            Game = new Game();
        }

        public void Run()
        {
            while (Window.IsValid)
            {
                int ticks = Environment.TickCount;
                Window.Update();

                Graphics.Begin();
                Game.Update();
                Game.Draw();
                Graphics.BeginHUD();
                Game.DrawUI();
                Graphics.End();

                DeltaTime = (float)(Environment.TickCount - ticks) / 1000.0f;
            }

            Game.Quit();
        }

        public Stream OpenAsset(string fileName)
        {
            if (File.Exists(SearchPath + fileName))
                return File.OpenRead(SearchPath + fileName);

            Log("OpenAsset {0} failed", fileName);
            return null;
        }

        public void Log(string fmt, params object[] args)
        {
            string formatted = string.Format(fmt, args);

            System.Diagnostics.Debug.WriteLine(formatted);
            Console.WriteLine(formatted);
            logWriter.WriteLine(formatted);
            logWriter.Flush();
        }
    }
}
