using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class PassLevelInfo
{
    //�ؿ�id
    public int LevelId;
    //��ȡ������
    public int star;
}
public class OnFileModel : Model
{
    public Dictionary<int,PassLevelInfo> PasssLevels= new Dictionary<int,PassLevelInfo>();//������ͨ���Ĺؿ�

    public int AllStar
    {
        get
        {
            int count = 0;
            foreach (var item in PasssLevels.Values)
            {
                count += item.star;
            }
            return count;
        }

    }
}
