using System;
using System.Collections;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2.UI;
using UnityEngine;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets(typeof(BuffDisplay))]
internal static class BuffsBar
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        // this used to be an ilhook but monodetour is goated and postfixes are kinda ilhooks anyways so this works great
        Mdh.RoR2.UI.BuffDisplay.UpdateLayout.Postfix(UpdateLayout);
    }


    private static void UpdateLayout(BuffDisplay buffDisplay)
    {
        // all healthbars have a buffDisplay, but the one for the player's hud has a special name
        if (buffDisplay.name != "BuffDisplayRoot" || buffDisplay.buffIconDisplayData.Count < 1)
        {
            return;
        }
        

        // the first buff always has +6 rotation on Y because ?????????? so it needs to be reset to 0
        // also i swear this was working without needing to delay it but now i have to?????
        if (buffDisplay.buffIconDisplayData[0].buffIconComponent != null)
        {
            MyHud?.StartCoroutine(DelayFixFirstBuffRotation(buffDisplay.buffIconDisplayData[0].buffIconComponent.rectTransform));
        }
        if (!IsHudUserBlacklisted)
        {
            buffDisplay.rectTranform.localPosition = new Vector3(-24 * buffDisplay.buffIconDisplayData.Count, -45, 0);
        }
    }
    private static IEnumerator DelayFixFirstBuffRotation(RectTransform rectTransform)
    {
        yield return null;
        if (IsHudFinishedLoading)
        {
            rectTransform.rotation = Quaternion.identity;
        }
    }
}