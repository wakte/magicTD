using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class FightModel : Model
{
   public int CurrentEnemyWave = 0;

    // 玩家血量
    public int PlayerHp;
    public int PlayerCoin;
    public int PlayerMaxHp;

    public override void OnInit()
    {
        base.OnInit();
        PlayerHp = Module.LoadController<LevelsController>().GetPlayerHp();//玩家血量设置
        PlayerMaxHp = Module.LoadController<LevelsController>().GetPlayerHp();//玩家血量设置
        int levelId = Module.LoadController<GameController>().GetCurrentPlayLevelId();
        LevelInfo levelInfo = Module.LoadController<LevelsController>().GetLevelInfo(levelId);
        PlayerCoin = levelInfo.InitialCoin;
    }
}
