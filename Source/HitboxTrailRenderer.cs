using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.HitboxTrail
{
    public class HitboxTrailRenderer(HitboxTrailSettings settings)
    {
        private readonly Queue<HitboxSnapshot> _trailHistory = new Queue<HitboxSnapshot>();
        private Vector2? _lastRecordedPosition = null;

        /// <summary>
        /// Records the current player hitbox state.
        /// Should only run when displacement is large enough.
        /// </summary>
        public void RecordSnapshot(Player player)
        {
            if (player?.Collider == null) return;

            if (_lastRecordedPosition.HasValue && settings.MinTrailSpacing > 0)
            {
                var distance = Vector2.Distance(player.Position, _lastRecordedPosition.Value);
                if (distance < settings.MinTrailSpacing) return;
            }

            // Get hurtbox via reflection (it's private)
            var hurtboxField = typeof(Player).GetField("hurtbox",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            var hurtbox = hurtboxField?.GetValue(player) as Hitbox;

            var snapshot = new HitboxSnapshot
            {
                Position = player.Position,
                Width = player.Collider.Width,
                Height = player.Collider.Height,
                ColliderOffset = player.Collider.Position,
                State = player.StateMachine?.State ?? 0,
                Ducking = player.Ducking,
                TimeStamp = Engine.Scene?.TimeActive ?? 0f,
                HurtWidth = hurtbox?.Width ?? 0f,
                HurtHeight = hurtbox?.Height ?? 0f,
                HurtOffset = hurtbox?.Position ?? Vector2.Zero
            };

            _trailHistory.Enqueue(snapshot);
            _lastRecordedPosition = player.Position;

            // Trim to max length. 0 ignores trimming.
            if (settings.TrailLength <= 0) return;
            while (_trailHistory.Count > settings.TrailLength) _trailHistory.Dequeue();
        }

        public void Clear()
        {
            _trailHistory.Clear();
            _lastRecordedPosition = null;
        }

        public void Render()
        {
            if (_trailHistory.Count == 0) return;

            foreach (var snapshot in _trailHistory)
            {
                var hitboxColor = GetHitboxColor(snapshot.State);
                var hurtboxColor = Color.Lime;

                var hitX = snapshot.Position.X + snapshot.ColliderOffset.X;
                var hitY = snapshot.Position.Y + snapshot.ColliderOffset.Y;

                if (settings.FilledRectangles) Draw.Rect(hitX, hitY, snapshot.Width, snapshot.Height, hitboxColor);
                else Draw.HollowRect(hitX, hitY, snapshot.Width, snapshot.Height, hitboxColor);

                if (!settings.ShowHurtboxTrail || !(snapshot.HurtWidth > 0)) continue;
                var hurtX = snapshot.Position.X + snapshot.HurtOffset.X;
                var hurtY = snapshot.Position.Y + snapshot.HurtOffset.Y;

                if (settings.FilledRectangles) Draw.Rect(hurtX, hurtY, snapshot.HurtWidth, snapshot.HurtHeight, hurtboxColor);
                else Draw.HollowRect(hurtX, hurtY, snapshot.HurtWidth, snapshot.HurtHeight, hurtboxColor);
            }
        }

        private Color GetHitboxColor(int state)
        {
            if (settings.ColorByState)
            {
                return state switch
                {
                    0 => Color.Red,            // StNormal
                    1 => Color.Green,          // StClimb
                    2 => Color.Cyan,           // StDash
                    3 => Color.Blue,           // StSwim
                    4 => Color.LimeGreen,      // StBoost
                    5 => Color.OrangeRed,      // StRedDash
                    6 => Color.Orange,         // StHitSquash
                    7 => Color.Yellow,         // StLaunch
                    9 => Color.Purple,         // StDreamDash
                    10 => Color.Gold,          // StSummitLaunch
                    19 => Color.Yellow,        // StStarFly
                    21 => Color.HotPink,       // StCassetteFly
                    22 => Color.Magenta,       // StAttract
                    24 => Color.Turquoise,     // StFlingBird
                    _ => Color.Red
                };
            }
            return Color.Red;
        }
    }
}
