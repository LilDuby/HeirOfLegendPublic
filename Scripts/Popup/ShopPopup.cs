

public class ShopPopup : PopupUI
{
    public void OnClickBack()
    { 
        UIManager.instance.HideUI<ShopPopup>();
    }
}
