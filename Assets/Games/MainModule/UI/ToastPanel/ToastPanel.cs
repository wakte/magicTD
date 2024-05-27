using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;

public class ToastPanel : Panel
{

    [SerializeField]
    private Text text_message;


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);

        if (param == null || param.Length == 0)
        {
            throw new System.Exception("参数异常!");
        }
            
        text_message.text = param[0].ToString();

        float time = (float)param[1];

        Invoke("Close", time);
    }

}
