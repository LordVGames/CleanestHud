using CleanestHud.ModSupport;
using HG;
using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;


[MonoDetourTargets]
internal static class SprintAndInventoryReminders
{
    public const float SprintClusterDefaultX = -260;
    internal static HudEditorComponents.TransformLocalPositionSetter SprintClusterMover;


    public const float InventoryClusterSpaceFromSprintCluster = 50;
    public const float InventoryClusterDefaultX = SprintClusterDefaultX - InventoryClusterSpaceFromSprintCluster;
    internal static HudEditorComponents.TransformLocalPositionSetter InventoryClusterMover;




    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudStructure.OnHudStructureEditsBegun += HudStructureEdits;
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
        ConfigOptions.OnShowSprintAndInventoryKeybindsChanged += SetSprintAndInventoryKeybindsStatus;
        Mdh.RoR2.UI.HUD.OnDestroy.Postfix(OnDestroy);
    }




    private static void HudStructureEdits()
    {
        SprintCluster.SetParent(BarRoots);
        InventoryCluster.SetParent(BarRoots);


        SprintClusterMover = SprintCluster.EnsureComponent<HudEditorComponents.TransformLocalPositionSetter>();
        InventoryClusterMover = InventoryCluster.EnsureComponent<HudEditorComponents.TransformLocalPositionSetter>();


        // can't edit the info on the components right away because of reasons
        MyHud?.StartCoroutine(SetStartingPosition());
    }


    private static IEnumerator SetStartingPosition()
    {
        yield return null;


        RectTransform healthbarRootRect = HealthBarRoot.GetComponent<RectTransform>();
        float yPos = healthbarRootRect.sizeDelta.y * 1.5f;


        SprintClusterMover.NewLocalPosition = new Vector3(SprintClusterDefaultX, yPos);
        InventoryClusterMover.NewLocalPosition = new Vector3(InventoryClusterDefaultX, yPos);


        SprintCluster.localRotation = Quaternion.identity;
        InventoryCluster.localRotation = Quaternion.identity;


        yield break;
    }




    private static void OnDestroy(HUD self)
    {
        SprintClusterMover = null;
        InventoryClusterMover = null;
    }




    internal static void HudDetailEdits()
    {
        RemoveSprintAndInventoryReminderTextBackgrounds();
        SetSprintAndInventoryKeybindsStatus();
    }


    internal static void SetSprintAndInventoryKeybindsStatus()
    {
        SprintCluster.gameObject.SetActive(ConfigOptions.ShowSprintAndInventoryKeybinds.Value);
        InventoryCluster.gameObject.SetActive(ConfigOptions.ShowSprintAndInventoryKeybinds.Value);
    }


    private static void RemoveSprintAndInventoryReminderTextBackgrounds()
    {
        SprintCluster.Find("KeyBackgroundPanel")?.DisableImageComponent();
        // why is the background panel name different from the SprintCluster? idfk
        InventoryCluster.Find("SkillBackgroundPanel")?.DisableImageComponent();
    }
}