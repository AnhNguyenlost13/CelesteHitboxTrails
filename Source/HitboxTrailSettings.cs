namespace Celeste.Mod.HitboxTrail
{
    public class HitboxTrailSettings : EverestModuleSettings
    {
        /// <summary>
        /// Number of previous hitbox positions to display.
        /// Setting to 0 (not recommended) makes the trail length infinite.
        /// Higher = longer trail, more performance impact.
        /// </summary>
        [SettingName("Trail Length")]
        [SettingRange(0, 120)]
        public int TrailLength { get; set; } = 60;

        /// <summary>
        /// Minimum distance (in pixels) the player must move before
        /// recording a new trail snapshot. Lower = denser trail.
        /// </summary>
        [SettingName("Min Trail Spacing")]
        [SettingRange(0, 16)]
        public int MinTrailSpacing { get; set; } = 4;

        /// <summary>
        /// Whether to show the trail only when debug view is active,
        /// or always show it during gameplay.
        /// </summary>
        [SettingName("Show Mode")]
        public ShowMode DisplayMode { get; set; } = ShowMode.DebugOnly;

        /// <summary>
        /// Forces the player hitbox and hurtbox to always be visible,
        /// even when the debug console is closed.
        /// Recommended to use with Show Mode = Always.
        /// </summary>
        [SettingName("Force Hitbox Display")]
        public bool ForceHitboxDisplay { get; set; } = false;

        /// <summary>
        /// Whether to also display the hurtbox trail (lime green).
        /// </summary>
        [SettingName("Show Hurtbox Trail")]
        public bool ShowHurtboxTrail { get; set; } = true;

        /// <summary>
        /// Whether to color trail segments by player state.
        /// </summary>
        [SettingName("Color By State")]
        public bool ColorByState { get; set; } = true;

        /// <summary>
        /// Whether to use filled rectangles instead of hollow for the trail.
        /// </summary>
        [SettingName("Filled Trail")]
        public bool FilledRectangles { get; set; } = false;

        /// <summary>
        /// Clears the trail on player death.
        /// </summary>
        [SettingName("Clear On Death")]
        public bool ClearOnDeath { get; set; } = true;

        /// <summary>
        /// Clears the trail on room transition.
        /// </summary>
        [SettingName("Clear On Transition")]
        public bool ClearOnTransition { get; set; } = true;

        /// <summary>
        /// Clears the trail on respawn.
        /// </summary>
        [SettingName("Clear On Respawn")]
        public bool ClearOnRespawn { get; set; } = true;
    }

    public enum ShowMode
    {
        DebugOnly,   // Only show when console is open
        Always,      // Always show during gameplay
        Never        // Disable trail completely
    }
}
