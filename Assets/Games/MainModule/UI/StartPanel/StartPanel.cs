using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class StartPanel : Panel
{

    public void OnBtnStartClick()
    {
        OnFliePanel a =  Module.LoadPanel<OnFliePanel>();
        LogUtil.Log("��ʼ��Ϸ TODO{0}" ,a == null);
        LogUtil.Log("��ʼ��Ϸ 123123");

    }

    public void OnBtnSettingClick()
    {
        Module.LoadPanel<SettingPanel>();
    }

    public void OnBtnCloseApp()
    {

        DialogPanelParam p = new DialogPanelParam();
        p.message = "��ȷ��Ҫ�˳���Ϸ��";
        LogUtil.Log("�򿪹رս���");
        p.click_btn_confirm = () =>
        {
            Application.Quit();
        };
        Module.LoadPanel<DialogPanel>(UIType.UI, null, p);
    }

}
