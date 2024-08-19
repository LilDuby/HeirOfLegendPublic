using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionPopup : PopupUI
{   
    public TextMeshProUGUI onOffTxt;
    public Slider SFXSlider;
    public Slider BGMSlider;
    
    void Start() 
    {
        onOffTxt.GetComponent<TextMeshProUGUI>();         
    }

    public void BGMVolume()
    {        
        float sound = BGMSlider.value;
        AudioManager.instance.BGMAudioControl(sound);
    }

    public void SFXVolume()
    {        
        float sound = SFXSlider.value;        
        AudioManager.instance.SFXVolControl(sound);
    }

    public void OnToggleVolume()
    {
        ToggleVolume();
    }    

    public void ToggleVolume()
    {
        if(AudioListener.volume==0)
        {
            AudioListener.volume= 1;
            onOffTxt.text="ON";
        }
        else
        {
            AudioListener.volume= 0;
            onOffTxt.text="Off";
        }
    }
    public void OnClickBack()
    {         
        UIManager.instance.HideUI<OptionPopup>();
    }
}
