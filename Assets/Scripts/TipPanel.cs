using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipPanel : MonoBehaviour
{
    [SerializeField]
    private Text text_title;
    [SerializeField]
    private Text text_content;
    [SerializeField]
    private Text btn_confirm_text;
    [SerializeField]
    private Text btn_cancel_text;
    [SerializeField]
    private Button btn_confirm;
    [SerializeField]
    private Button btn_cancel;


    private void Awake()
    {
        Hide();

    }

    public void ShowTips(string title,string content,
        UnityAction onConfirmClick, UnityAction onCancelClick = null,
        bool isShowCancelBtn = true,string btnConfirmText="确定",string btnCancelText= "取消" )
    {
        gameObject.SetActive( true );
        text_title.text = title;
        text_content.text = content;
        btn_confirm_text.text = btnConfirmText;
        btn_cancel_text.text = btnCancelText;

        btn_confirm.onClick.RemoveAllListeners();//移除按钮上的所有事件
        btn_confirm.onClick.AddListener(onConfirmClick);

        btn_cancel.onClick.RemoveAllListeners();
        btn_cancel.onClick.AddListener(onCancelClick);
           
        btn_cancel.gameObject.SetActive( isShowCancelBtn );

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
