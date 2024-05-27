using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;
using XFFSM;

public class GameState_Start : FSMState
{
    private StartPanel startPanel;
    public override void OnEnter()
    {
        base.OnEnter();
        LogUtil.Log("Game Start!");
        Module module = userData as Module;
        if (module == null) throw new System.Exception("�����쳣");
        startPanel = module.LoadPanel<StartPanel>();

        // ���ű�������
        module.LoadController<AudiosController>().PlayBGM(AudioConst.bg_main);
    }

    public override void OnExit() { 
        base.OnExit();
        if(startPanel != null)
        {
            startPanel.Close();
            startPanel = null;  
        }
    }

}
