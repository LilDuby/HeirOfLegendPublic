using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    FullScreenMode screenMode;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenBtn;
    public TextMeshProUGUI toggleTxt;
    List<Resolution> resolutions = new List<Resolution>();
    public int resolutionNum;

    void Start()
    {
        InitUI();
    }


    void InitUI()
    {
        for(int i=0; i<Screen.resolutions.Length; i++)
        {
            if(Screen.resolutions[i].refreshRateRatio.value==60)
            {resolutions.Add(Screen.resolutions[i]);}
        }        

        resolutionDropdown.options.Clear();

        int optionNum=0;
        foreach (Resolution info in Screen.resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text=info.width+"X"+info.height+" "+info.refreshRateRatio.value + "hz";
            resolutionDropdown.options.Add(option);

            if(info.width==Screen.width && info.height==Screen.height)
                {resolutionDropdown.value=optionNum;}
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        fullscreenBtn.isOn=Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow)? true:false;
    }

    public void DropboxOptionChange(int num)
    {
        resolutionNum=num;
    }

    public void FullSceenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow:FullScreenMode.Windowed;
        toggleTxt.text= isFull ? "FullScreen":"Windowed";
    }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,resolutions[resolutionNum].height,screenMode);
    }
}