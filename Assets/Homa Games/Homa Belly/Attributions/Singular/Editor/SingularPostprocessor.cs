using System.Collections.Generic;
using System.IO;
using HomaGames.HomaBelly.Utilities;
using UnityEditor;

#if UNITY_IOS
using UnityEditor.Callbacks;
#endif

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Configure Singular after importing the package
    /// </summary>
    public class SingularPostprocessor
    {
        [InitializeOnLoadMethod]
        static void Configure()
        {
            HomaBellyEditorLog.Debug($"Configuring {HomaBellySingularConstants.ID}");
            PluginManifest pluginManifest = PluginManifest.LoadFromLocalFile();

            if (pluginManifest != null)
            {
                PackageComponent packageComponent = pluginManifest.Packages
                    .GetPackageComponent(HomaBellySingularConstants.ID, HomaBellySingularConstants.TYPE);
                if (packageComponent != null)
                {
                    Dictionary<string, string> configurationData = packageComponent.Data;

                    // Create directory if does not exist
                    string parentPath = Directory.GetParent(HomaBellySingularConstants.CONFIG_FILE).ToString();
                    if (!string.IsNullOrEmpty(parentPath) && !Directory.Exists(parentPath))
                    {
                        Directory.CreateDirectory(parentPath);
                    }

                    File.WriteAllText(HomaBellySingularConstants.CONFIG_FILE, Json.Serialize(configurationData));
                }
            }
            else
            {
                HomaBellyEditorLog.Error($"Singular configuration data not found. Skipping.");
            }

            AndroidProguardUtils.AddProguardRules("\n-keep class com.singular.sdk.** { *; }\r\n-keep public class com.android.installreferrer.** { *; }\r\n-keep public class com.singular.unitybridge.** { *; }");
        }

#if UNITY_IOS
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
        {
            iOSPbxProjectUtils.AddFrameworks(buildTarget, buildPath, new string[] {
                "AdSupport.framework",
                "SystemConfiguration.framework",
                "Security.framework",
                "iAd.framework",
                "libsqlite3.0.tbd",
                "libz.tbd",
                "WebKit.framework"
            });
        }
#endif
    }
}
