using System.Collections;
using System.Collections.Generic;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class StartUpPanel : MonoBehaviour
{
    [SerializeField]//����SerializeField���ԣ��������л��ֶ�
    private Slider slider_progress;
    [SerializeField]
    private Text text_tip;

    // Start is called before the first frame update
    void Start()
    {
        slider_progress.gameObject.SetActive(false);
        text_tip.text=string.Empty;
    }
    public void UpdateProgress(float progress)
    {
        if (slider_progress.gameObject.activeSelf == false)
        {
            slider_progress.gameObject.SetActive(true);
        }
        slider_progress.value = progress;
        
    }
    public void showTip(string message)
    {
        text_tip.text = message;
    }

}
