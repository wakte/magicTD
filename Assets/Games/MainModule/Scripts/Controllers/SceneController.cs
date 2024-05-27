using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XFABManager;
using XFGameFramework;

public class SceneController : Controller
{
    //���س����ĺ���������Ϊ��Ҫ���صĳ�������������ɵĻص��������ڼ������֮�����һЩ�������������ı�Ļص�
    public void LoadScene(string sceneName,Action onFinsh,Action<float> progress = null)
    {
        if (SceneManager.GetSceneByName(sceneName).IsValid())
        {
            onFinsh?.Invoke();
            return;
            //if (progress != null)
            //{
            //    progress.Invoke(1);
            //}
        }
        CoroutineStarter.Start(LoadSceneExecute(sceneName, onFinsh, progress));
    }

    private IEnumerator LoadSceneExecute(string sceneName, Action onFinsh, Action<float> progress = null)
    {
        AsyncOperation operation = AssetBundleManager.LoadSceneAsync(Module.ProjectName, sceneName, LoadSceneMode.Single);
        //���첽ִ�еĳ������ػ�û����ʱ
        while (!operation.isDone)
        {
            yield return null;
            //�����ǰ������Ҫ��ʾ���ؽ�����
            //���������صĽ�����Ϊ�������ݸ�������ģ��
            progress?.Invoke(operation.progress);
        }
        //������ڼ�����ɵĻص���ִ��
        onFinsh?.Invoke();
    }
}
