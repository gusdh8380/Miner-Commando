using UnityEditor;


public class TestMultiplay
{
    [MenuItem("Test/Test Multiplay")]
    private static void TestMultiplayWin64()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

        for (int n = 0; n < 2; n++)
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes = GetScenesPath();
            options.locationPathName = string.Format("Build/Win64/{0}/Test.exe", n);
            options.target = BuildTarget.StandaloneWindows64;
            options.options = BuildOptions.AutoRunPlayer;
            BuildPipeline.BuildPlayer(options);
        }
    }

    // Scene 경로 가져오기
    private static string[] GetScenesPath()
    {
        EditorBuildSettingsScene[] scenes = UnityEditor.EditorBuildSettings.scenes;
        string[] scenes_path = new string[scenes.Length];
        for (int n = 0; n < scenes.Length; n++)
        {
            scenes_path[n] = scenes[n].path;
        }

        return scenes_path;
    }
}