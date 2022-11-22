using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PostBuildUnit : IPostprocessBuildWithReport
{
    private const string ExeStorage = "build/GameExe";
    private const string Win64Exe = "x64/Release/WindowsPlayer.exe";
    private const string Win32Exe = "x86/Release/WindowsPlayer.exe";
    private const string SteamAppId = "480";

    public int callbackOrder => 99;

    public void OnPostprocessBuild(BuildReport report)
    {
        var summary = report.summary;

        var buildDir = Directory.GetParent(summary.outputPath);
        var targetName = Path.GetFileNameWithoutExtension(summary.outputPath);

        Directory.Move($"{buildDir}/{targetName}_Data", $"{buildDir}/Data");
        File.WriteAllText($"{buildDir}/steam_appid.txt", SteamAppId);
        ApplyCustomExe(summary, buildDir.FullName, targetName);

        Process.Start(buildDir.ToString());
    }

    private void ApplyCustomExe(BuildSummary summary, string buildDir, string targetName)
    {
        string exe;
        switch (summary.platform)
        {
            case BuildTarget.StandaloneWindows64:
                {
                    exe = Win64Exe;
                    break;
                }

            case BuildTarget.StandaloneWindows:
                {
                    exe = Win32Exe;
                    break;
                }

            default:
                return;
        }

        targetName = $"{buildDir}/{targetName}.exe";
        exe = $"{ExeStorage}/{exe}";

        if (File.Exists(exe))
        {
            File.Delete(targetName);
            File.Copy(exe, targetName);
        }
    }
}
