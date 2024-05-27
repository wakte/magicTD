using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XFABManager;
using XFFSM;
using XFGameFramework;

public class GameState_Ready : FSMState
{
    Module module;
    //声明加载中的界面
    private LoadingPanel loadingPanel;

    private FightController fightController => module.LoadController<FightController>();
    private GameController gameController => module.LoadController<GameController>();
    private FightPanel fightPanel;
    public override void OnEnter()
    {
        base.OnEnter();
        module = userData as Module;

        if (module == null)
        {
            throw new System.Exception("参数异常");
        }

        Debug.LogFormat("GameState_Ready OnEnter,关卡ID：{0}", module.LoadController<GameController>().GetCurrentPlayLevelId());
        //启用协程
        controller.StartCoroutine(LoadingLevel());
        LogUtil.Log("加载成功");
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("GameState_Ready OnExit");
    } 

    //使用协程进行场景加载
    private IEnumerator LoadingLevel()
    {
        //加载场景
        int nowLevelId = module.LoadController<GameController>().GetCurrentPlayLevelId();
        //获取当前场景配表
        LevelInfo levelInfo = module.LoadController<LevelsController>().GetLevelInfo(nowLevelId);

        LogUtil.Log(string.Format("正在加载关卡：{0}", nowLevelId+1));

        if (levelInfo == null)
        {
            throw new System.Exception(string.Format("关卡配置查询失败：{0}", nowLevelId));
        }

        bool finish =false;
        //异步加载关卡

        //因为需要在两个场景中切换，所以需要使用UIType.DontDestroyUI保证过程中ui不被销毁
        loadingPanel = module.LoadPanel<LoadingPanel>(UIType.DontDestroyUI);
        module.LoadController<SceneController>().LoadScene(levelInfo.sceneName, () => { 
            finish = true; 
            loadingPanel.UpdateProgress(1); }, 
            (p) => {

            loadingPanel.UpdateProgress(p);
        });

        while (!finish || (loadingPanel != null && !loadingPanel.isDone()))
        {
            yield return null;  
        }

        if (fightPanel != null)
        {
            fightPanel.Close();
            fightPanel = null;
        }
        //加载战斗UI
        fightPanel = module.LoadPanel<FightPanel>();

        //开始战斗按钮初始化
        Scene scene = SceneManager.GetSceneByName(levelInfo.sceneName);
        fightController.InitScene(scene);

        //加载第一波敌人的战斗按钮
        EnemyWaves waves = levelInfo.enemyWaves[0];//定位第一波敌人
        FightButton button = fightController.GetFightButton(waves.fight_buttton);
        button.Show();
        button.UpdateProgress(1);
        button.AddClick(() =>
        {
            Debug.Log("点击开始战斗");
            gameController.ControlGameState(GameState.Gaming_Fight);
            button.Hide();
        });

        //在加载完成后关闭加载界面
        if (loadingPanel != null)
        {
            loadingPanel.Close();
            loadingPanel = null;    
        }

        // 播放背景音乐
        module.LoadController<AudiosController>().PlayBGM(levelInfo.bgm);
    }

}
