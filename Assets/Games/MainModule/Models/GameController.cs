using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFFSM;
using XFGameFramework;


public class GameController : Controller
{

    //设置当前存档下标
    public void SetFileIndex(int index)
    {
        GameModel model = GetGameModel();

        model.fileIndex=index;
        //LogUtil("当前存档ID:{0}", model.fileIndex);

    }
    //
    public int GetOnFileIndex()
    {
        return GetGameModel().fileIndex;
    }

    public GameModel GetGameModel()
    {
        GameModel model = Module.GetModel<GameModel>();
        if (model == null)
        {
            model = new GameModel();
            Module.AddModel(model);
        }
        return model;
    }
    //控制状态机改变
    public void ControlGameState(GameState state)
    {
        //获取状态机种的状态
        GetGameModel().state = state;
        LogUtil.Log("启动状态机");
        //获取GameState状态机
        FSMController fsm1 = FSMController.GetFSM("GameState");

        if (fsm1==null)
        {
            RuntimeFSMController fsm = AssetBundleManager.LoadAsset<RuntimeFSMController>(Module.ProjectName, "GameState");
            fsm1=FSMController.StartupFSM("GameState", fsm, Module);
        }

        fsm1.SetInt("state", (int)state);
    }

    public void SetCurrentPlayLevelId(int levelId)//设置正在游玩关卡的id
    {
        GetGameModel().currentPlayingLevelId = levelId;
    }

    public int GetCurrentPlayLevelId() 
    {
        return GetGameModel().currentPlayingLevelId;
    }

    public bool IsHaveNextLevel()
    {
        int levelId = GetCurrentPlayLevelId();
        levelId++;
        return Module.LoadController<LevelsController>().GetLevelInfo(levelId) != null;
    }

    public void NextLevel()
    {
        if (!IsHaveNextLevel()) return;
        int levelId = GetCurrentPlayLevelId();
        SetCurrentPlayLevelId(levelId + 1);
        // 修改游戏状态
        ControlGameState(GameState.Gaming_Ready);

    }

}
