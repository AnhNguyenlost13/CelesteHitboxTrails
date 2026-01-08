using Microsoft.Xna.Framework;

namespace Celeste.Mod.HitboxTrail
{
    public struct HitboxSnapshot
    {
        public Vector2 Position;

        public float Width;
        public float Height;

        public Vector2 ColliderOffset;

        public int State;

        public bool Ducking;

        public float TimeStamp;

        public float HurtWidth;
        public float HurtHeight;
        public Vector2 HurtOffset;

        public Vector2 AbsolutePosition => Position + ColliderOffset;

        public Vector2 AbsoluteHurtPosition => Position + HurtOffset;
    }
}