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
    //只读属性
    private OnFlieController onFileController=>Module.LoadController<OnFlieController>();   

    //重写awake
    protected override void Awake()
    {
        LogUtil.Log("开始调用");
        base.Awake();
        LogUtil.Log("awake结束");
        RegisterBtnListener();
    }

    //重写OnLoaded
    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);
        UpdateOnFile();
    }

    //ui事件监听
    public void OnBtnNewGameClick(int index)
    {
        onFileController.Create(index);
        // 选择当前存档 
        Module.LoadController<GameController>().SetFileIndex(index);
        // 进入关卡选择状态（通过修改状态机种的参数实习）
        gameController.ControlGameState(GameState.SelectLevel);

        Close();
    }
    public void OnBtnDeleteClick(int index)
    {
        
        DialogPanelParam p = new DialogPanelParam();
        p.message = "您确认要删除这个存档"+index as string +"吗？";
        
        p.click_btn_confirm = () =>
        {
            //删除存档
            onFileController.Delete(index);
            //刷新界面
            UpdateOneFile(index);
        };

        Module.LoadPanel<DialogPanel>(UIType.UI,null,p);//该方法可以通过传入ui类型，父物体以及自定义参数来显示ui
        
    }
    public void OnBtnPlayClick(int index)
    {
        Module.LoadController<GameController>().SetFileIndex(index);
        // 进入关卡选择状态（通过修改状态机种的参数实习）
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
        LogUtil.Log("开始更新");
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
        //如果数据为空，
        emptyObj.SetActive(model == null);
        notEmptyObj.SetActive(model != null);

        if (model != null)
        {
            int levelCount =Module.LoadController<LevelsController>().GetLevelCount();
            text_star.text = string.Format("{0}/{1}", model.AllStar, levelCount * 3);
            text_level.text = string.Format(" level：{0}/{1}", model.PasssLevels.Count, levelCount);


        }
    }
}
