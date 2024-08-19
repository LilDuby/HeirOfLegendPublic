
using UnityEngine;

public class DeletPopup : PopupUI
{
    GameObject obj;
    LoadDataPopup loadDataPopup;
    void Start()
    {
        obj=UIManager.instance.FindUI<LoadDataPopup>(UIs.Popup);
        loadDataPopup=obj.GetComponent<LoadDataPopup>();
    }

    public void OnDeletOK()
    {   
        GameObject Popup;
        DataManager.instance.OnDeletSaveData();
        DataManager.instance.ResetData();
        loadDataPopup.LoadResetData();
        loadDataPopup.Initiailzer(); //UI창 최신화

        OffDeletPopup();

        Popup = UIManager.instance.FindUI<NewGameCheckPopup>(UIs.Popup);
        if(Popup!=null && Popup.activeSelf)
        {
            UIManager.instance.HideUI<NewGameCheckPopup>();
        }
        
        Popup = UIManager.instance.FindUI<SaveInfoPopup>(UIs.Popup);
        if(Popup!=null && Popup.activeSelf)
        {
            UIManager.instance.HideUI<SaveInfoPopup>();
        }

        DataManager.instance.ResetData();
        
    }
    
    public void OffDeletPopup()
    {
        UIManager.instance.HideUI<DeletPopup>();
    } 
}
