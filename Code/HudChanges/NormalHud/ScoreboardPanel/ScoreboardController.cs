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
    }
    private static void EditScoreboardStripsIfApplicable(RoR2.UI.ScoreboardController scoreboardController)
    {
        foreach (var scoreboardStrip in scoreboardController.stripAllocator.elements)
        {
            Transform scoreboardStripTransform = scoreboardStrip.transform;


            Transform longBackground = scoreboardStripTransform.GetChild(0);
            Image longBackgroundImage = longBackground.GetComponent<Image>();
            if (IsHudEditable && scoreboardStrip.userBody != null && !Helpers.AreColorsEqualIgnoringAlpha(longBackgroundImage.color, scoreboardStrip.userBody.bodyColor))
            {
                HudColor.ColorAllOfScoreboardStrip(scoreboardStrip, scoreboardStrip.userBody.bodyColor);
            }
            Transform itemsBackground = longBackground.GetChild(6);
            // DO NOT remove the items background image
            // if you do it will break the automatic scaling of the hud at ultrawide resolutions
            itemsBackground.GetComponent<Image>().color = Color.clear;
        }
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