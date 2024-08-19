

using UnityEngine;

public class NewGameCheckPopup : PopupUI
{
    public void OnClickOpen()
    {
        OnSelectPopup();
    }
    
    public void OnSelectPopup()
    {
        UIManager.instance.ShowUI<SelectPopup>(UIs.Popup);
        GameManager.instance.OnplayTutorial();
    }

    public void OnClickClose()
    {         
        UIManager.instance.HideUI<NewGameCheckPopup>();
        UIManager.instance.HideUI<LoadDataPopup>();
    
        GameObject Popup = UIManager.instance.FindUI<SaveInfoPopup>(UIs.Popup);
        if(Popup!=null && Popup.activeSelf)
            UIManager.instance.HideUI<SaveInfoPopup>();
    }

    public void OnClickBack()
    { 
        if(!GameManager.instance.isTutorial)
             UIManager.instance.HideUI<NewGameCheckPopup>();
    }
}
