using UnityEditor;
using UnityEngine;
using UnityEditor.U2D.PSD;

public class PSDSetup
{
    public static void Run()
    {
        ConfigurePSB("Assets/Resources/CharacterPS-01.psb", "CharacterPS-01");
        ConfigurePSB("Assets/Resources/character2.psb", "character2");
    }

    static void ConfigurePSB(string path, string label)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        var importer = AssetImporter.GetAtPath(path) as PSDImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;

            importer.SaveAndReimport();
            Debug.Log($"PSD Importer successfully configured for {label}!");
        }
        else
        {
            Debug.LogError($"Still failed to get PSDImporter for {path}. Got type: " + AssetImporter.GetAtPath(path).GetType().Name);
        }
    }
}
