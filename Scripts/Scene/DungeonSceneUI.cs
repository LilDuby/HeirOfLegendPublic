using UnityEngine;

public class DungeonSceneUI : SceneUI
{
    DataManager DM;

    private void Start() 
    {
        DM=DataManager.instance;
    }

public void OnShopPopup()
    {
        UIManager.instance.ShowUI<ShopPopup>(UIs.Popup);
        UIManager.instance.ShowUI<InventoryPopup>(UIs.Popup);
    }
    
    public void OnForgePopup()
    {
        UIManager.instance.ShowUI<ForgePopup>(UIs.Popup);
        UIManager.instance.ShowUI<InventoryPopup>(UIs.Popup);
    }

    public void OffShopPopup()
    { 
        UIManager.instance.HideUI<ShopPopup>();
        UIManager.instance.HideUI<InventoryPopup>();
    }

    public void OffForgePopup()
    { 
        UIManager.instance.HideUI<ForgePopup>();
        UIManager.instance.HideUI<InventoryPopup>();
    }   
    
    public void OnSaveGame()
    {           
        DM.SaveFileDelet();
        DM.InGameSaveData();
        DM.SaveGame(DM.SaveDataNum);
    }   
}
