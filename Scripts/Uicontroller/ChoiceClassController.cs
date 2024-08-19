using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceClassController : MonoBehaviour
{
    GameManager GM; 
    DataManager DM;   
    public Image chioceClassImg;
    public TextMeshProUGUI classNameTxt;
    public TextMeshProUGUI classInfoTxt;    

    void Start()
    {
        GM=GameManager.instance; 
        DM=DataManager.instance;
        ChangeClassImgs(GM.playerClassIndex);
    }
    public void OnChioceClass(int num)
    {
        GM.playerClassIndex += (num == 0) ? 1 : -1;
        
        if(GM.playerClassIndex==DM.classList.Count)
            GM.playerClassIndex=0;

        else if(GM.playerClassIndex<0)
            GM.playerClassIndex=DM.classList.Count-1;
        
        ChangeClassImgs(GM.playerClassIndex);
        DM.saveData.ClassNum = GM.playerClassIndex;
    }

    void ChangeClassImgs(int num)
    {
        chioceClassImg.sprite=DM.classList[num].classImg;
        classInfoTxt.text = DM.classList[num].classInfo;
        classNameTxt.text = DM.classList[num].className;
    }
}
