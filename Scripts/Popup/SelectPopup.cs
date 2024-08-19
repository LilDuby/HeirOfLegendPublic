

public class SelectPopup : PopupUI
{
    public void OnClickStart()
    {   
        if(GameManager.instance.isTutorial)        
            UIManager.instance.RemoveDocUI<TutorialInfoPopup>();        
        
        GameManager.instance.StartGame();
    }
    
    public void OnClickBack()
    { 
        if(!GameManager.instance.isTutorial)
            UIManager.instance.HideUI<SelectPopup>();
    }
}
