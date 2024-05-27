using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFGameFramework;


public enum AudioType
{
    BGM,
    Sound

}

public class AudiosController : Controller
{

    public override void OnInit()
    {
        base.OnInit();

        AudioPlayer.RegisterAudioController(AudioType.BGM.ToString(), true);//使用音效文件名来播放音频
        AudioPlayer.RegisterAudioController(AudioType.Sound.ToString(), false);

        AudioPlayer.GetAuidoController(AudioType.BGM.ToString()).Volume = 1;//控制音量大小
        AudioPlayer.GetAuidoController(AudioType.Sound.ToString()).Volume = 1;
    }

    public void PlayBGM(string asset_name)
    {

        AudioSource source = AudioPlayer.GetAuidoController(AudioType.BGM.ToString()).AudioSource;//获取音频
        if (source.clip != null && source.clip.name == asset_name)
        {
            return;
        }

        AudioClip clip = AssetBundleManager.LoadAsset<AudioClip>(Module.ProjectName, asset_name);//加载音频
        if (clip == null)
        {
            return;
        }
        AudioPlayer.Play(AudioType.BGM.ToString(), clip, false);
    }

    public void PlaySound(string asset_name)
    {
        AudioClip clip = AssetBundleManager.LoadAsset<AudioClip>(Module.ProjectName, asset_name);
        AudioPlayer.Play(AudioType.Sound.ToString(), clip, true);
    }

    public void SetMute(AudioType audioType, bool mute)
    {
        AudioPlayer.GetAuidoController(audioType.ToString()).Mute = mute;
    }

    public bool GetMute(AudioType audioType)
    {
        return AudioPlayer.GetAuidoController(audioType.ToString()).Mute;
    }
}
