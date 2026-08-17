using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CSharpWars.Editor
{
    public static class WebGlBuildAutomation
    {
        [MenuItem("CSharpWars/Build WebGL into CSharpWars.Web")]
        public static void BuildAndStage()
        {
            var frontendPath = Directory.GetParent(Application.dataPath)?.FullName
                ?? throw new InvalidOperationException("Could not determine the Unity project path.");
            var buildPath = Path.Combine(frontendPath, "Builds", "WebGL", "bin");

            if (Directory.Exists(buildPath))
            {
                Directory.Delete(buildPath, true);
            }

            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                throw new BuildFailedException("No enabled scenes are configured for the WebGL build.");
            }

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = buildPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            });

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"WebGL build failed with {report.summary.totalErrors} error(s).");
            }

            StageBuild(report.summary.outputPath);
        }

        internal static void StageBuild(string buildPath)
        {
            var buildAssetsPath = Path.Combine(buildPath, "Build");
            if (!Directory.Exists(buildAssetsPath))
            {
                throw new BuildFailedException($"WebGL build assets were not found at '{buildAssetsPath}'.");
            }

            var frontendPath = Directory.GetParent(Application.dataPath)?.FullName
                ?? throw new InvalidOperationException("Could not determine the Unity project path.");
            var webGlPath = Path.GetFullPath(
                Path.Combine(frontendPath, "..", "CSharpWars.Web", "wwwroot", "lib", "unity"));

            if (Directory.Exists(webGlPath))
            {
                Directory.Delete(webGlPath, true);
            }

            CopyDirectory(buildAssetsPath, webGlPath);

            var streamingAssetsPath = Path.Combine(buildPath, "StreamingAssets");
            if (Directory.Exists(streamingAssetsPath))
            {
                CopyDirectory(streamingAssetsPath, Path.Combine(webGlPath, "StreamingAssets"));
            }

            EnsureBuildFileExists(webGlPath, "bin.loader.js");
            EnsureBuildFileExists(webGlPath, "bin.data.gz");
            EnsureBuildFileExists(webGlPath, "bin.framework.js.gz");
            EnsureBuildFileExists(webGlPath, "bin.wasm.gz");

            Debug.Log($"Unity WebGL build copied to '{webGlPath}'.");
        }

        private static void CopyDirectory(string sourcePath, string destinationPath)
        {
            Directory.CreateDirectory(destinationPath);

            foreach (var filePath in Directory.GetFiles(sourcePath))
            {
                File.Copy(filePath, Path.Combine(destinationPath, Path.GetFileName(filePath)), true);
            }

            foreach (var directoryPath in Directory.GetDirectories(sourcePath))
            {
                CopyDirectory(
                    directoryPath,
                    Path.Combine(destinationPath, Path.GetFileName(directoryPath)));
            }
        }

        private static void EnsureBuildFileExists(string webGlPath, string fileName)
        {
            var filePath = Path.Combine(webGlPath, fileName);
            if (!File.Exists(filePath))
            {
                throw new BuildFailedException($"Expected WebGL build file was not found at '{filePath}'.");
            }
        }
    }

    public sealed class WebGlBuildStager : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (!Application.isBatchMode
                && report.summary.platform == BuildTarget.WebGL
                && report.summary.result == BuildResult.Succeeded)
            {
                WebGlBuildAutomation.StageBuild(report.summary.outputPath);
            }
        }
    }
}
