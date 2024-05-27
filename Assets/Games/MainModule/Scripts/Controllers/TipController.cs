using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;


public class TipController : Controller
{
    public void ShowToast(string message, float time = 1)
    {
        Module.LoadPanel<ToastPanel>(UIType.UI, null, message, time);
    }
}
