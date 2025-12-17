using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using RoR2.UI;
using MonoDetour.HookGen;
using MonoDetour;
using System.Collections;
namespace CleanestHud.HudChanges.NormalHud;

internal static class NotificationArea
{
    internal static void HudStructureEdits()
    {
        RectTransform notificationAreaRect = ImportantHudTransforms.NotificationArea.GetComponent<RectTransform>();
        notificationAreaRect.localEulerAngles = new Vector3(0f, 6f, 0f);
        // local position in game is boosted by +576.2813 X and +54 Y, Z is unaffected
        notificationAreaRect.localPosition = new Vector3(-26, -334, 0f);
        notificationAreaRect.anchorMin = new Vector2(0.8f, 0.05f);
        notificationAreaRect.anchorMax = new Vector2(0.8f, 0.05f);
    }


    internal static void HudDetailEdits()
    {
        ImportantHudTransforms.ContextNotification.Find("ContextDisplay").DisableRawImageComponent();
        // GetChild(2) is inspectDisplay
        ImportantHudTransforms.ContextNotification.GetChild(2).DisableRawImageComponent();
    }




    [MonoDetourTargets(typeof(NotificationUIController))]
    private static class RemovingNotificationBackground
    {
        [MonoDetourHookInitialize]
        private static void SetupHook()
        {
            Mdh.RoR2.UI.NotificationUIController.SetUpNotification.Postfix(StartRemoveBackgroundCoroutine);
        }


        private static void StartRemoveBackgroundCoroutine(NotificationUIController self, ref CharacterMasterNotificationQueue.NotificationInfo notificationInfo)
        {
            if (IsHudUserBlacklisted)
            {
                return;
            }

            MyHud?.StartCoroutine(DelayRemoveNotificationBackground());
        }
        internal static IEnumerator DelayRemoveNotificationBackground()
        {
            // need to wait a frame or else only the first notification is changed???????
            yield return null;
            RemoveNotificationBackground();
        }
        private static void RemoveNotificationBackground()
        {
            if (ImportantHudTransforms.NotificationArea == null || ImportantHudTransforms.NotificationArea.childCount == 0)
            {
                Log.Debug("Notification area had no children, returning");
                return;
            }
            Transform notificationPanel = ImportantHudTransforms.NotificationArea.GetChild(0);
            if (notificationPanel == null)
            {
                Log.Debug("Couldn't find a notification panel to modify, returning");
                return;
            }


            GameObject notificationBackdrop = notificationPanel.GetChild(0).GetChild(0).gameObject;
            notificationBackdrop.SetActive(false);
        }
    }
}