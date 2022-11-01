using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public abstract class Animation
    {
        public Vector3 Position;

        public virtual void Update(float time)
        {

        }
    }

    public sealed class UISwipeIn : Animation
    {
        private Vector3 from, to;

        public UISwipeIn(Vector3 from, Vector3 to)
        {
            this.from = from;
            this.to = to;
        }

        public override void Update(float time)
        {
            base.Update(time);

            Position.X = Mathf.Lerp(from.X, to.X, time);
            Position.Y = Mathf.Lerp(from.Y, to.Y, time);
            Position.Z = Mathf.Lerp(from.Z, to.Z, time);
        }
    }

    public sealed class SmoothAnimator
    {
        public float Time;
        public Animation Animation;
        public bool IsPlaying;
        public float Speed;

        private float sign;

        public SmoothAnimator(Animation animation)
        {
            Speed = 1.0f;
            Animation = animation;
        }

        public void Play(bool inverse)
        {
            IsPlaying = true;
            Time = 0;
            sign = inverse ? -1 : 1;

            if (inverse)
                Time = 1;
        }

        public void Update()
        {
            if (IsPlaying)
            {
                Time = Mathf.Clamp(Time + ((Engine.Current.DeltaTime * Speed) * sign), 0, 1);

                if (Animation != null)
                    Animation.Update(Time);
            }
        }
    }
}
