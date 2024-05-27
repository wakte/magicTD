using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;

public class LoadingPanel : Panel
{
    [SerializeField]
    private Image progress;

    private float targetProgress;

    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);
        //使代码在调用时的初始值为0
        UpdateProgress(0);
        progress.fillAmount = 0;    
    }

    public void UpdateProgress (float progressValue)
    {
        targetProgress = progressValue; 
        
    }
    //让调用的程序判断当前页面是否加载完成
    public bool isDone()
    {
        return progress.fillAmount == 1;
    }
    //使用update让进度条逐渐加载，让加载界面不会闪一下就消失
    private void Update()
    {
        if (progress.fillAmount <= targetProgress)
        {
            progress.fillAmount += Time.deltaTime;
            //progress.fillAmount = Mathf.Lerp(progress.fillAmount, targetProgress, Time.deltaTime);
        }
    }

}
