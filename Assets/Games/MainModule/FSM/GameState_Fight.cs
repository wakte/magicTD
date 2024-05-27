using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFFSM;
using XFGameFramework;

public class GameState_Fight : FSMState
{

    private Module module;

    private FightController fightController => module.LoadController<FightController>();

    private Coroutine generateEnemyCoroutine;

    public override void OnEnter()
    {
        base.OnEnter();

        LogUtil.Log("已经进入战斗状态！！！！");

        module = userData as Module;
        if (module == null)
        {
            throw new System.Exception("参数异常!");
        }
        //启动生成敌人的协程
        generateEnemyCoroutine = CoroutineStarter.Start(fightController.GenerateEnemy());

    }
    //
    public override void OnExit()
    {
        base.OnExit();
        if (generateEnemyCoroutine != null)
        {
            CoroutineStarter.Stop(generateEnemyCoroutine);//退出时停止协程
        }
            
    }
}
