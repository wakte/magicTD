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

        LogUtil.Log("�Ѿ�����ս��״̬��������");

        module = userData as Module;
        if (module == null)
        {
            throw new System.Exception("�����쳣!");
        }
        //�������ɵ��˵�Э��
        generateEnemyCoroutine = CoroutineStarter.Start(fightController.GenerateEnemy());

    }
    //
    public override void OnExit()
    {
        base.OnExit();
        if (generateEnemyCoroutine != null)
        {
            CoroutineStarter.Stop(generateEnemyCoroutine);//�˳�ʱֹͣЭ��
        }
            
    }
}
