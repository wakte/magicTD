using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFABManager;
using XFGameFramework;

public class WinPanel : Panel
{

    [SerializeField]
    private ImageLoader image_loader_star;
    [SerializeField]
    private Button btn_next;


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);

        // 查询星星数

        int index = Module.LoadController<GameController>().GetOnFileIndex();

        int levelId = Module.LoadController<GameController>().GetCurrentPlayLevelId();

        int star = Module.LoadController<OnFlieController>().Get(index).PasssLevels[levelId].star;

        image_loader_star.AssetName = string.Format("star_{0}", star + 1);

        btn_next.interactable = Module.LoadController<GameController>().IsHaveNextLevel();

    }


    public void OnBtnMenuClick() {
        Module.LoadController<GameController>().ControlGameState(GameState.SelectLevel);
    }

    public void OnBtnNextClick() {
        Module.LoadController<GameController>().NextLevel();
    }


}
