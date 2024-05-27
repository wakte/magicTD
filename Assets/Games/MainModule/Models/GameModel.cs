using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

//枚举要与状态机中的设置一致
public enum GameState
{
    Start,
    SelectLevel,
    Gaming_Ready,
    Gaming_Fight,
    GamingEnd
}

public class GameModel : Model
{
    //  存档下标
    public int fileIndex;

    //游戏状态
    public GameState state=GameState.Start;

    public int currentPlayingLevelId;
        

}
