using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class OnFlieController : Controller
{
    //创建存档
    public void Create(int index)
    {   
        OnFileModel onFileModel = new OnFileModel();
        onFileModel.Id = index;
        Module.AddModel(onFileModel);
        //将数据对象转化成json保存到本地
        string key = GetPrefsKey(index);
        PlayerPrefs.SetString(key, JsonConvert.SerializeObject(onFileModel));
        PlayerPrefs.Save();
    }

    public void Delete(int index) {
        OnFileModel model = Module.GetModel<OnFileModel>(index);
        if(model != null)
        {
            Module.RemoveModel(model);
        }
        // 删除本地内容
        string key = GetPrefsKey(index);
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }

    public void Update(int index)
    {
        OnFileModel model = Module.GetModel<OnFileModel>(index);
        if(model == null)
        {
            return;//查询到数据直接返回
        }
        string key = GetPrefsKey(index);
        PlayerPrefs.SetString(key, JsonConvert.SerializeObject(model));
        PlayerPrefs.Save();
    }
    public OnFileModel Get(int index)
    {
        OnFileModel model = Module.GetModel<OnFileModel>(index);
        if (model != null)
        {
            return model;
        }
 
        string key = GetPrefsKey(index);
        try
        {
            string content = PlayerPrefs.GetString(key, string.Empty);//查找数据
            model = JsonConvert.DeserializeObject<OnFileModel>(content);//对查找到的数据进行反序列化
            Module.AddModel(model);
        }
        catch(System.Exception)
        {
            
        }

        return model;


    }

    public string GetPrefsKey(int index) 
    {
        return string.Format("OnFileController:OnFileModel:{0}", index);
    }
    //判断关卡解锁情况
    public bool IsLockLevel(int levelId)
    {
        if (levelId == 0)
        {
            return true;    //默认解锁第一关
        }

        int index = Module.LoadController<GameController>().GetOnFileIndex();
        OnFileModel model = Get(index);
        if (model.PasssLevels.ContainsKey(levelId) || model.PasssLevels.ContainsKey(levelId - 1))
        {
            return true;    
        }
        else
        {
            return false;
        }
    }

    public void SaveCurrentPassLevelInfo()
    {//保存过关信息

        int levelId = Module.LoadController<GameController>().GetCurrentPlayLevelId();

        OnFileModel model = Get(Module.LoadController<GameController>().GetOnFileIndex());

        int star_count = Module.LoadController<FightController>().CaculateStar();

        if (model.PasssLevels.ContainsKey(levelId))
        {
            // 更新星星数据
            if (model.PasssLevels[levelId].star < star_count)
            {
                model.PasssLevels[levelId].star = star_count;
            }
        }
        else
        {
            // 添加一条过关数据

            PassLevelInfo passLevelInfo = new PassLevelInfo();
            passLevelInfo.LevelId = levelId;
            passLevelInfo.star = star_count;
            model.PasssLevels.Add(passLevelInfo.LevelId, passLevelInfo);

        }

        // 更新存档信息
        Update(Module.LoadController<GameController>().GetOnFileIndex());

    }

}
