using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace WMGame3D
{
    public struct KeyBinding
    {
        public Keys Primary;
        public Keys Secondary;

        public KeyBinding(Keys p, Keys s)
        {
            Primary = p;
            Secondary = s;
        }
    }

    public struct PlatformInfo
    {
        public string Name;
        public bool IsMobile;
        public string AppPath;
        public string DataPath;
    }

    public sealed class Window
    {
        public const string SearchPath = "data/";

        private Form form;
        public bool IsValid;
        public IntPtr Handle;
        public int ViewportWidth;
        public int ViewportHeight;

        public PlatformInfo Info;

        private KeyBinding[] bindings;

        private void SetupBindings()
        {
            bindings = new KeyBinding[(int)Key.RT + 1];
            bindings[(int)Key.Left] = new KeyBinding(Keys.Left, Keys.A);
            bindings[(int)Key.Up] = new KeyBinding(Keys.Up, Keys.W);
            bindings[(int)Key.Right] = new KeyBinding(Keys.Right, Keys.D);
            bindings[(int)Key.Down] = new KeyBinding(Keys.Down, Keys.S);
            bindings[(int)Key.Fire1] = new KeyBinding(Keys.Return, Keys.None);
            bindings[(int)Key.Fire2] = new KeyBinding(Keys.Space, Keys.None);
            bindings[(int)Key.LT] = new KeyBinding(Keys.LMenu, Keys.None);
            bindings[(int)Key.RT] = new KeyBinding(Keys.RMenu, Keys.Escape);
        }

        public Window(int desiredWidth, int desiredHeight)
        {
            form = new Form();
            form.Text = "WMGame3D";
#if WIN32
            form.Size = new System.Drawing.Size(800, 600);
#endif
            form.Show();

            SetupBindings();

            IsValid = true;
            form.HandleDestroyed += new EventHandler(OnDestroyed);
            form.KeyUp += new KeyEventHandler(KeyUp);
            form.KeyDown += new KeyEventHandler(KeyDown);
            form.MouseMove += new MouseEventHandler(MouseMove);
            form.MouseUp += new MouseEventHandler(MouseUp);
            form.MouseDown += new MouseEventHandler(MouseMove);

            Info = new PlatformInfo();
            Info.Name = Environment.OSVersion.Platform.ToString();
            Info.AppPath = "./profile/";
            Info.DataPath = "./";
#if WIN32
            Info.IsMobile = false;
#else
            Info.IsMobile = true;
#endif

            Handle = form.Handle;

            ViewportWidth = form.ClientRectangle.Width;
            ViewportHeight = form.ClientRectangle.Height;
        }

        void MouseUp(object sender, MouseEventArgs e)
        {
            Engine.Current.Input.Touch.IsTouching = false;
        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            Engine.Current.Input.Touch.IsTouching = e.Button == MouseButtons.Left;
            Engine.Current.Input.Touch.X = (float)e.X;
            Engine.Current.Input.Touch.Y = (float)e.Y;
        }

        void KeyUp(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < bindings.Length; i++)
            {
                if (bindings[i].Primary == e.KeyCode || bindings[i].Secondary == e.KeyCode)
                    Engine.Current.Input.SetState((Key)i, false);
            }
        }

        void KeyDown(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < bindings.Length; i++)
            {
                if (bindings[i].Primary == e.KeyCode || bindings[i].Secondary == e.KeyCode)
                    Engine.Current.Input.SetState((Key)i, true);
            }
        }

        void OnDestroyed(object sender, EventArgs e)
        {
            IsValid = false;
        }

        public Stream OpenAsset(string fileName)
        {
            if (File.Exists(SearchPath + fileName))
                return File.OpenRead(SearchPath + fileName);

            return null;
        }

        public void Update()
        {
            Application.DoEvents();

#if WindowsCE
            IsValid = form.Focused;
#endif
        }
    }
}
