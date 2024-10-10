using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

public class MapCreator : OdinMenuEditorWindow
{

    [MenuItem("Tools/MapCreator &M")]
    private static void OpenWindow()
    {
        GetWindow<MapCreator>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();

        tree.Add("Create New Map", new CreateNewMap());
        tree.AddAllAssetsAtPath("Maps", "Assets/_Assets/Maps", typeof(Map));

        return tree;
    }

    public class CreateNewMap
    {
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public Map map;

        public CreateNewMap()
        {
            map = CreateInstance<Map>();
            map.mapName = "Map_" + Random.Range(0, 1000).ToString("000");
        }

        [Button("Create Map", ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void Create()
        {
            string path = "Assets/_Assets/Maps/" + map.mapName + ".asset";
            AssetDatabase.CreateAsset(map, path);

            // Enable Addressable
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.DefaultGroup;
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group);
            entry.address = map.mapName;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Map created!");
        }
    }
}
