using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class FailedPanel : Panel
{

    public void OnBtnMenuClick() {
        Module.LoadController<GameController>().ControlGameState(GameState.SelectLevel);
        Close();
    }

    public void OnBtnReplayClick() {
        Module.LoadController<FightController>().Replay();
        Close();
    }

}
