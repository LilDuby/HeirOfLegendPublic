using UnityEngine;

public class TitleSceneUI : SceneUI
{   
    public void OnLoadDataPopup()
    {
        UIManager.instance.ShowUI<LoadDataPopup>(UIs.Popup);
        GameManager.instance.OnplayTutorial();        
    }

    public void OnOptionPopup()
    {
        UIManager.instance.ShowUI<OptionPopup>(UIs.Popup);
    }

    public void OnTutorialPopup()
    {
        UIManager.instance.ShowUI<TutorialPopup>(UIs.Popup);
    }
    public void Quit()
    {
        Application.Quit();//game out
    }
}
