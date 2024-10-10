using BakingSheetImpl;
using UnityEngine;

public class SceneManager : SSSceneManager
{
    protected override void OnFirstSceneLoad()
    {
        base.OnFirstSceneLoad();

        // Load GameData
        DataManager.Instance.InitialData();
    }
}
