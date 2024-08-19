public class TutorialPopup : PopupUI
{
    public void OnTutorialInfoPopup()
    {        
        UIManager.instance.HideUI<TutorialPopup>();
        GameManager.instance.isTutorial=true;
        GameManager.instance.OnTutorial();
    }

    public void OnClickBack()
    { 
        UIManager.instance.HideUI<TutorialPopup>();
    }
}
