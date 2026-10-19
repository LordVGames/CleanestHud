using CleanestHud.ModSupport;
using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;


[MonoDetourTargets]
internal static class KeybindReminders
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
    }


    internal static void HudDetailEdits()
    {
        foreach (SkillIcon skillIcon in MyHud.skillIcons)
        {
            skillIcon.transform.Find("SkillBackgroundPanel")?.DisableImageComponent();
        }
        // GetChild(1) is EquipmentDisplayRoot
        MyHud.equipmentIcons[0].transform?.GetChild(1)?.Find("EquipmentTextBackgroundPanel")?.DisableImageComponent();
        // GetChild(0) is DisplayRoot
        HudResources.ImportantHudTransforms.Scaler.Find("AltEquipmentSlot")?.GetChild(0).Find("ExtraEquipmentKey")?.Find("NextIcon")?.DisableImageComponent();
    }
}