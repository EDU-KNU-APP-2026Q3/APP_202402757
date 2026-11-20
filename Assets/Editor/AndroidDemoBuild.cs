using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

public static class AndroidDemoBuild
{
    [MenuItem("Tools/Android Demo/Build APK")]
    public static void Build()
    {
        if (EditorApplication.isPlaying)
            throw new InvalidOperationException("Stop Play Mode before building.");

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            throw new InvalidOperationException("Switch the active Build Profile to Android first.");

        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        // Include native emulator code to avoid ARM translation crashes.
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.X86_64;
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        AssetDatabase.SaveAssets();

        // Keep generated binaries outside the source project.
        string folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../../AndroidDemo"));
        Directory.CreateDirectory(folder);
        string apk = Path.Combine(folder, "APP_202402757.apk");
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        if (scenes.Length == 0)
            throw new InvalidOperationException("Enable the demo scenes in Build Profiles > Scene List.");

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = apk,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });
        string result = $"{report.summary.result}: {apk}\nErrors: {report.summary.totalErrors}";
        File.WriteAllText(Path.Combine(folder, "build-result.txt"), result);
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception(result);
        Debug.Log("[Android Demo] " + result);
    }
}
