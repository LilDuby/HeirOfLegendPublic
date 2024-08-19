

public class ForgePopup : PopupUI
{
    public void OnClickBack()
    { 
        UIManager.instance.HideUI<ForgePopup>();
    }
}
