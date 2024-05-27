using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XFABManager;
using XFGameFramework;

public class SceneController : Controller
{
    //加载场景的函数，参数为需要加载的场景名，加载完成的回调（用于在加载完成之后进行一些处理），进度条改变的回调
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
        //当异步执行的场景加载还没结束时
        while (!operation.isDone)
        {
            yield return null;
            //如果当前场景需要显示加载进度条
            //将场景加载的进度作为参数传递给进度条模块
            progress?.Invoke(operation.progress);
        }
        //如果存在加载完成的回调则执行
        onFinsh?.Invoke();
    }
}
