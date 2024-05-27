using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class StartPanel : Panel
{

    public void OnBtnStartClick()
    {
        OnFliePanel a =  Module.LoadPanel<OnFliePanel>();
        LogUtil.Log("开始游戏 TODO{0}" ,a == null);
        LogUtil.Log("开始游戏 123123");

    }

    public void OnBtnSettingClick()
    {
        Module.LoadPanel<SettingPanel>();
    }

    public void OnBtnCloseApp()
    {

        DialogPanelParam p = new DialogPanelParam();
        p.message = "您确认要退出游戏吗？";
        LogUtil.Log("打开关闭界面");
        p.click_btn_confirm = () =>
        {
            Application.Quit();
        };
        Module.LoadPanel<DialogPanel>(UIType.UI, null, p);
    }

}
