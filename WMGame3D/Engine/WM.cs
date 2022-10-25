using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

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

    public sealed class Window
    {
        private Form form;
        public bool IsValid;
        public IntPtr Handle;
        public int ViewportWidth;
        public int ViewportHeight;

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
            bindings[(int)Key.RT] = new KeyBinding(Keys.RMenu, Keys.None);
        }

        public Window()
        {
            form = new Form();
            form.Text = "WMGame3D";
#if WIN32
            form.Size = new System.Drawing.Size(640, 480);
#endif
            form.Show();

            SetupBindings();

            IsValid = true;
            form.HandleDestroyed += new EventHandler(OnDestroyed);
            form.KeyUp += new KeyEventHandler(KeyUp);
            form.KeyDown += new KeyEventHandler(KeyDown);
            form.MouseMove += new MouseEventHandler(MouseMove);
            form.MouseUp += new MouseEventHandler(MouseMove);
            form.MouseDown += new MouseEventHandler(MouseMove);

            Handle = form.Handle;
            ViewportWidth = form.ClientRectangle.Width;
            ViewportHeight = form.ClientRectangle.Height;
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

        public void Update()
        {
            Application.DoEvents();


        }
    }
}
