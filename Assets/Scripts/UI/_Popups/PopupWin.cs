using UnityEngine;

public class PopupWin : SSController
{
    public override void OnEnable()
    {
        base.OnEnable();
    }

    public void OnBtnHomeClicked()
    {
        SSSceneManager.Instance.Close();
        SSSceneManager.Instance.Screen(ScreenNames.HOME);
    }

    public void OnBtnNextClicked()
    {
        SSSceneManager.Instance.Close();
        GameManager.Instance.LoadGame();
        NotificationService.Instance.Post(GloblaConstants.Noti.ON_LOAD_NEXT_LEVEL);
    }
}
