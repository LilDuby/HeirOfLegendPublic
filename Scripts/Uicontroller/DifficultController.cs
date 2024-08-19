using TMPro;
using UnityEngine;

[System.Serializable]
public class Difficulty
{  
    public int difficultNum;
    public string info;
    public string rewerd;
}

public class DifficultController : MonoBehaviour
{
    public GameManager GM;
    public int difficultNum;
    public TextMeshProUGUI difficultNumTxt;
    public TextMeshProUGUI difficultInfoTxt;
    public TextMeshProUGUI difficultRewerdTxt;

    private void Start()
    {
        GM=GameManager.instance;
        difficultNum=GM.difficult;
        ShowDifficultNum(0);
    }

    public void OnDifficultBtn(int num)
    {
        if(num==0) //up
        {
            if(difficultNum<GM.Difficulty.Count-1)
            {
                difficultNum++;                
            }
            ShowDifficultNum(difficultNum);
        }
        else if(num==1)//down
        {
            if(difficultNum>0)
            {
                difficultNum--;                
            }
            ShowDifficultNum(difficultNum);
        }
    }

    void ShowDifficultNum(int num)
    {   
        string info="";
        GM.difficult=num;
        difficultNumTxt.text=GM.difficult.ToString();
        difficultRewerdTxt.text=GM.Difficulty[num].rewerd;
        if(num!=0)
        {
            if(num>=1)
            {
                info=GM.Difficulty[1].info;
            }
            if(num>=2)
            {
                string modStat = GM.Difficulty[2].info;
                if (num>=4)
                modStat+="+";
                if(num>=6)
                modStat+="+";

                info+="\n"+modStat;
            }
            if(num>=3)
            {
                string dropRate = GM.Difficulty[3].info;
                if (num>=7)
                dropRate+="+";

                info+="\n"+dropRate;
            }
            if(num>=5)
            {
                string bossSpawn = GM.Difficulty[5].info;
                if (num>=8)
                bossSpawn+="+";

                info+="\n"+bossSpawn;
            }
        }
        difficultInfoTxt.text=info;
    }
}
