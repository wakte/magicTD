using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFFSM;
using XFGameFramework;

public class GameState_End : FSMState
{
    private Module module;
    public override void OnEnter()
    {
        base.OnEnter();

        module = userData as Module;
        if (module == null) throw new System.Exception("module is null!");

        Time.timeScale = 0;

        FightModel fightModel = module.LoadController<FightController>().GetFightModel();

        if (fightModel.PlayerHp > 0)
        {
            // ��Ϸʤ��

            // �����������
            module.LoadController<OnFlieController>().SaveCurrentPassLevelInfo();
            module.LoadPanel<WinPanel>();

            // ������Ϸʤ������Ч
            module.LoadController<AudiosController>().PlaySound(AudioConst.sound_victory);

        }
        else
        {
            // ʧ����
            module.LoadPanel<FailedPanel>();
        }


    }

    public override void OnExit()
    {
        base.OnExit();
        Time.timeScale = 1;

        module.LoadController<FightController>().Clear();
    }
}
