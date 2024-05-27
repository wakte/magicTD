using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class PassLevelInfo
{
    //关卡id
    public int LevelId;
    //获取星星数
    public int star;
}
public class OnFileModel : Model
{
    public Dictionary<int,PassLevelInfo> PasssLevels= new Dictionary<int,PassLevelInfo>();//保存已通过的关卡

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
