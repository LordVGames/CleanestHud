using CleanestHud.ModSupport;
using static CleanestHud.Main;
using static CleanestHud.HudChanges.HudColor;
using static CleanestHud.HudResources.ImportantHudTransforms;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2.UI;
using UnityEngine.UI;
using MonoDetour.HookGen;
using MonoDetour;
using System.Collections;
using HG;
namespace CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea.KeyItem;


[MonoDetourTargets]
internal static class KeyItemSlot
{
    private static Vector3 _standardLocalPos = new(0, -23);
    private static Vector3 _driverLocalPos = new(-133, _standardLocalPos.y);
    private static Vector3 _mystLocalPos = new(18, _standardLocalPos.y);


    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudStructure.OnHudStructureEditsBegun += HudStructureEdits;
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
        HudColor.OnHudColorEditsBegun += HudColorEdits;
        Main.OnHudInitImmediate += Main_OnHudInitImmediate;
        ConfigOptions.OnShowSkillAndEquipmentKeybindsChanged += ConfigOptions_OnShowSkillAndEquipmentKeybindsChanged;
    }


    private static void ConfigOptions_OnShowSkillAndEquipmentKeybindsChanged()
    {
        HudResources.ImportantHudTransforms.KeyItemSlot.transform.Find("DisplayRoot/KeyItemTextBackgroundPanel").gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
    }


    private static void HudDetailEdits()
    {
        Transform keyItemTextBackgroundPanel = HudResources.ImportantHudTransforms.KeyItemSlot.transform.Find("DisplayRoot/KeyItemTextBackgroundPanel");
        keyItemTextBackgroundPanel.DisableImageComponent();
    }


    // ts kinda dumb but whatever it works fine
    private static void Main_OnHudInitImmediate()
    {
        MyHud.onTargetMasterDiscovered += MyHud_onTargetMasterDiscovered;
    }
    private static void MyHud_onTargetMasterDiscovered(RoR2.CharacterMaster obj)
    {
        ActuallySetPosition();
        MyHud.targetMaster?.inventory?.onKeyItemAddedClient += KeyItem_onKeyItemAddedClient;
    }
    private static void KeyItem_onKeyItemAddedClient(RoR2.KeyItemIndex obj)
    {
        CheckToMoveKeyItemSlot();
    }
    private static void CheckToMoveKeyItemSlot()
    {
        if (!MyHud.keyItemIcon.gameObject.activeSelf)
        {
            MyHud?.StartCoroutine(DelaySetPosition());
        }
    }
    private static IEnumerator DelaySetPosition()
    {
        yield return null;
        ActuallySetPosition();
        yield break;
    }
    private static void ActuallySetPosition()
    {
        SkillsScaler.SkillScalerLocalPosition = SkillsScaler.SkillScalerLocalPosition;
        if (Myst.IsHudCameraTargetMyst)
        {
            Myst.MoveMystScaler();
        }
        HudStructureEdits();
    }


    internal static void HudStructureEdits()
    {
        float scaleFactor = 0.9f;
        Transform displayRoot = MyHud?.keyItemIcon.transform.Find("DisplayRoot");
        RectTransform displayRootRect = displayRoot.GetComponent<RectTransform>();
        displayRootRect.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        MyHud?.StartCoroutine(DelaySetLocalPos());

        Transform cooldownText = displayRoot.Find("CooldownText");
        RectTransform cooldownTextRect = cooldownText.GetComponent<RectTransform>();
        cooldownTextRect.localPosition = new Vector3(0f, 1f, 0f);

        Transform keyTextBackgroundPanel = displayRoot.Find("KeyItemTextBackgroundPanel");
        keyTextBackgroundPanel.localPosition = new Vector3(0, -33);
        keyTextBackgroundPanel.gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
    }
    private static IEnumerator DelaySetLocalPos()
    {
        yield return null;


        var actualLocalPos = _standardLocalPos;
        if (Myst.IsHudCameraTargetMyst)
        {
            actualLocalPos = _mystLocalPos;
        }
        else if (Driver.IsHudCameraTargetDriver)
        {
            actualLocalPos = _driverLocalPos;
        }
        var positionSetter = MyHud?.keyItemIcon.EnsureComponent<HudEditorComponents.TransformLocalPositionSetter>();
        positionSetter.SetNewLocalPositionForSetterDelayed(actualLocalPos);


        yield break;
    }


    internal static void HudColorEdits()
    {
        Image BGPanelImage = HudResources.ImportantHudTransforms.KeyItemSlot.transform.Find("DisplayRoot").transform.Find("BGPanel").GetComponent<Image>();
        BGPanelImage.color = Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);
    }
}