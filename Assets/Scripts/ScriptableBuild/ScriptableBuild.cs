#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AframaxBuildMinSizeRel
{
    private static readonly string _OUT_PATH = "MinSizeRel";
    private static readonly string _BURST_COMPILER_DIR = "Shrouded Depths_BurstDebugInformation_DoNotShip";
    
    [MenuItem("File/Build MinSizeRel/Windows")]
    public static void AframaxBuildWindows()
    {
        AframaxBuild(BuildTarget.StandaloneWindows64);
    }
    
    [MenuItem("File/Build MinSizeRel/Mac")]
    public static void AframaxBuildMac()
    {
        AframaxBuild(BuildTarget.StandaloneOSX);
    }
    
    [MenuItem("File/Build MinSizeRel/Linux")]
    public static void AframaxBuildLinux()
    {
        AframaxBuild(BuildTarget.StandaloneLinux64);
    }
    
    private static void AframaxBuild(BuildTarget buildTarget)
    {
        var buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(
            new BuildPlayerOptions()
        );
        
        buildPlayerOptions.options |= BuildOptions.CompressWithLz4HC;
        
        buildPlayerOptions.target = buildTarget;

        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var buildSummary = buildReport.summary;

        if (buildSummary.result == BuildResult.Succeeded)
        {
            Debug.Log("MinSizeRel successfully exported to: " +
                buildSummary.outputPath + " with size of " +
                buildSummary.totalSize + " bytes."
            );
        }
        else
        {
            throw new BuildFailedException(
                "Build failed! Ensure no errors and try again."
            );
        }
        
        // Remove burst compiler directory
        
        
        RemoveBurstCompilerDirectory(Path.Join(_OUT_PATH, _BURST_COMPILER_DIR));
    }

    private static void RemoveBurstCompilerDirectory(string path)
    {
        var files = Directory.GetFiles(path);

        foreach (var file in files)
        {
            File.Delete(file);
        }
        
        Directory.Delete(path, true);
    }
}

#endif // UNITY_EDITOR
