using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;

namespace CleanestHud.HudChanges
{
    internal class Helpers
    {
        internal static Transform GetContainerFromScoreboardPanel(Transform scoreboardPanel)
        {
            Transform container = scoreboardPanel.GetChild(0);
            if (container.name == "CommandQueueScoreboardWrapper")
            {
                // commandqueue puts the entire scoreboard container in a new transform
                // and it needs to be accounted for or else it causes an NRE
                container = container.GetChild(0);
            }
            return container;
        }
        
        internal static Transform GetScoreboardPanelFromContainer(Transform container)
        {
            Transform scoreboardPanel = container.parent;
            if (scoreboardPanel.name == "CommandQueueScoreboardWrapper")
            {
                // commandqueue puts the entire scoreboard container in a new transform
                // and it needs to be accounted for or else it causes an NRE
                scoreboardPanel = scoreboardPanel.parent;
            }
            return scoreboardPanel;
        }

        internal static bool TestLevelDisplayClusterAvailability()
        {
            // LevelDisplayCluster sometimes doesn't exist even though IsHudFinishedLoading is true???????
            // it works when CameraModeBase_OnTargetChanged goes off though so as long as we can catch it and handle it we're fine

            Transform levelDisplayCluster;
            try
            {
                // BarRoots is at BottomLeftCluster before ChangeRestOfHud changes it to BottomCenterCluster
                levelDisplayCluster = HudResources.ImportantHudTransforms.BarRoots.Find("LevelDisplayCluster");
            }
            catch
            {
                Log.Info("levelDisplayCluster could not be found\nthis can mean something messed up, but more than likely everthing is fine");
                return false;
            }
            return true;
        }
    }
}
