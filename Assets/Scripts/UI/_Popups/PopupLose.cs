using UnityEngine;

public class PopupLose : SSController
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

    public void OnBtnRetryClicked()
    {
        SSSceneManager.Instance.Close();
        GameManager.Instance.LoadGame();
    }
}
