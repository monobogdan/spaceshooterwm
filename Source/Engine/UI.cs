using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    [Flags]
    public enum ItemFlags
    {
        None = 0,
        HandleBackKey = 1
    }

    public sealed class ListMenu
    {
        private const float ActionInterval = 0.3f;
        public static Vector4 Normal = new Vector4(255, 0, 0, 255);
        public static Vector4 Active = new Vector4(128, 255, 0, 255);
        private static int MarginY = 5;

        private static float nextAction;
        public delegate void OnAction();

        private class Item
        {
            public string Text;
            public OnAction Event;
        }

        public Vector3 Position;

        private List<Item> items;
        private Item backItem;
        private int currentItem;
        private NativeFont renderer;

        public ListMenu(Vector3 position)
        {
            items = new List<Item>();
            nextAction = ActionInterval;

            Position = position;
            float bestFontSize = Engine.Current.Window.Info.IsMobile ? 25 : 10.0f;
            renderer = new NativeFont("Arial", GUI.GetAbsFontSize(bestFontSize));
        }

        public ListMenu AddItem(string text, OnAction ev, ItemFlags flags = ItemFlags.None)
        {
            Item item = new Item();
            item.Text = text;
            item.Event = ev;
            items.Add(item);

            if (((int)flags & (int)ItemFlags.HandleBackKey) == 1)
                backItem = item;

            return this;
        }

        private int TouchTest()
        {
            if (!Engine.Current.Input.Touch.IsTouching)
                return -1;

            for (int i = 0; i < items.Count; i++)
            {
                TouchState touch = Engine.Current.Input.Touch;

                Engine.Current.Log("{0} {1} {2}", touch.X, touch.Y, renderer.MeasureString(items[i].Text));

                if (touch.X > Position.X && touch.Y > Position.Y + (i * (renderer.Size + MarginY))
                    && touch.X < (Position.X + renderer.MeasureString(items[i].Text)) && touch.Y < Position.Y + ((i * renderer.Size + MarginY) + renderer.Size))
                    return i;
            }

            return -1;
        }

        public void Update()
        {
            if (Engine.Current.Input.IsPressed(Key.Up) && nextAction < 0 && currentItem > 0)
            {
                currentItem--;
                nextAction = ActionInterval;
            }

            if (Engine.Current.Input.IsPressed(Key.Down) && nextAction < 0 && currentItem < items.Count - 1)
            {
                currentItem++;
                nextAction = ActionInterval;
            }

            if ((Engine.Current.Input.IsPressed(Key.Fire1)) && nextAction < 0 && items[currentItem].Event != null)
            {
                items[currentItem].Event();
                nextAction = ActionInterval;
            }

            if ((Engine.Current.Input.IsPressed(Key.RT)) && nextAction < 0 && backItem != null)
            {
                backItem.Event();
                nextAction = ActionInterval;
            }

            int idx = TouchTest();

            if (idx != -1 && nextAction < 0 && items[idx].Event != null)
            {
                items[idx].Event();
                nextAction = ActionInterval;
            }

            nextAction -= Engine.Current.DeltaTime;
        }

        public void Draw()
        {

            for (int i = 0; i < items.Count; i++)
            {
                Vector4 color = (currentItem != i) ? Normal : Active;
                renderer.DrawString(items[i].Text, (int)Position.X, (int)(Position.Y + i * (renderer.Size + MarginY)), 
                    (int)color.X, (int)color.Y, (int)color.Z, (int)color.W);
            }
        }
    }

    public sealed class GUI
    {
        private const float MaxFontSize = 50;

        public static void DrawTextRelative(NativeFont renderer, string text, Vector4 rgb, float relX, float relY)
        {
            Rectf abs = GUI.RelativeToAbs(new Rectf(relX, relY, 0, 0));

            renderer.DrawString(text, (int)abs.X, (int)abs.Y, (int)rgb.X, (int)rgb.Y, (int)rgb.Z, (int)rgb.W);
        }

        // rel is relative font unit according to 640x480 resolution, but clamped to some maximum. For example, on 240x320 this will give us font size 6
        public static float GetAbsFontSize(float rel)
        {
            float ratio = (float)Engine.Current.Window.ViewportWidth / 640;

            return Mathf.Clamp(rel * ratio, 8, MaxFontSize);
        }

        public static Rectf RelativeToAbs(Rectf rel)
        {
            Vector3 pos = new Vector3(rel.X * Engine.Current.Window.ViewportWidth, rel.Y * Engine.Current.Window.ViewportHeight, 0);
            Vector3 size = new Vector3(rel.W * Engine.Current.Window.ViewportWidth, rel.H * Engine.Current.Window.ViewportHeight, 1);

            return new Rectf(pos.X, pos.Y, size.X, size.Y);
        }
    }
}
