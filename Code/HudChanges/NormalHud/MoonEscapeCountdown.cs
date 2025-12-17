using System;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets(typeof(EscapeSequenceController))]
internal static class MoonEscapeCountdown
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.EscapeSequenceController.SetHudCountdownEnabled.Postfix(SetHudCountdownEnabled);
    }

    private static void SetHudCountdownEnabled(EscapeSequenceController self, ref HUD hud, ref bool shouldEnableCountdownPanel)
    {
        if (!IsHudEditable || TopCenterCluster == null)
        {
            return;
        }

        HudDetails.EditMoonDetonationPanel();
    }
}