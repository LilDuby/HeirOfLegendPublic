using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowInfoOnHover : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI infoTxt;  // Start is called before the first frame update
    public List<GameObject> infoObject=new List<GameObject>();
    public string itemInfo;

    private List<string> infoMessages = new List<string>
    {
        "유산이란?\n선대의 지식을\n계승받아 능력치\n상승 가능한 점수",
        "시작시 소지할\n장비를 랜덤으로\n받습니다",
        "최대 체력을 \n5만큼 올릴 수 있습니다.\n(유산 10소모)",
        "공격력을 \n1만큼 올릴 수 있습니다.\n(유산 10소모)",
        "방어력을 \n1만큼 올릴 수 있습니다.\n(유산 10소모)",
        "초기 골드를 \n10만큼 올릴 수 있습니다.\n(유산 10소모)"
    };

    void Start()
    {   
        for (int i = 0; i < infoObject.Count; i++)
        {
            int index = i; 
            EventTrigger trigger = infoObject[i].AddComponent<EventTrigger>();

            // 마우스 엔터 이벤트 추가
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { OnPointerEnter(index); });
            trigger.triggers.Add(entryEnter);

            // 마우스 나가기 이벤트 추가
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { OnPointerExit(); });
            trigger.triggers.Add(entryExit);
        }

        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }    

    public void OnPointerEnter(int index)
    {
        if(infoPanel != null)
        {
            if(index==infoObject.Count-1 && itemInfo!="")
            {   
                infoPanel.SetActive(true);
                infoTxt.text=itemInfo;
            }            
            else if (index >= 0 && index < infoMessages.Count)
            {
                infoPanel.SetActive(true);
                infoTxt.text = infoMessages[index];
            }
        }        
    }

    // 마우스가 이미지에서 나갔을 때 호출되는 메소드
    public void OnPointerExit()
    {
        if (infoPanel != null)        
            infoPanel.SetActive(false);
    }
}
