using CleanestHud.HudChanges.NormalHud.HealthBarArea;
using CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;
using CleanestHud.ModSupport;
using System;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.SurvivorSpecific;


internal static class SurvivorSpecific
{
    /// <summary>
    /// Happens on camera target change BEFORE HUD color Edits.
    /// </summary>
    public static event Action OnSurvivorSpecificHudChangesBegun;


    /// <summary>
    /// Happens after OnSurvivorSpecificHudChangesBegun.
    /// </summary>
    public static event Action OnSurvivorSpecificHudChangesFinished;


    internal static void FireSurvivorSpecificUIEvent()
    {
        if (IsHudEditable)
        {
            // reset any possible Edits made by other characters first
            SkillsScaler.ResetSkillsScalerLocalPositionToDefault();
            BarRoots.ResetBarRootsLocalPositionToDefault();


            // if a survivor specific edit is needed, it's called by this
            OnSurvivorSpecificHudChangesBegun?.Invoke();
            OnSurvivorSpecificHudChangesFinished?.Invoke();
        }
        else
        {
            Log.Debug("Cannot do survivor-specific HUD Edits, the HUD is not editable!");
        }
    }
}