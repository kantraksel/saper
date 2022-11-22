using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PreBuildUnit : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var summary = report.summary;

        var buildDir = Directory.GetParent(summary.outputPath);
        var targetName = Path.GetFileNameWithoutExtension(summary.outputPath);

        var dataDir = $"{buildDir}/Data";
        var dataBuildDir = $"{buildDir}/{targetName}_Data";
        if (Directory.Exists(dataDir))
        {
            try
            {
                Directory.Delete(dataBuildDir, true);
            }
            catch { }
            
            Directory.Move(dataDir, dataBuildDir);
        }
    }
}
