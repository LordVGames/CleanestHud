using System;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MiscFixes.Modules;
using CleanestHud.ModSupport;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;
namespace CleanestHud.HudChanges;

public static class HudStructure
{
    private static bool IsHudEditable
    {
        get
        {
            return Main.IsHudEditable && ConfigOptions.AllowHudStructureEdits.Value;
        }
    }


    /// <summary>
    /// The first phase of editing the HUD BEFORE this mod's HUD edits are done. This is for changes on where a HUD element is or how big it is.
    /// </summary>
    public static event Action OnHudStructureEditsBegun;


    /// <summary>
    /// The first phase of editing the HUD AFTER this mod's HUD edits are done. This is for changes on where a HUD element is or how big it is.
    /// </summary>
    public static event Action OnHudStructureEditsFinished;


    internal static void AddEventSubscriptions()
    {
        OnHudStructureEditsBegun += NormalHud.DifficultyHud.DifficultyBarSegments.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.HealthBarArea.BarRoots.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.HealthBarArea.HealthBar.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.HealthBarArea.BuffDisplay.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.HealthBarArea.LevelDisplay.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.HealthBarArea.XpBar.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.SpectatorLabel.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.SkillsAndEquipsArea.SkillSlots.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.SkillsAndEquipsArea.EquipmentSlots.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.CurrenciesArea.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.ItemInventoryDisplayArea.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.BossHpBar.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.NotificationArea.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.ScoreboardPanel.ScoreboardPanel.HudStructureEdits;
        OnHudStructureEditsBegun += NormalHud.SkillsAndEquipsArea.SkillsScaler.HudStructureEdits;
        OnHudStructureEditsBegun += Simulacrum.DefaultWaveUI.HudStructureEdits;
    }


    internal static void BeginEdits()
    {
        if (!IsHudEditable)
        {
            return;
        }
        
        OnHudStructureEditsBegun?.Invoke();
        OnHudStructureEditsFinished?.Invoke();
        RepositionAllHudElements();
    }
    

    internal static void RepositionAllHudElements()
    {
        if (!IsHudEditable)
        {
            return;
        }

        SkillsScaler.ResetSkillsScalerLocalPositionToDefault();
        SprintAndInventoryReminders.ResetInventoryClusterLocalPositionToDefault();
        SprintAndInventoryReminders.ResetSprintClusterLocalPositionToDefault();
        SkillsScaler.RepositionSkillScaler();
        SprintAndInventoryReminders.RepositionSprintAndInventoryReminders();

        Transform itemInventoryDisplayRoot = MyHud.itemInventoryDisplay.transform.parent;
        RectTransform itemInventoryDisplayRootRect = itemInventoryDisplayRoot.GetComponent<RectTransform>();
        itemInventoryDisplayRootRect.localPosition = new Vector3(
            // 0 is always at the center of the whole screen, and the inventory's left edge is placed at 0
            // we remove half the size delta to move it over left by half it's size to get it perfectly centered no matter the game width
            0 - (itemInventoryDisplayRootRect.sizeDelta.x / 2),
            itemInventoryDisplayRootRect.localPosition.y,
            itemInventoryDisplayRootRect.localPosition.z
        );

        Transform healthbarRoot = ImportantHudTransforms.BarRoots.GetChild(1);
        RectTransform healthbarRootRect = healthbarRoot.GetComponent<RectTransform>();
        healthbarRootRect.localPosition = new Vector3(
            0 - (healthbarRootRect.sizeDelta.x / 2),
            45,
            healthbarRootRect.localPosition.z
        );



        Transform levelDisplayCluster = ImportantHudTransforms.BarRoots.GetChild(0);

        Transform expBarRoot = levelDisplayCluster.GetChild(1);
        RectTransform expBarRootRect = expBarRoot.GetComponent<RectTransform>();
        // gotta do this one in 2 parts
        expBarRootRect.position = new Vector3(
            0,
            expBarRootRect.position.y,
            expBarRootRect.position.z
        );
        expBarRootRect.localPosition = new Vector3(
            expBarRootRect.localPosition.x,
            -4.2f,
            0
        );

        NormalHud.SkillsAndEquipsArea.SprintAndInventoryReminders.RepositionSprintAndInventoryReminders();
    }
}