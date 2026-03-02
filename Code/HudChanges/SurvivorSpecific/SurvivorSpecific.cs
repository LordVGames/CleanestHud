using CleanestHud.HudChanges.NormalHud.HealthBarArea;
using CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;
using CleanestHud.ModSupport;
using System;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.SurvivorSpecific;


internal static class SurvivorSpecific
{
    /// <summary>
    /// Happens on camera target change BEFORE HUD color changes.
    /// </summary>
    public static event Action OnSurvivorSpecificHudEditsBegun;


    /// <summary>
    /// Happens after OnSurvivorSpecificHudEditsBegun.
    /// </summary>
    public static event Action OnSurvivorSpecificHudEditsFinished;


    internal static void AddEventSubscriptions()
    {
        OnSurvivorSpecificHudEditsBegun += VoidFiend.SetupViendEdits;
        OnSurvivorSpecificHudEditsBegun += Seeker.RepositionSeekerLotusUI;
        ConfigOptions.OnShowSkillKeybindsChanged += Seeker.RepositionSeekerLotusUI;
    }


    internal static void FireSurvivorSpecificUIEvent()
    {
        if (IsHudEditable)
        {
            // reset any possible changes made by other characters first
            SkillsScaler.ResetSkillsScalerLocalPositionToDefault();
            BarRoots.ResetBarRootsLocalPositionToDefault();
            SprintAndInventoryReminders.ResetInventoryClusterLocalPositionToDefault();
            SprintAndInventoryReminders.ResetSprintClusterLocalPositionToDefault();


            // if a survivor specific edit is needed, it's called by this
            OnSurvivorSpecificHudEditsBegun?.Invoke();
            OnSurvivorSpecificHudEditsFinished?.Invoke();
        }
        else
        {
            Log.Debug("Cannot do survivor-specific HUD edits, the HUD is not editable!");
        }
    }
}