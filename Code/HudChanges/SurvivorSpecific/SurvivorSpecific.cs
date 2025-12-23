using System;
using static CleanestHud.Main;
using CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;
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
            // if a survivor specific edit is needed, it's called by this
            OnSurvivorSpecificHudEditsBegun?.Invoke();
            //HudStructure.RepositionAllHudElements();
            OnSurvivorSpecificHudEditsFinished?.Invoke();
        }
        else
        {
            Log.Debug("Cannot do survivor-specific HUD edits, the HUD is not editable!");
        }
    }
}