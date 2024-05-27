using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using XFABManager;
using XFGameFramework;

public class SelectLevelPanel : Panel
{
    [SerializeField]
    private Transform[] levelTransform;
    [SerializeField]
    private Text star_count;

    private LevelsController levelController=>Module.LoadController<LevelsController>();

    private GameController gameController => Module.LoadController<GameController>();

    private OnFlieController onFlieController => Module.LoadController<OnFlieController>();

    private int currentPage = 0;

    public void OnBtnCloseClick()
    {
        gameController.ControlGameState(GameState.Start);
        Close();
    }
    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);
        InitLevels(1);
    }

    private void InitLevels(int page)
    {

        List<LevelInfo> levelsInfo = levelController.GetLevelsByPages(page, levelTransform.Length);

        foreach (Transform levelInfo in levelTransform)
        {
            levelInfo.gameObject.SetActive(false);
        }

        for (int i = 0; i < levelsInfo.Count; i++)
        {
            Transform levelTrans = levelTransform[i];
            if (levelTrans == null)
            {
                continue;
            }

            levelTrans.gameObject.SetActive(true);

            Text text_name = levelTrans.Find("name").GetComponent<Text>();//��ȡ�ؿ����ڵ�
            Text text_starCount = levelTrans.Find("star/star_count").GetComponent<Text>();//��ȡ������ͳ��
            GameObject obj_lock =levelTrans.Find("lock").gameObject;
            Button btn_startGame= levelTrans.Find("button").GetComponent<Button>();
            int leveId = levelsInfo[i].levelId;

            text_name.text = (leveId + 1).ToString();

            int index = gameController.GetOnFileIndex();//��ȡ�浵�±�
            int star_count = 0;
            OnFileModel model = onFlieController.Get(index);

            if (model.PasssLevels.ContainsKey(leveId))
            {
                star_count = model.PasssLevels[leveId].star;
            }
            text_starCount.text = star_count.ToString()+"/3";//��������״̬


            bool islock = !onFlieController.IsLockLevel(leveId);
            obj_lock.SetActive(islock);

            btn_startGame.onClick.RemoveAllListeners();

            btn_startGame.onClick.AddListener(() =>{//�������첽ִ�еģ������ʹ�þֲ����������ػ����


                if (islock)
                {
                    Module.LoadController<TipController>().ShowToast("�ؿ�δ����");
                    return;
                }
                //�������Ĺؿ�id
                gameController.SetCurrentPlayLevelId(leveId);
                //�л���Ϸ״̬ -> ׼��״̬
                gameController.ControlGameState(GameState.Gaming_Ready);
                //�������ؿ��¼�
                LogUtil.Log("����ؿ�{0}", leveId);
            });

        }

            


    }

    protected override void OnDisable()
    {
        base.OnDisable();
        currentPage = 0;
    }

}
