using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
public class ExportAudioLinkPackage : MonoBehaviour
{
    static void CopyArtifacts()
    {
        FileUtil.CopyFileOrDirectory("Assets/Docs", "Assets/AudioLink/Docs");
        FileUtil.CopyFileOrDirectory("Assets/README.md", "Assets/AudioLink/README.md");
        FileUtil.CopyFileOrDirectory("Assets/CHANGELOG.md", "Assets/AudioLink/CHANGELOG.md");
        FileUtil.CopyFileOrDirectory("Assets/LICENSE", "Assets/AudioLink/LICENSE.txt");
        AssetDatabase.Refresh();
    }

    static void CleanupArtifacts()
    {
        FileUtil.DeleteFileOrDirectory("Assets/AudioLink/Docs");
        FileUtil.DeleteFileOrDirectory("Assets/AudioLink/Docs.meta");
        FileUtil.DeleteFileOrDirectory("Assets/AudioLink/README.md");
        FileUtil.DeleteFileOrDirectory("Assets/AudioLink/README.md.meta");
        FileUtil.DeleteFileOrDirectory("Assets/AudioLink/CHANGELOG.md");
        FileUtil.DeleteFileOrDirectory("Assets/AudioLink/CHANGELOG.md.meta");
        FileUtil.DeleteFileOrDirectory("Assets/AudioLink/LICENSE.txt");
        FileUtil.DeleteFileOrDirectory("Assets/AudioLink/LICENSE.txt.meta");
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Export AudioLink Packages")]
    static void ExportPackageAudioLink()
    {

        string version = new StreamReader("Assets/AudioLink/VERSION.txt").ReadLine();

        CleanupArtifacts();
        CopyArtifacts();

        var exportedPackageAssetList = new List<string>();
        exportedPackageAssetList.Add("Assets/AudioLink");
        exportedPackageAssetList.Add("Assets/AudioLink/Docs");
        exportedPackageAssetList.Add("Assets/AudioLink/CHANGELOG.md");

        AssetDatabase.ExportPackage(exportedPackageAssetList.ToArray(), $"AudioLink-CVR-{version}-full.unitypackage",
            ExportPackageOptions.Recurse );

        var exportedMinimal = new List<string>();
        exportedMinimal.Add( "Assets/AudioLink/AudioLink.prefab" );
        exportedMinimal.Add( "Assets/AudioLink/AudioLinkController.prefab" );
        exportedMinimal.Add( "Assets/AudioLink/AudioLinkMiniPlayer.prefab" );
        exportedMinimal.Add( "Assets/AudioLink/AudioLinkAvatar.prefab" );
        exportedMinimal.Add( "Assets/AudioLink/LICENSE.txt" );
        exportedMinimal.Add( "Assets/AudioLink/README.md" );
        exportedMinimal.Add( "Assets/AudioLink/VERSION.txt" );
        exportedMinimal.Add( "Assets/AudioLink/Scripts" );
        exportedMinimal.Add( "Assets/AudioLink/RenderTextures" );
        exportedMinimal.Add( "Assets/AudioLink/Shaders" );
        exportedMinimal.Add( "Assets/AudioLink/Materials" );
        exportedMinimal.Add( "Assets/AudioLink/Resources" );
        exportedMinimal.Add( "Assets/AudioLink/CHANGELOG.md" );

        AssetDatabase.ExportPackage(exportedMinimal.ToArray(), $"AudioLink-CVR-{version}-minimal.unitypackage",
            ExportPackageOptions.Recurse );

        CleanupArtifacts();
    }
}
#endif
