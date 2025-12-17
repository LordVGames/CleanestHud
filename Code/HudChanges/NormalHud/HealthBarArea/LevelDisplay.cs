using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud.HealthBarArea;


internal static class LevelDisplay
{
    internal static void HudStructureEdits()
    {
        RectTransform levelDisplayClusterRect = LevelDisplayCluster.GetComponent<RectTransform>();
        levelDisplayClusterRect.localPosition = new Vector3(-300f, 45f, 0f);


        Transform levelDisplayRoot = LevelDisplayCluster.Find("LevelDisplayRoot");
        RectTransform levelDisplayRootRect = levelDisplayRoot.GetComponent<RectTransform>();
        levelDisplayRootRect.pivot = new Vector2(0.5f, 0.5f);
        levelDisplayRootRect.localPosition = new Vector3(311, -27f, 0);
    }
}