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
        // 准备资源
        yield return StartCoroutine(ReadyMainModuleRes());

        // 启动MainModule
        StartUpModuleRequest request = ModuleManager.StartUpModule<MainModule>();
        yield return request;
        if (!string.IsNullOrEmpty(request.error))
            Debug.LogErrorFormat("模块启动失败:{0}", request.error);
    }

    /// <summary>
    ///  准备MainModule资源
    /// </summary>
    /// <returns></returns>
    IEnumerator ReadyMainModuleRes()
    {
        //死循环作用:如果资源准备失败则重新准备,如果成功则跳出循环
        while (true)
        {
            // 检测资源
            CheckResUpdateRequest request_check = AssetBundleManager.CheckResUpdate("MainModule");

            yield return request_check;

            if (string.IsNullOrEmpty(request_check.error))
            {
                // 资源检测成功 
                if (request_check.result.updateType == UpdateType.DontNeedUpdate) break;
                // 如果需要更新,这里可以根据更新类型来给一些提示,
                // 比如:一般情况下会在更新之前提示用户,需要下载多少资源,更新内容,等信息 TODO 
                // 如果不需要提示的话，不用做任何处理

                if (request_check.result.updateSize > 0)
                {
                    string updateSize = StringTools.FormatByte(request_check.result.updateSize);
                    string content = string.Format("检测到资源需要更新！\n大小：{0} 版本：{1}", updateSize, request_check.result.version);

                    tipPanel.ShowTips("更新提示",content,() => tipPanel.Hide(),()=>Application.Quit(),true,"更新资源","退出游戏");
                
                    while (tipPanel.gameObject.activeSelf)
                    {
                        yield return null;
                    }
                }
            }
            else
            {
                //string updateSize = StringTools.FormatByte(request_check.result.updateSize);
                string content = string.Format("资源检查失败，请检查网络后再试！");

                tipPanel.ShowTips("更新提示", content, () => tipPanel.Hide(), () => Application.Quit(), true, "更新资源", "退出游戏");

                while (tipPanel.gameObject.activeSelf)
                {
                    yield return null;
                }
                // 资源检测失败,给出相应提示后,再次检测 TODO
                continue;
            }

            // 准备资源
            ReadyResRequest request = AssetBundleManager.ReadyRes(request_check.result);

            while (!request.isDone)
            {
                yield return null;
                // 可以在这里更新UI界面 TODO
                switch (request.ExecutionType)
                {
                    case ExecutionType.Download:
                        // 正在下载资源
                        startUpPanel.showTip("正在下载资源...");
                        break;
                    case ExecutionType.Decompression:
                        // 解压资源
                        startUpPanel.showTip("正在解压资源...");
                        break;
                    case ExecutionType.Verify:
                        // 校验资源(资源下载完成后,需要校验文件是否损坏)
                        startUpPanel.showTip("正在校验资源...");
                        break;
                    case ExecutionType.ExtractLocal:
                        // 释放资源(把资源从内置目录复制到数据目录)
                        startUpPanel.showTip("正在释放资源...");
                        break;
                }

                startUpPanel.UpdateProgress(request.progress);
            }

            if (string.IsNullOrEmpty(request.error))
            {
                // 资源准备成功跳出循环
                break;
            }
            else
            {
                // 资源准备失败,给出相应提示,然后再次准备 TODO
                string content = string.Format("资源准备失败，请检查网络后再试！");

                tipPanel.ShowTips("更新提示", content, () => tipPanel.Hide(), () => Application.Quit(), true, "更新资源", "退出游戏");

                while (tipPanel.gameObject.activeSelf)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(2);
            }
        }
    }
}