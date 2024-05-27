using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;

public class SettingPanel : Panel
{

    [SerializeField]
    public Toggle tog_sound;
    [SerializeField]
    public Toggle tog_music;


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);
        tog_sound.isOn = !Module.LoadController<AudiosController>().GetMute(AudioType.Sound);
        tog_music.isOn = !Module.LoadController<AudiosController>().GetMute(AudioType.BGM);
    }



    public void OnTogSoundValueChange(bool isOn) {
        Module.LoadController<AudiosController>().SetMute(AudioType.Sound,!isOn);
    }

    public void OnTogMusicValueChange(bool isOn) {
        Module.LoadController<AudiosController>().SetMute(AudioType.BGM, !isOn);
    }


}
