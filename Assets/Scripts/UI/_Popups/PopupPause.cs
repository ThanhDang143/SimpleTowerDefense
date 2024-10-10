public class PopupPause : SSController
{
    public override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.PauseGame();
    }

    public void OnBtnHomeClicked()
    {
        SSSceneManager.Instance.Close();
        SSSceneManager.Instance.Screen(ScreenNames.HOME);
    }

    public override void OnKeyBack()
    {
        base.OnKeyBack();
        GameManager.Instance.ResumeGame();
    }
}
