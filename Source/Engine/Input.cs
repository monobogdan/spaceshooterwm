using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public enum Key
    {
        Left,
        Right,
        Up,
        Down,
        Fire1,
        Fire2,
        LT,
        RT
    }

    public struct TouchState
    {
        public bool IsTouching;
        public float X;
        public float Y;
    }

    public sealed class Input
    {
        private bool[] keys;
        public TouchState Touch;

        internal Input()
        {
            keys = new bool[(int)Key.RT + 1];
            Touch = new TouchState();
        }

        public bool IsPressed(Key key)
        {
            return keys[(int)key];
        }

        public void SetState(Key key, bool state)
        {
            keys[(int)key] = state;
        }
    }
}
