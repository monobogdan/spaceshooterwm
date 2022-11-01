using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace WMGame3D
{
    

    public sealed class Engine
    {

        private static Engine _current;
        public static Engine Current
        {
            get { return _current; }
            set { _current = value; }
        }

        public Graphics Graphics;
        public SoundManager Sound;
        public Input Input;
        internal FontManager FontManager;

        public Game Game;

        public float DeltaTime;
        public Window Window;

        private System.IO.StreamWriter logWriter;

        public static void Init(int viewWidth, int viewHeight)
        {
            Current = new Engine(viewWidth, viewHeight);
            Current.PostInitialize();
        }

        private Engine(int viewWidth, int viewHeight)
        {
            //logWriter = new StreamWriter("./log.txt");
            
            Window = new Window(viewWidth, viewHeight);
        }

        private void PostInitialize()
        {
            Graphics = new Graphics();
            Graphics.LoadResources();
            Sound = new SoundManager();
            Input = new Input();
            FontManager = new FontManager();

            Game = new Game();
        }

        public void DrawFrame()
        {
            int ticks = Environment.TickCount;
            Window.Update();

            Game.Update();
            Graphics.Begin();
            Game.Draw();
            Graphics.BeginUI();
            FontManager.Begin();
            Game.DrawUI();
            FontManager.End();
            Graphics.End();

            DeltaTime = (float)(Environment.TickCount - ticks) / 1000.0f;
        }

        public void Run()
        {
            while (Window.IsValid)
            {
                DrawFrame();
            }

            Game.Quit();
        }

        public Stream OpenAsset(string fileName)
        {
            Stream strm = Window.OpenAsset(fileName);

            if(strm == null)
                Log("OpenAsset {0} failed", fileName);

            return strm;
        }

        public void Log(string fmt, params object[] args)
        {
            string formatted = string.Format(fmt, args);

            System.Diagnostics.Debug.WriteLine(formatted);
            //Console.WriteLine(formatted);
            //logWriter.WriteLine(formatted);
            //logWriter.Flush();
        }
    }
}
