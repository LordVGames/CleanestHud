using System.IO;
using UnityEngine;

namespace CleanestHud
{
    public static class ModAssets
    {
        public static AssetBundle AssetBundle;
        public const string BundleName = "cleanesthud";

        public static string AssetBundlePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Plugin.PluginInfo.Location), BundleName);
            }
        }

        public static void Init()
        {
            AssetBundle = AssetBundle.LoadFromFile(AssetBundlePath);
        }
    }
}