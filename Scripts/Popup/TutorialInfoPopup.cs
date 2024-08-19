using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialInfoPopup : PopupUI
{
    public TextMeshProUGUI infoTxt;    
    Vector3 popupPos;
    Transform children;
    public List<Dictionary<string, object>> data_Dialog;    
    int chapterNum=1;
    string chapter="Chapter";
    string content="Content";
    string vector="VectorX";

    void Start()
    {
        data_Dialog = CSVReader.Read("csv/TutorialInfo2");        
        children=transform.GetChild(0);
        popupPos=children.localPosition;
        Initiailzer();
    }
    
    public void Initiailzer() 
    { 
        infoTxt.text=InfoTxt(GameManager.instance.tutoNum);
        children.localPosition=popupPos;
    }

    string InfoTxt(int _tutoNum) 
    {
        string info="";    

        for (int i = 0; i < data_Dialog.Count; i++)
        {   
            if(chapterNum==int.Parse(data_Dialog[i][chapter].ToString()))
            {
                if(i==_tutoNum)
                {
                    info=data_Dialog[i][content].ToString();

                    string posTxt = data_Dialog[i][vector].ToString();                    
                    if(posTxt!="")
                    {
                        popupPos.x=int.Parse(posTxt);
                    }
                }
            }
        }
        return info;
    }

    public void ChengChapter(int _chapterNum)
    {
        chapterNum=_chapterNum;
    }
}
