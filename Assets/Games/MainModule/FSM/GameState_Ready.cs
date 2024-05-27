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
    //���������еĽ���
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
            throw new System.Exception("�����쳣");
        }

        Debug.LogFormat("GameState_Ready OnEnter,�ؿ�ID��{0}", module.LoadController<GameController>().GetCurrentPlayLevelId());
        //����Э��
        controller.StartCoroutine(LoadingLevel());
        LogUtil.Log("���سɹ�");
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("GameState_Ready OnExit");
    } 

    //ʹ��Э�̽��г�������
    private IEnumerator LoadingLevel()
    {
        //���س���
        int nowLevelId = module.LoadController<GameController>().GetCurrentPlayLevelId();
        //��ȡ��ǰ�������
        LevelInfo levelInfo = module.LoadController<LevelsController>().GetLevelInfo(nowLevelId);

        LogUtil.Log(string.Format("���ڼ��عؿ���{0}", nowLevelId+1));

        if (levelInfo == null)
        {
            throw new System.Exception(string.Format("�ؿ����ò�ѯʧ�ܣ�{0}", nowLevelId));
        }

        bool finish =false;
        //�첽���عؿ�

        //��Ϊ��Ҫ�������������л���������Ҫʹ��UIType.DontDestroyUI��֤������ui��������
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
        //����ս��UI
        fightPanel = module.LoadPanel<FightPanel>();

        //��ʼս����ť��ʼ��
        Scene scene = SceneManager.GetSceneByName(levelInfo.sceneName);
        fightController.InitScene(scene);

        //���ص�һ�����˵�ս����ť
        EnemyWaves waves = levelInfo.enemyWaves[0];//��λ��һ������
        FightButton button = fightController.GetFightButton(waves.fight_buttton);
        button.Show();
        button.UpdateProgress(1);
        button.AddClick(() =>
        {
            Debug.Log("�����ʼս��");
            gameController.ControlGameState(GameState.Gaming_Fight);
            button.Hide();
        });

        //�ڼ�����ɺ�رռ��ؽ���
        if (loadingPanel != null)
        {
            loadingPanel.Close();
            loadingPanel = null;    
        }

        // ���ű�������
        module.LoadController<AudiosController>().PlayBGM(levelInfo.bgm);
    }

}
