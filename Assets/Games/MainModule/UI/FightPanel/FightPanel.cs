using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;

public class FightPanel : Panel
{

    [SerializeField]
    private Text text_hp;
    [SerializeField]
    private Text text_coin;
    [SerializeField]
    private Text text_enemy;


    protected override void Awake()
    {
        base.Awake();

        ListenerEvents = new EventConfig[] {
            new EventConfig(EventConst.ON_PLAYER_HP_VALUE_CHANGE,(p)=>InitUI()),
            new EventConfig(EventConst.ON_PLAYER_COIN_VALUE_CHANGE,(p)=>InitUI()),
            new EventConfig(EventConst.ON_ENEMY_WAVE_CHANGE,(p)=>InitUI())
        };

    }


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);
        InitUI();
    }


    private void InitUI() {

        FightModel fightModel = Module.LoadController<FightController>().GetFightModel();

        text_hp.text = fightModel.PlayerHp.ToString();
        text_coin.text = fightModel.PlayerCoin.ToString();

        int currentEnemyWave = fightModel.CurrentEnemyWave;

        int levelId = Module.LoadController<GameController>().GetCurrentPlayLevelId();
        LevelInfo levelInfo = Module.LoadController<LevelsController>().GetLevelInfo(levelId);
        int allEnemyWave = levelInfo.enemyWaves.Count;

        text_enemy.text = string.Format("{0}/{1}波",currentEnemyWave,allEnemyWave);
    }


    public void OnBtnPauseClick() {
        // 打开暂停界面 
        Module.LoadPanel<PausePanel>();
    }




}
