using System;
using RoR2;
using RoR2.UI;
using MonoDetour;
using MonoDetour.HookGen;
using static CleanestHud.Main;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets(typeof(AllyCardController))]
internal static class AllyCards
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.UI.AllyCardController.Awake.Postfix(Awake);
        Mdh.RoR2.UI.AllyCardController.UpdateInfo.Postfix(UpdateInfo);
    }


    private static void Awake(AllyCardController self)
    {
        if (IsHudUserBlacklisted)
        {
            return;
        }


        // icons go out of the background when there's not a lot of allies so they need to be changed a lil bit
        Transform portrait = self.transform.GetChild(0);
        MyHud?.StartCoroutine(HudDetails.DelayEditAllyCardPortrait(portrait));
        if (!ConfigOptions.AllowAllyCardBackgrounds.Value)
        {
            self.DisableImageComponent();
        }


        // healthbar > BackgroundPanel
        Transform backgroundPanel = self.healthBar?.transform.GetChild(0);
        if (backgroundPanel != null)
        {
            MyHud?.StartCoroutine(DelayRemoveBadHealthSubBar(backgroundPanel));
        }
    }
    private static IEnumerator DelayRemoveBadHealthSubBar(Transform backgroundPanel)
    {
        yield return null;
        List<Transform> healthBarSubBars = backgroundPanel.FindListOfPartialMatches("HealthBarSubBar");
        if (healthBarSubBars != null && healthBarSubBars.Count > 1)
        {
            // the 2nd one is the bad super light-green hp bar that we wanna get rid of
            healthBarSubBars[1]?.DisableImageComponent();
        }
    }


    private static void UpdateInfo(AllyCardController self)
    {
        if (IsHudUserBlacklisted)
        {
            return;
        }


        if (ConfigOptions.AllowAllyCardBackgrounds.Value)
        {
            HudColor.ColorAllyCardControllerBackground(self);
        }
    }
}