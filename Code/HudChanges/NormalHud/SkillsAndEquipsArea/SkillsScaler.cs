using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;


internal static class SkillsScaler
{
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
            _skillScalerLocalPositionBoosted.y += ConfigOptions.ShowSkillKeybinds.Value ? 27 : 0;
            scalerRect.localPosition = _skillScalerLocalPositionBoosted;
        }
    }
    public static void ResetSkillsScalerLocalPositionToDefault()
    {
        SkillScalerLocalPosition = new Vector2(SkillScalerDefaultX, SkillScalerDefaultY);
    }
    public const float SkillScalerDefaultX = 0;
    public const float SkillScalerDefaultY = 100;



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