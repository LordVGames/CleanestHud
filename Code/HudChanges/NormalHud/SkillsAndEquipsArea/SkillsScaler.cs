using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources.ImportantHudTransforms;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;


[MonoDetourTargets]
internal static class SkillsScaler
{
    public const float SkillScalerDefaultX = -174;
    public const float SkillScalerDefaultY = 100;
    internal const float SkillScalerXBoostValue = 35;
    internal const float SkillScalerYBoostValue = 27;
    private static Vector2 _skillScalerLocalPositionBoosted = new(SkillScalerDefaultX, SkillScalerDefaultY);
    private static Vector2 _skillScalerLocalPosition = new(SkillScalerDefaultX, SkillScalerDefaultY);
    public static Vector2 SkillScalerLocalPosition
    {
        get
        {
            return _skillScalerLocalPosition;
        }
        set
        {
            if (!IsHudEditable)
            {
                return;
            }


            RectTransform scalerRect = SkillDisplayRoot.GetComponent<RectTransform>();
            // if keybinds are shown, the scaler needs to be boosted up some to account for it
            _skillScalerLocalPosition = value;
            _skillScalerLocalPositionBoosted = _skillScalerLocalPosition;
            _skillScalerLocalPositionBoosted.x += MyHud.keyItemIcon.gameObject.activeSelf ? SkillScalerXBoostValue : 0;
            _skillScalerLocalPositionBoosted.y += ConfigOptions.ShowSkillKeybinds.Value ? SkillScalerYBoostValue : 0;
            scalerRect.localPosition = _skillScalerLocalPositionBoosted;
        }
    }
    public static void ResetSkillsScalerLocalPositionToDefault()
    {
        SkillScalerLocalPosition = new Vector2(SkillScalerDefaultX, SkillScalerDefaultY);
    }




    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudStructure.OnHudStructureEditsBegun += HudStructureEdits;
        ConfigOptions.OnShowSkillAndEquipmentKeybindsChanged += RepositionSkillScaler;
    }


    internal static void HudStructureEdits()
    {
        SkillDisplayRoot.SetParent(BottomCenterCluster);
        RectTransform scalerRect = SkillDisplayRoot.GetComponent<RectTransform>();
        scalerRect.rotation = Quaternion.identity;
        scalerRect.pivot = new Vector2(0.0875f, 0);
        scalerRect.sizeDelta = new Vector2(scalerRect.sizeDelta.x, -234f);
        scalerRect.localPosition = SkillScalerLocalPosition;
    }


    internal static void RepositionSkillScaler()
    {
        SkillScalerLocalPosition = SkillScalerLocalPosition;
    }
}