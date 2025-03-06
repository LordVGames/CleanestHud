using System.IO;
using UnityEngine;

namespace CleanestHud
{
    public static class Assets
    {
        public static AssetBundle ModIconAssetBundle;
        public const string BundleName = "cleanesthudicon";

        public static string AssetBundlePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Plugin.PluginInfo.Location), BundleName);
            }
        }

        public static void Init()
        {
            ModIconAssetBundle = AssetBundle.LoadFromFile(AssetBundlePath);
        }
    }
}