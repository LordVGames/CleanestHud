using CleanestHud.ModSupport;
using MiscFixes.Modules;
using RoR2.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using static CleanestHud.Main;
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
    /// The first phase of editing the HUD BEFORE this mod's HUD Edits are done. This is for Edits on where a HUD element is or how big it is.
    /// </summary>
    public static event Action OnHudStructureEditsBegun;


    /// <summary>
    /// The first phase of editing the HUD AFTER this mod's HUD Edits are done. This is for Edits on where a HUD element is or how big it is.
    /// </summary>
    public static event Action OnHudStructureEditsFinished;


    internal static void BeginEdits()
    {
        if (!IsHudEditable)
        {
            return;
        }
        if (!ConfigOptions.AllowHudStructureEdits.Value)
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

        NormalHud.SkillsAndEquipsArea.SkillsScaler.ResetSkillsScalerLocalPositionToDefault();
        NormalHud.SkillsAndEquipsArea.SkillsScaler.RepositionSkillScaler();
        NormalHud.HealthBarArea.BarRoots.ResetBarRootsLocalPositionToDefault();

        Transform itemInventoryDisplayRoot = MyHud.itemInventoryDisplay.transform.parent;
        RectTransform itemInventoryDisplayRootRect = itemInventoryDisplayRoot.GetComponent<RectTransform>();
        itemInventoryDisplayRootRect.localPosition = new Vector3(
            // 0 is always at the center of the whole screen, and the inventory's left edge is placed at 0
            // we remove half the size delta to move it over left by half it's size to get it perfectly centered no matter the game width
            0 - (itemInventoryDisplayRootRect.sizeDelta.x / 2),
            itemInventoryDisplayRootRect.localPosition.y,
            itemInventoryDisplayRootRect.localPosition.z
        );


        RectTransform healthbarRootRect = HealthBarRoot.GetComponent<RectTransform>();
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

        //NormalHud.SkillsAndEquipsArea.SprintAndInventoryReminders.RepositionSprintAndInventoryReminders();
    }
}