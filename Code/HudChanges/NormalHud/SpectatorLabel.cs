using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
using MonoDetour.HookGen;
using MonoDetour;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets]
internal static class SpectatorLabel
{
    private const float _spectatorLabelYPos = 250;


    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudChanges.HudStructure.OnHudStructureEditsBegun += HudStructureEdits;
    }


    internal static void HudStructureEdits()
    {
        Transform spectatorLabel = BottomCenterCluster.Find("SpectatorLabel");
        RectTransform spectatorLabelRect = spectatorLabel.GetComponent<RectTransform>();
        spectatorLabelRect.anchoredPosition = new Vector2(0f, _spectatorLabelYPos);


        Transform actualSpectatorLabel = spectatorLabel.Find("Backdrop");
        actualSpectatorLabel?.GetComponent<Image>().sprite = HudResources.HudAssets.WhiteSprite;
        Transform remoteOpLabel = spectatorLabel.Find("RemoteOp");
        remoteOpLabel?.GetComponent<Image>().sprite = HudResources.HudAssets.WhiteSprite;


        GraphicRaycaster bottomRightGraphicRaycaster = BottomRightCluster.GetComponent<GraphicRaycaster>();
        GraphicRaycaster bottomCenterGraphicRaycaster = BottomCenterCluster.gameObject.AddComponent<GraphicRaycaster>();
        bottomCenterGraphicRaycaster.blockingObjects = bottomRightGraphicRaycaster.blockingObjects;
        bottomCenterGraphicRaycaster.ignoreReversedGraphics = bottomRightGraphicRaycaster.ignoreReversedGraphics;
        bottomCenterGraphicRaycaster.useGUILayout = bottomRightGraphicRaycaster.useGUILayout;
    }




    internal static void MoveSpectatorLabel()
    {
        if (!IsHudEditable)
        {
            return;
        }
        Transform spectatorLabel = MyHudLocator.FindChild("BottomCenterCluster").Find("SpectatorLabel");
        if (spectatorLabel == null)
        {
            return;
        }


        spectatorLabel.localPosition = new Vector3(0, _spectatorLabelYPos);
    }
}