using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AS.Core.Editor
{
	public static class BuildFile
	{
	
        [MenuItem("AS/Build/Android Build")]
        public static void AndroidBuild()
        {
           // CheckStreamingAssetsFolder();

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            /*BuildPipeline.BuildAssetBundles("Assets/StreamingAssets",
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.Android);*/

            AssetDatabase.Refresh();

            /*if (File.Exists("Assets/StreamingAssets/StreamingAssets"))
            {
                FileUtil.ReplaceFile("Assets/StreamingAssets/StreamingAssets", "Assets/StreamingAssets/Android");
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/StreamingAssets");
            }
            if (File.Exists("Assets/StreamingAssets/StreamingAssets.meta"))
            {
                FileUtil.ReplaceFile("Assets/StreamingAssets/StreamingAssets.meta", "Assets/StreamingAssets/Android.meta");
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/StreamingAssets.meta");
            }
            if (File.Exists("Assets/StreamingAssets/StreamingAssets.manifest"))
            {
                FileUtil.ReplaceFile("Assets/StreamingAssets/StreamingAssets.manifest", "Assets/StreamingAssets/Android.manifest");
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/StreamingAssets.manifest");
            }
            if (File.Exists("Assets/StreamingAssets/StreamingAssets.manifest.meta"))
            {
                FileUtil.ReplaceFile("Assets/StreamingAssets/StreamingAssets.manifest.meta", "Assets/StreamingAssets/Android.manifest.meta");
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/StreamingAssets.manifest.meta");
            }*/

            BuildPlayerOptions buildOptions = default;
            buildOptions.options = BuildOptions.CompressWithLz4HC;
            buildOptions.locationPathName = $"Build/{Application.productName}.apk";
            buildOptions.scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
            buildOptions.target = BuildTarget.Android;

            var backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

           // Fabric.Internal.Editor.Prebuild.FabricAndroidPrebuild.UpdateBuildId();
            AssetDatabase.Refresh();

            BuildPipeline.BuildPlayer(buildOptions);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, backend);
        }
        
        [MenuItem("AS/Build/iOS")]
        public static void iOSBuild()
        {

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

            /*
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.iOS);
                */

            AssetDatabase.Refresh();

            /*if (File.Exists("Assets/StreamingAssets/StreamingAssets"))
            {
                FileUtil.ReplaceFile("Assets/StreamingAssets/StreamingAssets", "Assets/StreamingAssets/iOS");
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/StreamingAssets");
            }
            if (File.Exists("Assets/StreamingAssets/StreamingAssets.meta"))
            {
                FileUtil.ReplaceFile("Assets/StreamingAssets/StreamingAssets.meta", "Assets/StreamingAssets/iOS.meta");
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/StreamingAssets.meta");
            }
            if (File.Exists("Assets/StreamingAssets/StreamingAssets.manifest"))
            {
                FileUtil.ReplaceFile("Assets/StreamingAssets/StreamingAssets.manifest", "Assets/StreamingAssets/iOS.manifest");
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/StreamingAssets.manifest");
            }
            if (File.Exists("Assets/StreamingAssets/StreamingAssets.manifest.meta"))
            {
                FileUtil.ReplaceFile("Assets/StreamingAssets/StreamingAssets.manifest.meta", "Assets/StreamingAssets/iOS.manifest.meta");
                FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/StreamingAssets.manifest.meta");
            }*/

            var buildOptions = default(BuildPlayerOptions);
            buildOptions.locationPathName = "Build/ios";
            buildOptions.scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
            buildOptions.options = BuildOptions.CompressWithLz4HC;
            buildOptions.target = BuildTarget.iOS;

            BuildPipeline.BuildPlayer(buildOptions);
        }
	}
}