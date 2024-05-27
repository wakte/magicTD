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
        if (module == null) throw new System.Exception("�����쳣!");


        module.LoadController<SceneController>().LoadScene("Start", () => {
            selectLevel = module.LoadPanel<SelectLevelPanel>();
            if (loadingPanel != null)
            {
                TimerManager.DelayInvoke(() => {//�ý����ӳٹر�
                    loadingPanel.Close();
                    loadingPanel = null;
                }
                , 1);
            }

        }, (p) => {

            if (loadingPanel == null)
            {
                loadingPanel = module.LoadPanel<LoadingPanel>(UIType.DontDestroyUI);//����DontDestroyUIȷ��UI���ᱻ����
            }

            loadingPanel.UpdateProgress(p);
        });


        // ���ű�������
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
