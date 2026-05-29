namespace LOGIYGames.Shared.Enums
{
    public enum AnimationEventType
    {
        None,

        // ========================================================
        // ABILITY
        // ========================================================

        AbilityStarted,
        AbilityActionStart,
        AbilityActionEnd,
        AbilityFinished,

        // ========================================================
        // HITBOX
        // ========================================================

        EnableHitbox,
        DisableHitbox,

        // ========================================================
        // COMBO
        // ========================================================

        OpenComboWindow,
        CloseComboWindow,

        // ========================================================
        // CANCEL
        // ========================================================

        OpenCancelWindow,
        CloseCancelWindow,

        // ========================================================
        // GENERAL
        // ========================================================
        AttackStarted,
        AttackFinished
    }
}
