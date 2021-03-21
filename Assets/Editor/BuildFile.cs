using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace AS.Core.Editor
{
	public static class BuildFile
	{
	
        [MenuItem("AS/Build/Android Build")]
        public static void AndroidBuild()
        {

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);


            AddressableAssetSettings.BuildPlayerContent();

            AssetDatabase.Refresh();

            

            BuildPlayerOptions buildOptions = default;
            buildOptions.options = BuildOptions.CompressWithLz4HC;
            buildOptions.locationPathName = $"Build/{Application.productName}.apk";
            buildOptions.scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
            buildOptions.target = BuildTarget.Android;

            var backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

            AssetDatabase.Refresh();

            BuildPipeline.BuildPlayer(buildOptions);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, backend);
        }
        
        [MenuItem("AS/Build/iOS")]
        public static void iOSBuild()
        {

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

			AssetDatabase.Refresh();
			

            var buildOptions = default(BuildPlayerOptions);
            buildOptions.locationPathName = "Build/ios";
            buildOptions.scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
            buildOptions.options = BuildOptions.CompressWithLz4HC;
            buildOptions.target = BuildTarget.iOS;

            BuildPipeline.BuildPlayer(buildOptions);
        }
	}
}