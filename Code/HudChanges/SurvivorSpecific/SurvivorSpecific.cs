using System;
using static CleanestHud.Main;
using CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;
namespace CleanestHud.HudChanges.SurvivorSpecific;


internal static class SurvivorSpecific
{
    /// <summary>
    /// Happens after HUD structure edits, HUD detail edits, and survivor-specific HUD elements have all been edited, but not HUD coloring.
    /// </summary>
    /// <remarks>
    /// Can happen multiple times after the HUD is created.
    /// </remarks>
    public static event Action OnSurvivorSpecificHudEditsFinished;


    internal static void AddEventSubscriptions()
    {
        OnSurvivorSpecificHudEditsFinished += VoidFiend.SetupViendEdits;
        OnSurvivorSpecificHudEditsFinished += Seeker.RepositionSeekerLotusUI;
        ConfigOptions.OnShowSkillKeybindsChanged += Seeker.RepositionSeekerLotusUI;
    }


    internal static void FireSurvivorSpecificUIEvent()
    {
        if (IsHudEditable)
        {
            // if a survivor specific edit is needed, it's called by this
            OnSurvivorSpecificHudEditsFinished?.Invoke();
            //HudStructure.RepositionAllHudElements();
        }
        else
        {
            Log.Debug("Cannot do survivor-specific HUD edits, the HUD is not editable!");
        }
    }
}