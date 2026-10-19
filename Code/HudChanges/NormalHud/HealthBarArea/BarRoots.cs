using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using MonoDetour.HookGen;
using MonoDetour;
namespace CleanestHud.HudChanges.NormalHud.HealthBarArea;


[MonoDetourTargets]
internal static class BarRoots
{
    private static Vector2 _barRootsLocalPosition = new(BarRootsDefaultX, BarRootsDefaultY);
    public static Vector2 BarRootsLocalPosition
    {
        get
        {
            return _barRootsLocalPosition;
        }
        set
        {
            _barRootsLocalPosition = value;
            ImportantHudTransforms.BarRoots.localPosition = _barRootsLocalPosition;
        }
    }
    public static void ResetBarRootsLocalPositionToDefault()
    {
        BarRootsLocalPosition = new Vector2(BarRootsDefaultX, BarRootsDefaultY);
    }
    public const float BarRootsDefaultX = 0;
    public const float BarRootsDefaultY = 0;




    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudChanges.HudStructure.OnHudStructureEditsBegun += NormalHud.HealthBarArea.BarRoots.HudStructureEdits;
    }
    internal static void HudStructureEdits()
    {
		/*ImportantHudTransforms.BarRoots.SetParent(BottomCenterCluster);
        //ImportantHudTransforms.BarRoots.TryDestroyComponent<VerticalLayoutGroup>();
        RectTransform barRootsRect = ImportantHudTransforms.BarRoots.GetComponent<RectTransform>();
        barRootsRect.rotation = Quaternion.identity;
        //barRootsRect.pivot = new Vector2(0.5f, 0.25f);
        //barRootsRect.anchoredPosition = Vector2.zero;
        //barRootsRect.sizeDelta = new Vector2(-400f, 100f);*/

		ImportantHudTransforms.BarRoots.SetParent(BottomCenterCluster);
		ImportantHudTransforms.BarRoots.TryDestroyComponent<VerticalLayoutGroup>();
		RectTransform barRootsRect = ImportantHudTransforms.BarRoots.GetComponent<RectTransform>();
		barRootsRect.rotation = Quaternion.identity;
		barRootsRect.pivot = new Vector2(0.5f, 0.25f);
		barRootsRect.anchoredPosition = Vector2.zero;
		barRootsRect.sizeDelta = new Vector2(-400f, 100f);
	}
}