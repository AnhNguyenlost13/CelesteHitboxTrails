using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.HitboxTrail
{
    public class HitboxTrailModule : EverestModule
    {
        private static HitboxTrailModule Instance { get; set; }

        public override Type SettingsType => typeof(HitboxTrailSettings);

        private static HitboxTrailSettings Settings =>
            (HitboxTrailSettings)Instance._Settings;

        private HitboxTrailRenderer _trailRenderer;

        public override void Load()
        {
            Instance = this;
            _trailRenderer = new HitboxTrailRenderer(Settings);

            // Core hooks
            On.Celeste.Player.Update += Player_Update;
            On.Celeste.GameplayRenderer.Render += GameplayRenderer_Render;

            // Clear trail hooks
            On.Celeste.Player.Die += Player_Die;
            On.Celeste.Level.LoadLevel += Level_LoadLevel;
            On.Celeste.Player.Added += Player_Added;

            Logger.Log(LogLevel.Info, "Hitbox Trails", "Loaded!");
        }

        public override void Unload()
        {
            // Core hooks
            On.Celeste.Player.Update -= Player_Update;
            On.Celeste.GameplayRenderer.Render -= GameplayRenderer_Render;

            // Clear trail hooks
            On.Celeste.Player.Die -= Player_Die;
            On.Celeste.Level.LoadLevel -= Level_LoadLevel;
            On.Celeste.Player.Added -= Player_Added;

            _trailRenderer = null;
            Instance = null;

            Logger.Log(LogLevel.Info, "Hitbox Trails", "Unloaded!");
        }

        private static void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            if (Settings.DisplayMode == ShowMode.Never) return;
            Instance._trailRenderer?.RecordSnapshot(self);
        }

        private static void GameplayRenderer_Render(On.Celeste.GameplayRenderer.orig_Render orig,
            GameplayRenderer self, Scene scene)
        {
            // Call the original
            orig(self, scene);

            var shouldRenderTrail = Settings.DisplayMode switch
            {
                ShowMode.Always => true,
                ShowMode.DebugOnly => Engine.Commands?.Open ?? false,
                ShowMode.Never => false,
                _ => false
            };

            if (!shouldRenderTrail && !Settings.ForceHitboxDisplay) return;

            GameplayRenderer.Begin();

            if (shouldRenderTrail) Instance._trailRenderer?.Render();

            // show hitboxes forcer 
            if (Settings.ForceHitboxDisplay && Engine.Commands != null && !Engine.Commands.Open) scene.Entities.DebugRender(self.Camera);

            GameplayRenderer.End();
        }

        private static PlayerDeadBody Player_Die(On.Celeste.Player.orig_Die orig,
            Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
        {
            if (Settings.ClearOnDeath)
            {
                Instance._trailRenderer?.Clear();
            }

            return orig(self, direction, evenIfInvincible, registerDeathInStats);
        }

        private static void Level_LoadLevel(On.Celeste.Level.orig_LoadLevel orig,
            Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            if (Settings.ClearOnTransition && !isFromLoader)
            {
                Instance._trailRenderer?.Clear();
            }

            orig(self, playerIntro, isFromLoader);
        }

        private static void Player_Added(On.Celeste.Player.orig_Added orig,
            Player self, Scene scene)
        {
            if (Settings.ClearOnRespawn)
            {
                Instance._trailRenderer?.Clear();
            }

            orig(self, scene);
        }
    }
}
