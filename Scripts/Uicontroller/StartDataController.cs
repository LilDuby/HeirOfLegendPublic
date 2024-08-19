using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartDataController : MonoBehaviour
{
    public DataManager DM;
    public Image classImg;
    public TextMeshProUGUI classNameTxt;
    public TextMeshProUGUI classInfoTxt;
    public TextMeshProUGUI maxHpTxt;
    public TextMeshProUGUI speedTxt;
    public TextMeshProUGUI atkTxt;
    public TextMeshProUGUI defTxt;
    public TextMeshProUGUI goldTxt;
    
    private void Start()
    {
        DM=DataManager.instance;
    }

    void OnEnable()
    {
        ShowData();
    }

    void OnDisable()
    {
        ClearData();
    }

    public void ShowData()
    {   
        StartCoroutine(DelayShowData());        
    }
    
    IEnumerator DelayShowData()
    {
        yield return new WaitForSeconds(0.1f);
        maxHpTxt.text=DM.stat.maxHealth.ToString();
        speedTxt.text=DM.stat.speed.ToString("N2");
        atkTxt.text=DM.stat.power.ToString();
        defTxt.text=DM.stat.defense.ToString();
        goldTxt.text="Gold: "+DM.saveData.Gold.ToString();
        classImg.sprite=DM.classList[DM.saveData.ClassNum].classImg;
        classNameTxt.text=DM.classList[DM.saveData.ClassNum].className;
        classInfoTxt.text=$"{DM.saveData.Difficulty}난이도\n {DM.saveData.Pos[0]}층 {DM.saveData.Pos[1]}번방\n";
    }
    void ClearData()
    {
        maxHpTxt.text="";
        maxHpTxt.text="";
        speedTxt.text="";
        atkTxt.text="";
        defTxt.text="";
        goldTxt.text="";
        classImg.sprite=null;
        classNameTxt.text="";
        classInfoTxt.text="";
    }

}
    
