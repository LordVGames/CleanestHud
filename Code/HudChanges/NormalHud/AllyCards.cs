using System;
using RoR2;
using RoR2.UI;
using MonoDetour;
using MonoDetour.HookGen;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
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
        MyHud?.StartCoroutine(DelayEditAllyCardPortrait(portrait));
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

    internal static void SetAllyCardBackgroundsStatus()
    {
        Transform allyCardContainer = MyHudLocator.FindChild("LeftCluster").Find("AllyCardContainer");
        for (int i = 0; i < allyCardContainer.childCount; i++)
        {
            Transform allyCard = allyCardContainer.GetChild(i);
            Image background = allyCard.GetComponent<Image>();
            background.enabled = ConfigOptions.AllowAllyCardBackgrounds.Value;
            // portrait edits get reset after enabling/disabling the background
            MyHud?.StartCoroutine(DelayEditAllyCardPortrait(allyCard.GetChild(0)));
        }
    }
    internal static IEnumerator DelayEditAllyCardPortrait(Transform portrait)
    {
        yield return null;
        EditAllyCardPortrait(portrait);
    }
    internal static void EditAllyCardPortrait(Transform portrait)
    {
        portrait.localPosition = new Vector3(-0.9f, -24, 1);
        portrait.localScale = new Vector3(1, 0.99f, 1);
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


        if (ConfigOptions.AllowAllyCardBackgrounds.Value && ConfigOptions.AllowHudColorEdits.Value)
        {
            ColorAllyCardControllerBackground(self);
        }
    }
    internal static void ColorAllyCardControllerBackground(AllyCardController allyCardController)
    {
        if (!allyCardController || !allyCardController.cachedSourceCharacterBody)
        {
            return;
        }

        Image background = allyCardController.GetComponent<Image>();
        Color colorToUse = allyCardController.cachedSourceCharacterBody.bodyColor;
        colorToUse.a = 0.15f;
        background.sprite = HudAssets.WhiteSprite;
        background.color = colorToUse;
    }
    internal static void ColorAllAllyCardBackgrounds()
    {
        Transform allyCardContainer = MyHudLocator.FindChild("LeftCluster").GetChild(0);
        for (int i = 0; i < allyCardContainer.childCount; i++)
        {
            Transform allyCard = allyCardContainer.GetChild(i);
            AllyCardController allyCardController = allyCard.GetComponent<AllyCardController>();
            ColorAllyCardControllerBackground(allyCardController);
        }
    }
}