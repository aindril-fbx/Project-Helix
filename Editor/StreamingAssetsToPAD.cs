using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class StreamingAssetsToPAD : MonoBehaviour
{
    private const string streamingAssetsPath = "Assets/StreamingAssets";
    private const string outputBundlePath = "Assets/AssetBundles/Android/";
    private const string assetPacksRoot = "Assets/AssetPacks/";
    private const string assetPackConfigPath = "Assets/GooglePlayPlugins/PlayAssetDelivery/AssetPackConfig.json";

    [MenuItem("Tools/Convert StreamingAssets to PAD")]
    public static void ConvertStreamingAssetsToPAD()
    {
        if (!Directory.Exists(streamingAssetsPath))
        {
            Debug.LogWarning("StreamingAssets folder does not exist.");
            return;
        }

        Directory.CreateDirectory(outputBundlePath);
        Directory.CreateDirectory(assetPacksRoot);

        var bundleBuilds = new List<AssetBundleBuild>();
        var assetPackList = new List<string>();

        // Process all files in StreamingAssets
        string[] files = Directory.GetFiles(streamingAssetsPath, "*.*", SearchOption.AllDirectories);

        foreach (string fullPath in files)
        {
            if (Path.GetExtension(fullPath) == ".meta") continue;

            string relativePath = fullPath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            string bundleName = Path.GetFileNameWithoutExtension(relativePath).ToLower();

            // Assign to bundle
            var abb = new AssetBundleBuild
            {
                assetBundleName = bundleName,
                assetNames = new[] { relativePath }
            };
            bundleBuilds.Add(abb);
            assetPackList.Add(bundleName);
        }

        // Build all bundles
        BuildPipeline.BuildAssetBundles(outputBundlePath, bundleBuilds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.Android);

        // Move bundles into PAD structure
        foreach (string bundleName in assetPackList)
        {
            string srcBundle = Path.Combine(outputBundlePath, bundleName);
            string destFolder = Path.Combine(assetPacksRoot, bundleName, "assets");
            Directory.CreateDirectory(destFolder);
            string destPath = Path.Combine(destFolder, bundleName);
            File.Copy(srcBundle, destPath, true);
        }

        // Generate AssetPackConfig.json
        GenerateAssetPackConfig(assetPackList);

        Debug.Log("Conversion to PAD complete!");
    }

    static void GenerateAssetPackConfig(List<string> bundleNames)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(assetPackConfigPath));

        var entries = new List<string>();
        foreach (string name in bundleNames)
        {
            entries.Add($"    {{\n      \"assetPackName\": \"{name}\",\n      \"deliveryMode\": \"install-time\"\n    }}");
        }

        string json = "{\n  \"assetPacks\": [\n" + string.Join(",\n", entries) + "\n  ]\n}";
        File.WriteAllText(assetPackConfigPath, json);

        AssetDatabase.Refresh();
        Debug.Log("AssetPackConfig.json generated.");
    }
}
