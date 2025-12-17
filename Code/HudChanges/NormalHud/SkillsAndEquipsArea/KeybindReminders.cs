using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using CleanestHud.ModSupport;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;


internal static class KeybindReminders
{
    internal static void HudDetailEdits()
    {
        foreach (SkillIcon skillIcon in MyHud.skillIcons)
        {
            skillIcon.transform.Find("SkillBackgroundPanel")?.DisableImageComponent();
        }
        // GetChild(1) is EquipmentDisplayRoot
        MyHud.equipmentIcons[0].transform?.GetChild(1)?.Find("EquipmentTextBackgroundPanel")?.DisableImageComponent();
    }
}