using System;
using RoR2;
using MonoDetour;
using MonoDetour.HookGen;
using static CleanestHud.Main;
using UnityEngine;
using UnityEngine.UI;
using MiscFixes.Modules;
using System.Collections;
using MonoDetour.DetourTypes;
namespace CleanestHud.HudChanges.NormalHud.ScoreboardPanel;


[MonoDetourTargets(typeof(RoR2.UI.ScoreboardController), GenerateControlFlowVariants = true)]
internal static class ScoreboardController
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.UI.ScoreboardController.Rebuild.Prefix(Rebuild_Prefix);
        Mdh.RoR2.UI.ScoreboardController.Rebuild.Postfix(Rebuild_Postfix);
        Mdh.RoR2.UI.ScoreboardController.SelectFirstScoreboardStrip.ControlFlowPrefix(SelectFirstScoreboardStrip);
    }


    private static void Rebuild_Prefix(RoR2.UI.ScoreboardController self)
    {
        if (IsHudUserBlacklisted)
        {
            return;
        }

        HudDetails.SetScoreboardLabelsActiveOrNot(self.transform);
    }


    private static void Rebuild_Postfix(RoR2.UI.ScoreboardController self)
    {
        if (IsHudUserBlacklisted)
        {
            return;
        }

        MyHud?.StartCoroutine(DelayScoreboardController_Rebuild(self));
    }
    private static IEnumerator DelayScoreboardController_Rebuild(RoR2.UI.ScoreboardController scoreboardController)
    {
        if (!IsHudEditable)
        {
            yield break;
        }

        // wait a frame first to make scoreboard strips added by other mods work
        yield return null;
        EditScoreboardStripsIfApplicable(scoreboardController);
        if (Helpers.IsGameModeSimulacrum)
        {
            SetupSuppressedItemsStripEditor(scoreboardController);
        }
    }
    private static void EditScoreboardStripsIfApplicable(RoR2.UI.ScoreboardController scoreboardController)
    {
        foreach (var scoreboardStrip in scoreboardController.stripAllocator.elements)
        {
            Transform scoreboardStripTransform = scoreboardStrip.transform;

            if (scoreboardStrip.itemInventoryDisplay.itemIconPrefabWidth != 58)
            {
                HudChanges.NormalHud.ScoreboardPanel.ScoreboardStrips.EditScoreboardStrip(scoreboardStrip);
            }

            Transform longBackground = scoreboardStripTransform.GetChild(0);
            Image longBackgroundImage = longBackground.GetComponent<Image>();
            if (IsHudEditable && scoreboardStrip.userBody != null && !Helpers.AreColorsEqualIgnoringAlpha(longBackgroundImage.color, scoreboardStrip.userBody.bodyColor))
            {
                HudColor.ColorAllOfScoreboardStrip(scoreboardStrip, scoreboardStrip.userBody.bodyColor);
            }
        }
    }
    private static void SetupSuppressedItemsStripEditor(RoR2.UI.ScoreboardController scoreboardController)
    {
        scoreboardController.transform.Find("Container/SuppressedItems")?.GetOrAddComponent<HudEditorComponents.SuppressedItemsStripEditor>();
    }


    private static ReturnFlow SelectFirstScoreboardStrip(RoR2.UI.ScoreboardController self)
    {
        if (IsHudUserBlacklisted || ConfigOptions.AllowAutoScoreboardHighlight.Value)
        {
            return ReturnFlow.None;
        }
        return ReturnFlow.SkipOriginal;
    }
}