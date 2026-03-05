using UnityEditor;
public class BundleBuilder {
    [MenuItem("Tools/Build AssetBundles")]
    static void BuildAll() {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
// This code defines a Unity editor script that adds a menu item to build asset bundles for the Android platform.
// When the menu item "Tools/Build AssetBundles" is clicked, it triggers the Build
// Pipeline to create asset bundles in the specified directory ("Assets/AssetBundles/Android").
// The BuildAssetBundleOptions.None indicates that no special options are applied during the build process.
// The BuildTarget.Android specifies that the asset bundles are being built for the Android platform.
// This script is useful for developers who want to package their assets for distribution on Android devices.
// To use this script, save it in a folder named "Editor" within your Unity project.
// The script will automatically appear in the Unity editor under the "Tools" menu.
// Make sure to have the necessary permissions and configurations set up in your Unity project for asset bundle building.