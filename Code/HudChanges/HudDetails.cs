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
namespace CleanestHud.HudChanges;


public static class HudDetails
{
    /// <summary>
    /// The second phase of editing the HUD BEFORE this mod's HUD edits. This is for disabling things like outlines and other detailing bits on HUD elements.
    /// </summary>
    public static event Action OnHudDetailEditsBegun;


    /// <summary>
    /// The second phase of editing the HUD AFTER this mod's HUD edits. This is for disabling things like outlines and other detailing bits on HUD elements.
    /// </summary>
    public static event Action OnHudDetailEditsFinished;


    internal static void AddEventSubscriptions()
    {
        OnHudDetailEditsBegun += NormalHud.ArtifactPanel.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.BossHpBar.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.CurrenciesArea.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.InspectionPanel.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.ItemInventoryDisplayArea.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.MapNameText.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.NotificationArea.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.TimerPanel.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.DifficultyHud.DifficultyBar.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.DifficultyHud.Wormgear.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.ScoreboardPanel.ScoreboardStrips.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.SkillsAndEquipsArea.KeybindReminders.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.SkillsAndEquipsArea.SkillSlots.HudDetailEdits;
        OnHudDetailEditsBegun += NormalHud.SkillsAndEquipsArea.SprintAndInventoryReminders.HudDetailEdits;
        OnHudDetailEditsBegun += Simulacrum.DefaultWaveUI.HudDetailEdits;
        OnHudDetailEditsBegun += Simulacrum.SuppressedItemsStrip.HudDetailEdits;
        OnHudDetailEditsBegun += Simulacrum.WavePanel.HudDetailEdits;
    }


    internal static void BeginEdits()
    {
        if (!ConfigOptions.AllowHudDetailsEdits.Value)
        {
            return;
        }
        
        OnHudDetailEditsBegun?.Invoke();
        OnHudDetailEditsFinished?.Invoke();
    }



    


    internal static void SetSkillsAndEquipmentReminderTextStatus()
    {
        foreach (SkillIcon skillIcon in MyHud.skillIcons)
        {
            Transform skillTextBackgroundPanel = skillIcon.transform.GetChild(5);
            skillTextBackgroundPanel.gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
        }

        Transform equipment1DisplayRoot = MyHud.equipmentIcons[0].displayRoot.transform;
        GameObject equipment1TextBackgroundPanel = equipment1DisplayRoot.Find("EquipmentTextBackgroundPanel").gameObject;
        equipment1TextBackgroundPanel.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
    }


    


    #region Infinite last difficulty segment
    
    #endregion


    internal static void SetSimulacrumWaveBarAnimatorStatus()
    {
        if (!Helpers.IsGameModeSimulacrum)
        {
            return;
        }
        Transform simulacrumWaveUIClone = ImportantHudTransforms.RunInfoHudPanel.Find("InfiniteTowerDefaultWaveUI(Clone)");
        if (!simulacrumWaveUIClone)
        {
            return;
        }

        InfiniteTowerWaveProgressBar progressBar = simulacrumWaveUIClone.GetComponent<InfiniteTowerWaveProgressBar>();
        Animator progressBarAnimator = simulacrumWaveUIClone.GetComponent<Animator>();
        if (ConfigOptions.AllowSimulacrumWaveBarAnimating.Value)
        {
            progressBarAnimator.enabled = true;
            progressBar.animator = progressBarAnimator;
        }
        else
        {
            progressBar.animator = null;
            progressBarAnimator.enabled = false;
        }
    }



    internal static void SetAllyCardBackgroundsStatus()
    {
        Transform allyCardContainer = MyHudLocator.FindChild("LeftCluster").Find("AllyCardContainer");
        for (int i = 0; i < allyCardContainer.childCount; i++)
        {
            Transform allyCard = allyCardContainer.GetChild(i);
            Image background = allyCard.GetComponent<Image>();
            background.enabled = ConfigOptions.AllowAllyCardBackgrounds.Value;
            // portrait edits get reset after enabling/disabling the background
            MyHud?.StartCoroutine(DelayEditAllyCardPortrait(allyCard.GetChild(0)));
        }
    }
    internal static IEnumerator DelayEditAllyCardPortrait(Transform portrait)
    {
        yield return null;
        EditAllyCardPortrait(portrait);
    }
    internal static void EditAllyCardPortrait(Transform portrait)
    {
        portrait.localPosition = new Vector3(-0.9f, -24, 1);
        portrait.localScale = new Vector3(1, 0.99f, 1);
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



    internal static void RemoveBadHealthSubBarFromPersonalHealthBar()
    {
        Image badHpBarImage = ImportantHudTransforms.BarRoots.Find("HealthbarRoot").GetChild(0).GetChild(1).GetComponent<Image>();
        // this image CAN sometimes be null and i don't want an error message to appear if it is
        // so we're not using DisableImageComponent here
        if (badHpBarImage != null)
        {
            badHpBarImage.enabled = false;
        }
        else
        {
            Log.Info("Couldn't find bad HP bar image. There's a chance an HP bar may appear more light-green than usual.");
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



    // there's a lil bit of restructuring but it's 99% detail editing so idc it's going here
    internal static void EditMoonDetonationPanel()
    {
        Transform hudCountdownPanel = MyHudLocator.FindChild("TopCenterCluster").Find("HudCountdownPanel(Clone)");
        if (hudCountdownPanel == null)
        {
            Log.Warning("Couldn't find HudCountdownPanel for editing???");
            return;
        }
        
        hudCountdownPanel.localPosition = new Vector3(0, -200f, 0f);

        // panel > Juice > Container
        Transform container = hudCountdownPanel.GetChild(0).GetChild(0);

        container.Find("Backdrop").DisableImageComponent();
        container.Find("Border").DisableImageComponent();

        Transform countdownTitleLabel = container.Find("CountdownTitleLabel");
        HGTextMeshProUGUI countdownTitleLabelMesh = countdownTitleLabel.GetComponent<HGTextMeshProUGUI>();
        countdownTitleLabelMesh.fontSharedMaterial = HudAssets.FontMaterial;
        countdownTitleLabelMesh.color = Color.red;

        Transform countdownLabel = container.Find("CountdownLabel");
        HGTextMeshProUGUI countdownLabelMesh = countdownLabel.GetComponent<HGTextMeshProUGUI>();
        countdownLabelMesh.fontSharedMaterial = HudAssets.FontMaterial;
        countdownLabelMesh.color = Color.red;
    }
}