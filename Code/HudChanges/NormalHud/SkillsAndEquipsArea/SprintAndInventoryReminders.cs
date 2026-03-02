using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using CleanestHud.ModSupport;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;


internal static class SprintAndInventoryReminders
{
    private static Vector2 _sprintClusterLocalPositionBoosted = new(SprintClusterDefaultX, SprintClusterDefaultY);
    public static Vector2 SprintClusterLocalPositionBoosted
    {
        get
        {
            return _sprintClusterLocalPositionBoosted;
        }
    }
    private static Vector2 _sprintClusterLocalPosition = new(SprintClusterDefaultX, SprintClusterDefaultY);
    public static Vector2 SprintClusterLocalPosition
    {
        get
        {
            return _sprintClusterLocalPosition;
        }
        set
        {
            Transform sprintCluster = SkillDisplayRoot.Find("SprintCluster");
            sprintCluster.position = new Vector3(
                HealthBarRoot.position.x,
                sprintCluster.position.y,
                sprintCluster.position.z
            );
            _sprintClusterLocalPosition = value;
            _sprintClusterLocalPositionBoosted = _sprintClusterLocalPosition;
            _sprintClusterLocalPositionBoosted.y -= ConfigOptions.ShowSkillKeybinds.Value ? 42 : 0;
            sprintCluster.localPosition = _sprintClusterLocalPositionBoosted;
        }
    }
    public static void ResetSprintClusterLocalPositionToDefault()
    {
        SprintClusterLocalPosition = new Vector2(SprintClusterDefaultX, SprintClusterDefaultY);
    }
    public const float SprintClusterDefaultX = -386.5721f;
    public const float SprintClusterDefaultY = -64;


    private static Vector2 _inventoryClusterLocalPositionBoosted = new(InventoryClusterDefaultX, InventoryClusterDefaultY);
    public static Vector2 InventoryClusterLocalPositionBoosted
    {
        get
        {
            return _sprintClusterLocalPositionBoosted;
        }
    }
    private static Vector2 _inventoryClusterLocalPosition = new(InventoryClusterDefaultX, InventoryClusterDefaultY);
    public static Vector2 InventoryClusterLocalPosition
    {
        get
        {
            return _inventoryClusterLocalPosition;
        }
        set
        {
            Transform inventoryCluster = SkillDisplayRoot.Find("InventoryCluster");
            inventoryCluster.position = new Vector3(
                HealthBarRoot.position.x,
                inventoryCluster.position.y,
                inventoryCluster.position.z
            );
            _inventoryClusterLocalPosition = value;
            _inventoryClusterLocalPositionBoosted = _inventoryClusterLocalPosition;
            _inventoryClusterLocalPositionBoosted.y -= ConfigOptions.ShowSkillKeybinds.Value ? 42 : 0;
            inventoryCluster.localPosition = _inventoryClusterLocalPositionBoosted;
        }
    }
    public static void ResetInventoryClusterLocalPositionToDefault()
    {
        InventoryClusterLocalPosition = new Vector2(InventoryClusterDefaultX, InventoryClusterDefaultY);
    }
    public const float InventoryClusterDefaultX = -458.5721f;
    public const float InventoryClusterDefaultY = -64;


    internal static void HudDetailEdits()
    {
        SetSprintAndInventoryKeybindsStatus();
        RemoveSprintAndInventoryReminderTextBackgrounds();
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


    internal static void RepositionSprintAndInventoryReminders()
    {
        SprintClusterLocalPosition = SprintClusterLocalPosition;
        InventoryClusterLocalPosition = InventoryClusterLocalPosition;
    }
}