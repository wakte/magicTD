using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFFSM;
using XFGameFramework;


public class GameController : Controller
{

    //���õ�ǰ�浵�±�
    public void SetFileIndex(int index)
    {
        GameModel model = GetGameModel();

        model.fileIndex=index;
        //LogUtil("��ǰ�浵ID:{0}", model.fileIndex);

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
    //����״̬���ı�
    public void ControlGameState(GameState state)
    {
        //��ȡ״̬���ֵ�״̬
        GetGameModel().state = state;
        LogUtil.Log("����״̬��");
        //��ȡGameState״̬��
        FSMController fsm1 = FSMController.GetFSM("GameState");

        if (fsm1==null)
        {
            RuntimeFSMController fsm = AssetBundleManager.LoadAsset<RuntimeFSMController>(Module.ProjectName, "GameState");
            fsm1=FSMController.StartupFSM("GameState", fsm, Module);
        }

        fsm1.SetInt("state", (int)state);
    }

    public void SetCurrentPlayLevelId(int levelId)//������������ؿ���id
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
        // �޸���Ϸ״̬
        ControlGameState(GameState.Gaming_Ready);

    }

}
