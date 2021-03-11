using System.Collections.Generic;
using HomaGames.HomaBelly;
using UnityEditor;
using UnityEngine;

public class HomaBellyPackagesWindow : HomaBellyBaseWindow
{
    private Vector2 installedPackagesScrollPosition;

    protected override void Draw(Rect windowPosition)
    {
        // Gather packages to be installed
        PluginManifest pluginManifest = PluginManifest.LoadFromLocalFile();
        List<PackageComponent> components = new List<PackageComponent>();
        if (pluginManifest != null && pluginManifest.Packages != null)
        {
            if (pluginManifest.Packages.CorePackages != null)
            {
                components.AddRange(pluginManifest.Packages.CorePackages);
            }

            if (pluginManifest.Packages.MediationLayers != null)
            {
                components.AddRange(pluginManifest.Packages.MediationLayers);
            }

            if (pluginManifest.Packages.AttributionPlatforms != null)
            {
                components.AddRange(pluginManifest.Packages.AttributionPlatforms);
            }

            if (pluginManifest.Packages.AdNetworks != null)
            {
                components.AddRange(pluginManifest.Packages.AdNetworks);
            }

            if (pluginManifest.Packages.AnalyticsSystems != null)
            {
                components.AddRange(pluginManifest.Packages.AnalyticsSystems);
            }

            if (pluginManifest.Packages.Others != null)
            {
                components.AddRange(pluginManifest.Packages.Others);
            }
        }

        // Start drawing
        Vector2 originalIconSize = EditorGUIUtility.GetIconSize();
        EditorGUIUtility.SetIconSize(new Vector2(16, 16));

        GUILayout.Space(EditorGUIUtility.singleLineHeight);

        var scrollViewHeight = windowPosition.height - 280;
        
        if (components != null && components.Count > 0 && scrollViewHeight>20)
        {
            installedPackagesScrollPosition = GUILayout.BeginScrollView(installedPackagesScrollPosition,
                false, false,GUILayout.MaxHeight(scrollViewHeight));

            foreach (PackageComponent component in components)
            {
                PackageComponent packageComponent = pluginManifest.Packages.GetPackageComponent(component.Id, component.Version);
                if (packageComponent != null)
                {
                    Texture2D icon = HomaGamesStyle.WhiteCircleIcon;
                    if (!homaBellyWindowController.IsInstalling())
                    {
                        icon = PackageCommon.IsPackageInstalled(packageComponent) ? HomaGamesStyle.FoundIcon : HomaGamesStyle.NotFoundIcon;
                    }

                    GUILayout.BeginHorizontal(new GUIStyle()
                    {
                        padding = new RectOffset((int)windowPosition.width / 2 - 100, 0, 0, 0)
                    });
                    GUILayout.Label(new GUIContent(icon), GUILayout.Height(20), GUILayout.Width(20));
                    GUILayout.Label($"{packageComponent.GetName()}");
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent(HomaGamesStyle.NotFoundIcon), GUILayout.Height(20), GUILayout.Width(20));
                    GUILayout.Label($"Could not find {component.GetName()}");
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
        }
        
        EditorGUIUtility.SetIconSize(originalIconSize);
    }

    protected override void OnVisibleFocus()
    {
        // NO-OP
    }
}
