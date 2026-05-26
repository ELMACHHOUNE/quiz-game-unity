using UnityEditor;
using UnityEngine;
using UnityEditor.U2D.PSD;

public class PSDSetup
{
    public static void Run()
    {
        string path = "Assets/Resources/CharacterPS-01.psb";
        
        // Force reimport so PSDImporter takes over from TextureImporter
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        var importer = AssetImporter.GetAtPath(path) as PSDImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            
            importer.SaveAndReimport();
            Debug.Log("PSD Importer successfully configured for CharacterPS-01.psd!");
        }
        else
        {
            Debug.LogError("Still failed to get PSDImporter for " + path + ". Got type: " + AssetImporter.GetAtPath(path).GetType().Name);
        }
    }
}
