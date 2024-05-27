using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;
using XFFSM;
public class GaneState_SelectLevel : FSMState
{

    private SelectLevelPanel selectLevel;

    private LoadingPanel loadingPanel;

    public override void OnEnter()
    {
        base.OnEnter();
        Module module = userData as Module;
        if (module == null) throw new System.Exception("参数异常!");


        module.LoadController<SceneController>().LoadScene("Start", () => {
            selectLevel = module.LoadPanel<SelectLevelPanel>();
            if (loadingPanel != null)
            {
                TimerManager.DelayInvoke(() => {//让界面延迟关闭
                    loadingPanel.Close();
                    loadingPanel = null;
                }
                , 1);
            }

        }, (p) => {

            if (loadingPanel == null)
            {
                loadingPanel = module.LoadPanel<LoadingPanel>(UIType.DontDestroyUI);//采用DontDestroyUI确保UI不会被销毁
            }

            loadingPanel.UpdateProgress(p);
        });


        // 播放背景音乐
        module.LoadController<AudiosController>().PlayBGM(AudioConst.bg_main);
    }

    public override void OnExit()
    {
        base.OnExit(); 
        if (selectLevel != null)
        {
            selectLevel.Close();
            selectLevel = null; 
        }
    }

}
