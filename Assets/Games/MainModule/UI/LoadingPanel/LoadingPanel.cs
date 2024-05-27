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
        //ʹ�����ڵ���ʱ�ĳ�ʼֵΪ0
        UpdateProgress(0);
        progress.fillAmount = 0;    
    }

    public void UpdateProgress (float progressValue)
    {
        targetProgress = progressValue; 
        
    }
    //�õ��õĳ����жϵ�ǰҳ���Ƿ�������
    public bool isDone()
    {
        return progress.fillAmount == 1;
    }
    //ʹ��update�ý������𽥼��أ��ü��ؽ��治����һ�¾���ʧ
    private void Update()
    {
        if (progress.fillAmount <= targetProgress)
        {
            progress.fillAmount += Time.deltaTime;
            //progress.fillAmount = Mathf.Lerp(progress.fillAmount, targetProgress, Time.deltaTime);
        }
    }

}
