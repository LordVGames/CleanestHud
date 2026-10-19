using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud.ScoreboardPanel;


[MonoDetourTargets]
internal static class ScoreboardPanel
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudChanges.HudStructure.OnHudStructureEditsBegun += HudStructureEdits;
    }


    internal static void HudStructureEdits()
    {
        RectTransform scoreboardPanelRect = ImportantHudTransforms.ScoreboardPanel.GetComponent<RectTransform>();
        scoreboardPanelRect.localPosition = new Vector3(0, -90, 0);

        RectTransform stripContainerRect = ScoreboardStripContainer.GetComponent<RectTransform>();
        stripContainerRect.sizeDelta = new Vector2(0f, 80f);

        VerticalLayoutGroup stripContainerVerticalLayoutGroup = ScoreboardStripContainer.GetComponent<VerticalLayoutGroup>();
        stripContainerVerticalLayoutGroup.childForceExpandHeight = true;
        stripContainerVerticalLayoutGroup.childForceExpandWidth = true;
        stripContainerVerticalLayoutGroup.childControlHeight = true;
        stripContainerVerticalLayoutGroup.childControlWidth = true;
        stripContainerVerticalLayoutGroup.childScaleHeight = true;
        stripContainerVerticalLayoutGroup.childScaleWidth = true;
    }
}