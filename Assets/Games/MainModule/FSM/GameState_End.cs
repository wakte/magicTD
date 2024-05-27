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
            // 游戏胜利

            // 保存过关数据
            module.LoadController<OnFlieController>().SaveCurrentPassLevelInfo();
            module.LoadPanel<WinPanel>();

            // 播放游戏胜利的音效
            module.LoadController<AudiosController>().PlaySound(AudioConst.sound_victory);

        }
        else
        {
            // 失败了
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
