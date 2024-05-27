using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;

public class PausePanel : Panel
{

    [SerializeField]
    private Toggle tog_music;
    [SerializeField]
    private Toggle tog_sound;


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);

        tog_music.isOn = Module.LoadController<AudiosController>().GetMute(AudioType.BGM);
        tog_sound.isOn = Module.LoadController<AudiosController>().GetMute(AudioType.Sound);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Time.timeScale = 0;//设置系统时间尺度
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Time.timeScale = 1;
    }


    public void OnBtnReplayClick() { 
       Module.LoadController<FightController>().Replay();
       Close();
    }

    public void OnBtnExitClick() 
    {
        Module.LoadController<GameController>().ControlGameState(GameState.SelectLevel);
    }


    public void OnTogMusicValueChange(bool isOn) {
        Module.LoadController<AudiosController>().SetMute(AudioType.BGM,isOn);
    }

    public void OnTogSoundValueChange(bool isOn) {
       Module.LoadController<AudiosController>().SetMute(AudioType.Sound, isOn);
    }



}
