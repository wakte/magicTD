using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XFABManager;
using XFGameFramework;


public class Main : MonoBehaviour
{
    [SerializeField]
    private StartUpPanel startUpPanel;
    [SerializeField]
    private TipPanel tipPanel;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(ReadyMainModuleRes());
        // ׼����Դ
        yield return StartCoroutine(ReadyMainModuleRes());

        // ����MainModule
        StartUpModuleRequest request = ModuleManager.StartUpModule<MainModule>();
        yield return request;
        if (!string.IsNullOrEmpty(request.error))
            Debug.LogErrorFormat("ģ������ʧ��:{0}", request.error);
    }

    /// <summary>
    ///  ׼��MainModule��Դ
    /// </summary>
    /// <returns></returns>
    IEnumerator ReadyMainModuleRes()
    {
        //��ѭ������:�����Դ׼��ʧ��������׼��,����ɹ�������ѭ��
        while (true)
        {
            // �����Դ
            CheckResUpdateRequest request_check = AssetBundleManager.CheckResUpdate("MainModule");

            yield return request_check;

            if (string.IsNullOrEmpty(request_check.error))
            {
                // ��Դ���ɹ� 
                if (request_check.result.updateType == UpdateType.DontNeedUpdate) break;
                // �����Ҫ����,������Ը��ݸ�����������һЩ��ʾ,
                // ����:һ������»��ڸ���֮ǰ��ʾ�û�,��Ҫ���ض�����Դ,��������,����Ϣ TODO 
                // �������Ҫ��ʾ�Ļ����������κδ���

                if (request_check.result.updateSize > 0)
                {
                    string updateSize = StringTools.FormatByte(request_check.result.updateSize);
                    string content = string.Format("��⵽��Դ��Ҫ���£�\n��С��{0} �汾��{1}", updateSize, request_check.result.version);

                    tipPanel.ShowTips("������ʾ",content,() => tipPanel.Hide(),()=>Application.Quit(),true,"������Դ","�˳���Ϸ");
                
                    while (tipPanel.gameObject.activeSelf)
                    {
                        yield return null;
                    }
                }
            }
            else
            {
                //string updateSize = StringTools.FormatByte(request_check.result.updateSize);
                string content = string.Format("��Դ���ʧ�ܣ�������������ԣ�");

                tipPanel.ShowTips("������ʾ", content, () => tipPanel.Hide(), () => Application.Quit(), true, "������Դ", "�˳���Ϸ");

                while (tipPanel.gameObject.activeSelf)
                {
                    yield return null;
                }
                // ��Դ���ʧ��,������Ӧ��ʾ��,�ٴμ�� TODO
                continue;
            }

            // ׼����Դ
            ReadyResRequest request = AssetBundleManager.ReadyRes(request_check.result);

            while (!request.isDone)
            {
                yield return null;
                // �������������UI���� TODO
                switch (request.ExecutionType)
                {
                    case ExecutionType.Download:
                        // ����������Դ
                        startUpPanel.showTip("����������Դ...");
                        break;
                    case ExecutionType.Decompression:
                        // ��ѹ��Դ
                        startUpPanel.showTip("���ڽ�ѹ��Դ...");
                        break;
                    case ExecutionType.Verify:
                        // У����Դ(��Դ������ɺ�,��ҪУ���ļ��Ƿ���)
                        startUpPanel.showTip("����У����Դ...");
                        break;
                    case ExecutionType.ExtractLocal:
                        // �ͷ���Դ(����Դ������Ŀ¼���Ƶ�����Ŀ¼)
                        startUpPanel.showTip("�����ͷ���Դ...");
                        break;
                }

                startUpPanel.UpdateProgress(request.progress);
            }

            if (string.IsNullOrEmpty(request.error))
            {
                // ��Դ׼���ɹ�����ѭ��
                break;
            }
            else
            {
                // ��Դ׼��ʧ��,������Ӧ��ʾ,Ȼ���ٴ�׼�� TODO
                string content = string.Format("��Դ׼��ʧ�ܣ�������������ԣ�");

                tipPanel.ShowTips("������ʾ", content, () => tipPanel.Hide(), () => Application.Quit(), true, "������Դ", "�˳���Ϸ");

                while (tipPanel.gameObject.activeSelf)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(2);
            }
        }
    }
}