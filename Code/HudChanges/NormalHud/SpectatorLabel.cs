using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud;


internal static class SpectatorLabel
{
    internal static void HudStructureEdits()
    {
        Transform spectatorLabel = BottomCenterCluster.Find("SpectatorLabel");
        RectTransform spectatorLabelRect = spectatorLabel.GetComponent<RectTransform>();
        spectatorLabelRect.anchoredPosition = new Vector2(0f, 200f);

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

        spectatorLabel.localPosition = new Vector3(0, 200, 0);
    }
}