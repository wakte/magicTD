using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

//ö��Ҫ��״̬���е�����һ��
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
    //  �浵�±�
    public int fileIndex;

    //��Ϸ״̬
    public GameState state=GameState.Start;

    public int currentPlayingLevelId;
        

}
