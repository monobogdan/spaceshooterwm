using System;
using System.Collections.Generic;
using System.Text;

using Android;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Opengl;

using System.IO;

namespace WMGame3D
{
    public struct KeyBinding
    {
        public Keycode Primary;
        public Keycode Secondary;

        public KeyBinding(Keycode p, Keycode s)
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
        public const string DataPath = "/GameData/WMGame3D/";

        private string storageSearchPath;

        public Activity Activity;
        public GLSurfaceView GLSurface;

        public bool IsValid;
        public IntPtr Handle;
        public int ViewportWidth;
        public int ViewportHeight;

        public PlatformInfo Info;

        private KeyBinding[] bindings;

        private void SetupBindings()
        {
            bindings = new KeyBinding[(int)Key.RT + 1];
            bindings[(int)Key.Left] = new KeyBinding(Keycode.DpadLeft, Keycode.A);
            bindings[(int)Key.Up] = new KeyBinding(Keycode.DpadUp, Keycode.W);
            bindings[(int)Key.Right] = new KeyBinding(Keycode.DpadRight, Keycode.D);
            bindings[(int)Key.Down] = new KeyBinding(Keycode.DpadDown, Keycode.S);
            bindings[(int)Key.Fire1] = new KeyBinding(Keycode.Enter, Keycode.Enter);
            bindings[(int)Key.Fire2] = new KeyBinding(Keycode.Space, Keycode.Space);
            bindings[(int)Key.LT] = new KeyBinding(Keycode.Q, Keycode.ShiftLeft);
            bindings[(int)Key.RT] = new KeyBinding(Keycode.E, Keycode.ShiftRight);
        }

        public Window(int desiredWidth, int desiredHeight)
        {
            Activity = Port.MainActivity.Current;

            SetupBindings();

            storageSearchPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            IsValid = true;

            Info = new PlatformInfo();
            Info.Name = System.Environment.OSVersion.Platform.ToString();
            Info.IsMobile = true;
            Info.AppPath = Activity.ApplicationContext.ApplicationInfo.DataDir;

            ViewportWidth = desiredWidth;
            ViewportHeight = desiredHeight;
        }

        public string GetAssetPath(string path)
        {
            return storageSearchPath + DataPath + path;
        }

        public void TouchUp(object sender)
        {
            Engine.Current.Input.Touch.IsTouching = false;
        }

        public void TouchMove(object sender, float x, float y, bool isPressing)
        {
            Engine.Current.Input.Touch.IsTouching = isPressing;
            Engine.Current.Input.Touch.X = x;
            Engine.Current.Input.Touch.Y = y;
        }

        public void KeyUp(object sender, Keycode e)
        {
            for (int i = 0; i < bindings.Length; i++)
            {
                if (bindings[i].Primary == e || bindings[i].Secondary == e)
                    Engine.Current.Input.SetState((Key)i, false);
            }
        }

        public void KeyDown(object sender, Keycode e)
        {
            for (int i = 0; i < bindings.Length; i++)
            {
                if (bindings[i].Primary == e || bindings[i].Secondary == e)
                    Engine.Current.Input.SetState((Key)i, true);
            }
        }

        void OnDestroyed(object sender, EventArgs e)
        {
            IsValid = false;
        }

        public void Update()
        {

        }
        
        public System.IO.Stream OpenAsset(string fileName)
        {
            if (File.Exists(storageSearchPath + DataPath + fileName))
                return File.OpenRead(storageSearchPath + DataPath + fileName);

            return null;
        }
    }
}
