using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine;
using RoR2;
using RoR2.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudChanges.HudEditorComponents;
using MiscFixes.Modules;
using CleanestHud.HudChanges.NormalHud;
namespace CleanestHud.HudChanges;


public static class HudDetails
{
    /// <summary>
    /// The second phase of editing the HUD BEFORE this mod's HUD Edits. This is for disabling things like outlines and other detailing bits on HUD elements.
    /// </summary>
    public static event Action OnHudDetailEditsBegun;


    /// <summary>
    /// The second phase of editing the HUD AFTER this mod's HUD Edits. This is for disabling things like outlines and other detailing bits on HUD elements.
    /// </summary>
    public static event Action OnHudDetailEditsFinished;


    internal static void BeginEdits()
    {
        if (!ConfigOptions.AllowHudDetailsEdits.Value)
        {
            return;
        }
        OnHudDetailEditsBegun?.Invoke();
        OnHudDetailEditsFinished?.Invoke();
    }




    internal static IEnumerator DelayRemoveBadHealthSubBarFromAllAllyCards()
    {
        yield return null;
        RemoveBadHealthSubBarFromAllAllyCards();
    }
    private static void RemoveBadHealthSubBarFromAllAllyCards()
    {
        if (!IsHudEditable)
        {
            return;
        }
        AllyCardManager allyCardManager = MyHudLocator.FindChild("LeftCluster").Find("AllyCardContainer").GetComponent<AllyCardManager>();
        if (allyCardManager.cardAllocator.elements.Count == 0)
        {
            return;
        }


        foreach (AllyCardController allyCard in allyCardManager.cardAllocator.elements)
        {
            // HealthBar > BackgroundPanel > HealthBarSubBar(Clone) #2
            Transform backgroundPanel = allyCard.healthBar.transform.GetChild(0);
            if (backgroundPanel.childCount < 3)
            {
                return;
            }
            Transform badHealthBar = backgroundPanel.GetChild(2);
            if (badHealthBar == null)
            {
                return;
            }
            badHealthBar.DisableImageComponent();
        }
    }


    internal static void SetScoreboardLabelsActiveOrNot(Transform scoreboardPanel)
    {
        Transform container = Helpers.GetContainerFromScoreboardPanel(scoreboardPanel);
        Transform headerGroup = container.GetChild(0);

        headerGroup?.gameObject.SetActive(ConfigOptions.AllowScoreboardLabels.Value);
    }

    



    internal static IEnumerator DelayRemoveMonstersItemsPanelDetails()
    {
        // stupidity incoming
        if (MyHud == null)
        {
            while (MyHud == null)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.15f);
        }
        yield return null;
        RemoveMonstersItemsPanelDetails();
    }
    private static void RemoveMonstersItemsPanelDetails()
    {
        if (!IsHudEditable)
        {
            return;
        }
        Transform enemyInfoPanel = ImportantHudTransforms.RunInfoHudPanel.Find("RightInfoBar/EnemyInfoPanel(Clone)");
        if (enemyInfoPanel == null)
        {
            return;
        }

        enemyInfoPanel.DisableImageComponent();
        Transform innerFrame = enemyInfoPanel.transform.Find("InnerFrame");
        innerFrame.DisableImageComponent();
        innerFrame.Find("InventoryContainer/InventoryDisplay").DisableImageComponent();
        innerFrame.Find("MonsterBodiesContainer/MonsterBodyIconContainer").DisableImageComponent();
    }
}