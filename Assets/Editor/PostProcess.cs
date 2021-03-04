using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

using Xcode = UnityEditor.iOS.Xcode;

public class PostProcess
{
	[PostProcessBuild]
	public static void FixError_ITMS90206(BuildTarget buildTarget, string pathToBuiltProject)
	{
		if (buildTarget == BuildTarget.iOS)
		{
			var projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

			var project = new Xcode.PBXProject();
			project.ReadFromString(File.ReadAllText(projPath));

			var mainTargetGuid = project.GetUnityMainTargetGuid();
			project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

			var unityFrameWorkTargetGuid = project.GetUnityFrameworkTargetGuid();
			project.SetBuildProperty(unityFrameWorkTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
			project.AddBuildProperty(unityFrameWorkTargetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");

			File.WriteAllText(projPath, project.WriteToString());
		}
	}
}