using System.Collections;
using System.Collections.Generic;
using BakingSheetImpl;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGame : SSController
{
    [Space]
    [SerializeField] private TextMeshProUGUI txtWave;
    [SerializeField] private TextMeshProUGUI txtGameState;
    [SerializeField] private RectTransform txtCoin;
    [SerializeField] private RectTransform txtHealth;

    [Space]
    [SerializeField] private Button btnReady;

    [Space]
    [SerializeField] private RectTransform towerBtnContainer;

    [Space]
    [SerializeField] private BtnTower btnTowerPrefab;

    public override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.LoadGame();
        LoadTowerInLevel();
        OnWaveChanged();

        AddNoti();
    }

    private void AddNoti()
    {
        NotificationService.Instance.Add(GloblaConstants.Noti.ON_UPDATE_WAVE, OnWaveChanged);
        NotificationService.Instance.Add(GloblaConstants.Noti.ON_UPDATE_GAME_STATE, OnGameStateChanged);
        NotificationService.Instance.Add(GloblaConstants.Noti.NOT_ENOUGH_COIN, OnNotEnoughCoin);
        NotificationService.Instance.Add(GloblaConstants.Noti.ON_LOAD_NEXT_LEVEL, LoadTowerInLevel);
    }

    private void RemoveNoti()
    {
        NotificationService.Instance.Remove(GloblaConstants.Noti.ON_UPDATE_WAVE, OnWaveChanged);
        NotificationService.Instance.Remove(GloblaConstants.Noti.ON_UPDATE_GAME_STATE, OnGameStateChanged);
        NotificationService.Instance.Remove(GloblaConstants.Noti.NOT_ENOUGH_COIN, OnNotEnoughCoin);
        NotificationService.Instance.Remove(GloblaConstants.Noti.ON_LOAD_NEXT_LEVEL, LoadTowerInLevel);
    }

    private void LoadTowerInLevel()
    {
        Utilities.ClearChildren(towerBtnContainer);

        List<TowerSheet.Reference> towerSheet = GameManager.Instance.GetLevelData().Towers;
        for (int i = 0; i < towerSheet.Count; i++)
        {
            Tower tower = towerSheet[i].Ref;
            BtnTower btnTower = Instantiate(btnTowerPrefab, towerBtnContainer);
            btnTower.gameObject.name = tower.Name;
            btnTower.Setup(tower);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        RemoveNoti();
    }

    private void OnWaveChanged()
    {
        txtWave.text = "Wave " + (GameManager.Instance.GetCurWave() + 1).ToString();
    }

    public void OnGameStateChanged()
    {
        txtGameState.text = GameManager.Instance.GetGameState().ToString();
        btnReady.gameObject.SetActive(GameManager.Instance.GetGameState() == GameState.PREPARE);
    }

    private void OnNotEnoughCoin()
    {
        txtCoin.DOShakeAnchorPos(0.5f, 10, 90);
    }

    #region Buttons Aciton
    public void OnBtnPauseClicked()
    {
        SSSceneManager.Instance.PopUp(PopupNames.PAUSE);
    }

    public void OnBtnReadyClicked()
    {
        GameManager.Instance.OnReady();
    }

    public void OnBtnRemoveTowerPreviewClicked()
    {
        GameManager.Instance.OnRemoveTowerPreview();
    }
    #endregion
}
