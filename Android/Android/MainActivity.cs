using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;

using Android.Opengl;
using Javax.Microedition.Khronos.Egl;
using Javax.Microedition.Khronos.Opengles;
using Android.Views;

namespace WMGame3D.Port
{
    [Activity(Label = "@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Theme = "@android:style/Theme.NoTitleBar.Fullscreen", MainLauncher = true)]
    public class MainActivity : Activity, GLSurfaceView.IRenderer
    {
        public static MainActivity Current
        {
            get;
            private set;
        }

        private Engine engine;
        private GLSurfaceView surfView;

        public void OnDrawFrame(IGL10 gl)
        {
            engine.DrawFrame();
        }

        public void OnSurfaceChanged(IGL10 gl, int width, int height)
        {

        }

        public void OnSurfaceCreated(IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
        {
            Engine.Init(surfView.Width, surfView.Height);

            engine = Engine.Current;
            engine.Window.GLSurface = surfView;
        }

        private bool ThrowCriticalError(string message)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Critical error");
            builder.SetMessage(message);
            builder.SetNegativeButton("OK", (object sender, Android.Content.DialogClickEventArgs args) => { Finish(); });
            builder.Show();

            return false;
        }

        private bool CheckStorageStatus()
        {
            string state = Android.OS.Environment.ExternalStorageState;
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            if (state != Android.OS.Environment.MediaMounted && state != Android.OS.Environment.MediaMountedReadOnly)
                return ThrowCriticalError("An external sdcard is required to play this game.");

            if (!System.IO.File.Exists(path + WMGame3D.Window.DataPath + ".data"))
                return ThrowCriticalError("Game assets not found. Please copy them from game archive to " + path + WMGame3D.Window.DataPath);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (CheckStorageStatus())
            {
                Current = this;
                surfView = new GLSurfaceView(ApplicationContext);
                surfView.SetRenderer(this);
                SetContentView(surfView);
            }
        }

        public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            engine.Window.KeyUp(keyCode, e.KeyCode);

            return base.OnKeyUp(keyCode, e);
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            engine.Window.KeyDown(null, e.KeyCode);

            return base.OnKeyDown(keyCode, e);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (engine != null)
            {
                if (e.Action == MotionEventActions.Move)
                    engine.Window.TouchMove(this, e.GetX(), e.GetY(), true);

                if (e.Action == MotionEventActions.Up)
                    engine.Window.TouchUp(null);
            }

            return base.OnTouchEvent(e);
        }
    }
}