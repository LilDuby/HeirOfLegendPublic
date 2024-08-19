using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int curSortingOrder = 10;

    public void ShowUI<T>(UIs type)
    {
        string uiName = typeof(T).Name;

        if (ResourcesManager.instance.CheckUIDictionary(uiName))
        {
            BaseUI ui = ResourcesManager.instance.GetUIInDic(uiName);

            ui.gameObject.SetActive(true);

            return;
        }
        
        BaseUI obj = Instantiate(Resources.Load<BaseUI>($"UI/{type.ToString()}/{uiName}"));
        obj.SetOrder(curSortingOrder++);
        obj.gameObject.SetActive(true);

        ResourcesManager.instance.AddUIInDic(uiName, obj);
    }

    public void HideUI<T>()
    {
        string uiName = typeof(T).Name;
        BaseUI ui = ResourcesManager.instance.GetUIInDic(uiName);
        ui.gameObject.SetActive(false);
        curSortingOrder--;
        curSortingOrder=Mathf.Clamp(curSortingOrder, 11, 20);
    }

    public GameObject FindUI<T>(UIs type)
    {
        string uiName = typeof(T).Name;
        if (ResourcesManager.instance.CheckUIDictionary(uiName))
        {            
            BaseUI ui = ResourcesManager.instance.GetUIInDic(uiName);

            return ui.gameObject;
        }        
        return null;
    }

    public void ToggleUI<T>(UIs type)
    {
        string uiName = typeof(T).Name;

        if (ResourcesManager.instance.CheckUIDictionary(uiName) && ResourcesManager.instance.IsUIValueNull(uiName) == false)
        {
            BaseUI ui = ResourcesManager.instance.GetUIInDic(uiName);
            if (ui.gameObject.activeSelf)
            {
                HideUI<T>();
                return;
            }
            else
            {
                ShowUI<T>(type);
                return;
            }
        }
        else
        {
            BaseUI obj = Instantiate(Resources.Load<BaseUI>($"UI/{type.ToString()}/{uiName}"));
            obj.SetOrder(curSortingOrder++);
            obj.gameObject.SetActive(true);
            ResourcesManager.instance.InitializeDicValue(uiName, obj);
        }
        
    }
    
    public void RemoveDocUI<T>()
    {
        string uiName = typeof(T).Name;
        BaseUI ui = ResourcesManager.instance.GetUIInDic(uiName);
        ui.gameObject.SetActive(false);
        curSortingOrder--;
        ResourcesManager.instance.RemoveUIInDic(uiName);
    }
}
