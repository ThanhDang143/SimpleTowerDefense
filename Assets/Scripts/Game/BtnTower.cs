using System.Collections;
using System.Collections.Generic;
using BakingSheetImpl;
using UnityEngine;
using UnityEngine.UI;

public class BtnTower : MonoBehaviour
{
    [Space]
    [SerializeField] private Image imgIcon;

    private Tower towerData;

    public void Setup(Tower _towerData)
    {
        towerData = _towerData;
        LoadIcon();
    }

    private void LoadIcon()
    {
        imgIcon.sprite = towerData.Icon.Get<Sprite>();
        imgIcon.SetNativeSize();
    }

    public void OnClicked()
    {
        GameManager.Instance.OnTowerSelected(towerData);
    }
}
