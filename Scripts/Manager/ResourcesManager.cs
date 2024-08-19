using System.Collections.Generic;
using UnityEngine;

public enum UIs
{
    Popup,
    Scene,
}

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager instance;
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

    private Dictionary<string, BaseUI> uiDictionary = new Dictionary<string, BaseUI>();
    
    public GameObject GetUI(UIs type, string uiName)
    {
        return Resources.Load<GameObject>($"UI/{type.ToString()}/{uiName}");
    }

    public BaseUI LoadUI<T>(UIs type)
    {
        string uiName = typeof(T).Name;
        BaseUI ui = Instantiate(Resources.Load<BaseUI>($"UI/{type.ToString()}/{uiName}"));
        ui.gameObject.SetActive(false);
        return ui;
    }

    public BaseUI GetUIInDic(string uiName)
    {    
        return uiDictionary[uiName];
    }

    public bool CheckUIDictionary(string uiName)
    {
        return uiDictionary.ContainsKey(uiName);
    }

    public void AddUIInDic(string uiName, BaseUI obj)
    {
        uiDictionary.Add(uiName, obj);
    }
    public void RemoveUIInDic(string uiName)
    {
        uiDictionary.Remove(uiName);
    }

    public void InitializeDicValue(string uiName, BaseUI obj)
    {
        uiDictionary[uiName] = obj;
    }

    public void ClearDic()
    {
        uiDictionary.Clear();
    }

    public bool IsUIValueNull(string uiName)
    {
        if (uiDictionary.ContainsKey(uiName))
        {
            return uiDictionary[uiName] == null;
        }
        return true;
    }

    public bool IsUIValueExist(string uiName)
    {
        return CheckUIDictionary(uiName) && IsUIValueNull(uiName) == false;
    }

}
