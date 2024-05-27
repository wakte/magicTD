using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;
public class OnFliePanel : Panel
{

    [SerializeField]
    private Transform[] onFileTransforms;



    private GameController gameController => Module.LoadController<GameController>();
    //ֻ������
    private OnFlieController onFileController=>Module.LoadController<OnFlieController>();   

    //��дawake
    protected override void Awake()
    {
        LogUtil.Log("��ʼ����");
        base.Awake();
        LogUtil.Log("awake����");
        RegisterBtnListener();
    }

    //��дOnLoaded
    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);
        UpdateOnFile();
    }

    //ui�¼�����
    public void OnBtnNewGameClick(int index)
    {
        onFileController.Create(index);
        // ѡ��ǰ�浵 
        Module.LoadController<GameController>().SetFileIndex(index);
        // ����ؿ�ѡ��״̬��ͨ���޸�״̬���ֵĲ���ʵϰ��
        gameController.ControlGameState(GameState.SelectLevel);

        Close();
    }
    public void OnBtnDeleteClick(int index)
    {
        
        DialogPanelParam p = new DialogPanelParam();
        p.message = "��ȷ��Ҫɾ������浵"+index as string +"��";
        
        p.click_btn_confirm = () =>
        {
            //ɾ���浵
            onFileController.Delete(index);
            //ˢ�½���
            UpdateOneFile(index);
        };

        Module.LoadPanel<DialogPanel>(UIType.UI,null,p);//�÷�������ͨ������ui���ͣ��������Լ��Զ����������ʾui
        
    }
    public void OnBtnPlayClick(int index)
    {
        Module.LoadController<GameController>().SetFileIndex(index);
        // ����ؿ�ѡ��״̬��ͨ���޸�״̬���ֵĲ���ʵϰ��
        gameController.ControlGameState(GameState.SelectLevel);

        Close();

    }

    private void RegisterBtnListener()
    {
        LogUtil.Log("111");
        for(int i = 0; i < onFileTransforms.Length; i++)
        {
            int index = i;
            Transform onFlie = onFileTransforms[i];
            if (onFlie == null)continue;
            
            Button btn_newgame=onFlie.Find("Empty/btn_newGame").GetComponent<Button>(); 
            Button btn_delete= onFlie.Find("NotEmpty/Image/btn_delete").GetComponent<Button>();
            Button btn_play = onFlie.Find("NotEmpty/btn_play").GetComponent<Button>();

            btn_newgame.onClick.RemoveAllListeners();
            btn_newgame.onClick.AddListener(() => OnBtnNewGameClick(index));
            btn_delete.onClick.RemoveAllListeners();
            btn_delete.onClick.AddListener(() => OnBtnDeleteClick(index));
            btn_play.onClick.RemoveAllListeners();
            btn_play.onClick.AddListener(() => OnBtnPlayClick(index));
        }
    }
    private void UpdateOnFile()
    {
        LogUtil.Log("��ʼ����");
        //TODO
        for (int i = 0;i < onFileTransforms.Length;i++) 
        {
            UpdateOneFile(i);

        }
    }

    private void UpdateOneFile(int index)
    {

        //int index = i;
        Transform onFlie = onFileTransforms[index];
        if (onFlie == null) return;

        GameObject emptyObj = onFlie.transform.Find("Empty").gameObject;
        GameObject notEmptyObj = onFlie.transform.Find("NotEmpty").gameObject;

        Text text_star = notEmptyObj.transform.Find("start/Text (Legacy)").GetComponent<Text>();
        Text text_level = notEmptyObj.transform.Find("Game_level").GetComponent<Text>();


        OnFileModel model = onFileController.Get(index);
        //�������Ϊ�գ�
        emptyObj.SetActive(model == null);
        notEmptyObj.SetActive(model != null);

        if (model != null)
        {
            int levelCount =Module.LoadController<LevelsController>().GetLevelCount();
            text_star.text = string.Format("{0}/{1}", model.AllStar, levelCount * 3);
            text_level.text = string.Format(" level��{0}/{1}", model.PasssLevels.Count, levelCount);


        }
    }
}
