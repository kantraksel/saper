using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildProcessor
{
    private static BuildPlayerOptions GetBaseOptions()
    {
        var options = new BuildPlayerOptions();
        options.scenes = new[] { "Assets/Scenes/Game.unity" };
        options.locationPathName = "sapergame.exe";
        return options;
    }

    [MenuItem("Project Actions/Build/Standalone Win64 Debug")]
    public static void BuildStandaloneWin64Debug()
    {
        var options = GetBaseOptions();
        options.locationPathName = $"build/debug/{options.locationPathName}";
        options.target = BuildTarget.StandaloneWindows64;
        options.options = BuildOptions.Development;

        Debug.Log("Started building 'Standalone Win64 Debug'");
        Build(options);
    }

    [MenuItem("Project Actions/Build/Standalone Win64 Shipping")]
    public static void BuildStandaloneWin64Shipping()
    {
        var options = GetBaseOptions();
        options.locationPathName = $"build/shipping/{options.locationPathName}";
        options.target = BuildTarget.StandaloneWindows64;
        options.options = BuildOptions.None;

        Debug.Log("Started building 'Standalone Win64 Shipping'");
        Build(options);
    }

    private static void Build(BuildPlayerOptions options)
    {
        var report = BuildPipeline.BuildPlayer(options);

        string message = $"Build {report.summary.result} in {(int)report.summary.totalTime.TotalSeconds}s";
        switch (report.summary.result)
        {
            case BuildResult.Succeeded:
                {
                    Debug.Log(message);
                    break;
                }

            case BuildResult.Failed:
                {
                    Debug.LogError(message);
                    break;
                }

            default:
                {
                    Debug.LogWarning(message);
                    break;
                }
        }
    }
}
