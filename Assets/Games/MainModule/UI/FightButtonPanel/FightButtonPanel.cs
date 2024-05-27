using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;
public class FightButtonPanel : Panel
{
    [SerializeField]
    private Image progress;
    [SerializeField]
    private Transform dir_anchor_point;
    [SerializeField]
    private Image Arrow;

    public Action onClick;

    public void UpdateProgress(float nowProgress)
    {
        progress.fillAmount = nowProgress;
    }

    public void UpdateDir(float angle)
    {
        dir_anchor_point.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void SetArrowInactive()
    {
        Arrow.gameObject.SetActive(false);
    }

    public void SetArrowActive()
    {
        Arrow.gameObject.SetActive(true);
    }

    public void OnButtonClick()
    {
        if (onClick!=null)
        {
            onClick.Invoke();
        }
    }

    //��ui��ʧʱ����հ�ť�¼�
    protected override void OnDisable()
    {
        base.OnDisable();
        onClick = null;
    }

}