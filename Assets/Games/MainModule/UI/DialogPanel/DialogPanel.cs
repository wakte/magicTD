using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;

public class DialogPanelParam
{
    public string message;//
    public Action click_btn_confirm;//ȷ�ϰ�ť������ʱ�Ļص��¼�
    public Action click_btn_cancel;//ȡ����ť������ʱ�Ļص��¼�
}

public class DialogPanel : Panel
{
    [SerializeField]
    private Text text_neirong;
    private Action click_btn_confirm;
    private Action click_btn_cancel;


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);
        if( param == null||param.Length==0)
        {
            throw new Exception("���ز����쳣1");
        }
        DialogPanelParam p = param[0] as DialogPanelParam;
        if (p == null)
        {
            throw new Exception("���ز����쳣2");
        }
        this.text_neirong.text =p.message;
        this.click_btn_cancel = p.click_btn_cancel;
        this.click_btn_confirm = p.click_btn_confirm;

    }

    public void OnBtnConfirmClick()
    {

        if (click_btn_confirm != null)
        {
            click_btn_confirm.Invoke();
            Close();    
        }
       
    }


    public void OnBtnCancelClick()
    {
        if (click_btn_cancel != null)
        {
            click_btn_cancel.Invoke();
            Close();
        }
    }
}
