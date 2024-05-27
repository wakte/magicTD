using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class OnFlieController : Controller
{
    //�����浵
    public void Create(int index)
    {   
        OnFileModel onFileModel = new OnFileModel();
        onFileModel.Id = index;
        Module.AddModel(onFileModel);
        //�����ݶ���ת����json���浽����
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
        // ɾ����������
        string key = GetPrefsKey(index);
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }

    public void Update(int index)
    {
        OnFileModel model = Module.GetModel<OnFileModel>(index);
        if(model == null)
        {
            return;//��ѯ������ֱ�ӷ���
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
            string content = PlayerPrefs.GetString(key, string.Empty);//��������
            model = JsonConvert.DeserializeObject<OnFileModel>(content);//�Բ��ҵ������ݽ��з����л�
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
    //�жϹؿ��������
    public bool IsLockLevel(int levelId)
    {
        if (levelId == 0)
        {
            return true;    //Ĭ�Ͻ�����һ��
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
    {//���������Ϣ

        int levelId = Module.LoadController<GameController>().GetCurrentPlayLevelId();

        OnFileModel model = Get(Module.LoadController<GameController>().GetOnFileIndex());

        int star_count = Module.LoadController<FightController>().CaculateStar();

        if (model.PasssLevels.ContainsKey(levelId))
        {
            // ������������
            if (model.PasssLevels[levelId].star < star_count)
            {
                model.PasssLevels[levelId].star = star_count;
            }
        }
        else
        {
            // ���һ����������

            PassLevelInfo passLevelInfo = new PassLevelInfo();
            passLevelInfo.LevelId = levelId;
            passLevelInfo.star = star_count;
            model.PasssLevels.Add(passLevelInfo.LevelId, passLevelInfo);

        }

        // ���´浵��Ϣ
        Update(Module.LoadController<GameController>().GetOnFileIndex());

    }

}
