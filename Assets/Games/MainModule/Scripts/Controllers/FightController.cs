using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.UI;
using UnityEngine.UIElements;
using XFGameFramework;

public class FightController : Controller
{
    //
    private Dictionary<string, SplineContainer> paths = new Dictionary<string, SplineContainer>();

    private Dictionary<string ,FightButton> fightButtons = new Dictionary<string ,FightButton>();

    private TowerPosition[] positions = null;

    private GameController gameController => Module.LoadController<GameController>();

    private LevelsController levelsController => Module.LoadController<LevelsController>();

    private EnemiesController enemiesController => Module.LoadController<EnemiesController>();
    //用来保存所有敌人数据
    private List<Enemy> enemies = new List<Enemy>();

    private float waveTimer = 0;//敌人等待间隔

    public void InitScene(Scene scene)
    {
        //初始化开始战斗ui
        InitFightButtons(scene);

        //初始化路径
        InitPaths(scene);

        //初始化炮塔位置 TODO
        InitTowerPositions(scene);
    }

    //查询路径方法
    public SplineContainer GetPath(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        //根据路径名查询实体
        if (paths.ContainsKey(name))
        {
            return paths[name];
        }
        else
        {
            return null;
        }
        
    }


    private void InitTowerPositions(Scene scene)
    {
        GameObject towerPositionObj = scene.FindRootGameObject("TowerPositions");
        if (towerPositionObj == null)
        {
            throw new System.Exception("未在场景中查询到TowerPositions!");
        }
            
        positions = towerPositionObj.GetComponentsInChildren<TowerPosition>();
        foreach (var item in positions)
        {
            item.InitModule(Module);
        }
    }


    //初始化路径
    private void InitPaths(Scene scene)
    {
        paths.Clear();
        GameObject pathObj = scene.FindRootGameObject("paths");
        if (pathObj == null)
        {
            throw new System.Exception("未在关卡场景中查询到paths!");
        }
        for (int i = 0; i < pathObj.transform.childCount; i++)
        {
            Transform child = pathObj.transform.GetChild(i);//查找所有路径子节点
            if (child == null)
            {
                continue;
            }
            SplineContainer container = child.GetComponent<SplineContainer>();
            if (container == null)
            {
                continue;
            }
            if (!paths.ContainsKey(child.name))
            {
                paths.Add(child.name, container);
            }
            else
            {
                //检查路径名是否重复
                throw new System.Exception(string.Format("路径名称重复:{0}!", child.name));
            }
        }
    }


    private void InitFightButtons(Scene scene)
    {
        fightButtons.Clear();
        //获取所有的开始战斗按钮
        GameObject fight_button_obj = scene.FindRootGameObject("fight_buttons");

        FightButton[] buttons = fight_button_obj.GetComponentsInChildren<FightButton>(); 

        for (int i = 0; i < fight_button_obj.transform.childCount; i++)
        {
            Transform child = fight_button_obj.transform.GetChild(i);
            if (child == null)
            {
                continue;
            }
            FightButton button = child.GetComponent<FightButton>();
            if (button == null)
            {
                continue;
            }
            if (fightButtons.ContainsKey(button.name))
            {
                throw new System.Exception(string.Format("fight_button名称重复:{0}", button.name));
            }        

            //传递初始化模块，不然会报错，再de一年bug
            button.Init(Module);
            fightButtons.Add(button.name, button);
        }
        //foreach (FightButton button in buttons) //该代码无法找到被隐藏起来的按钮，产生报错
        //{
        //    if (fightButtons.ContainsKey(button.name))
        //    {
        //        throw new System.Exception(string.Format("战斗开始按钮名称重复：{0}", button.name));
        //    }
        //    
        //    button.Init(Module);
        //    fightButtons.Add(button.name, button);  
        //}
    }
    //查询按钮
    public FightButton GetFightButton(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        if (fightButtons.ContainsKey(name))
        {
            return fightButtons[name];
        }
        else
        {
            return null;    
        }
    }
    //通过协程生成敌人
    public IEnumerator GenerateEnemy()
    {
        int levelId = gameController.GetCurrentPlayLevelId();

        LevelInfo levelInfo = levelsController.GetLevelInfo(levelId);

        if (levelInfo == null)
        {
            throw new System.Exception(string.Format("查询关卡信息失败:{0}", levelId));
        }

        enemies.Clear();

        //保存当前波次所有需要生成的敌人数据
        List<EnemyData> enemyData = new List<EnemyData>();

        for (int i = 0; i < levelInfo.enemyWaves.Count; i++)
        {
            // 更新当前波数
            GetFightModel().CurrentEnemyWave = i + 1;
            // 通过传入事件名称触发当前波次改变事件
            EventManager.TriggerEvent(EventConst.ON_ENEMY_WAVE_CHANGE);
            //敌人数据是通过配表来获取的，不建议直接操作
            EnemyWaves wave = levelInfo.enemyWaves[i];
            if (i != 0)//当需要生成第一波敌人时
            {
                // 获取开始战斗的按钮
                FightButton button = GetFightButton(wave.fight_buttton);
                if (button == null)
                {
                    throw new System.Exception(string.Format("查询战斗按钮失败:{0}", wave.fight_buttton));
                }
                button.Show();
                button.AddClick(() =>
                {//注册按钮事件：当点击时，立刻开始下一波敌人，让关卡计时直接等于敌人间隔
                    waveTimer = levelsController.GetGenerateEnemyTimeInterval();
                });
                waveTimer = 0;
                while (waveTimer < levelsController.GetGenerateEnemyTimeInterval())
                {
                    yield return null;
                    waveTimer += Time.deltaTime;
                    button.UpdateProgress(waveTimer / levelsController.GetGenerateEnemyTimeInterval());//更新按钮进度
                }
                button.Hide();
            }
            enemyData.Clear();
            enemyData.AddRange(wave.enemies);
            float timer = 0;
            while (enemyData.Count > 0)
            {
                yield return null;
                timer += Time.deltaTime;
                //遍历所有敌人数据，根据当前时间判断是否生成
                for (int j = 0; j < enemyData.Count; j++)
                {
                    if (enemyData[j].time > timer)
                    {
                        continue;
                    } 
                    SplineContainer path = GetPath(enemyData[j].path);
                    // 创建敌人
                    Enemy enemy = enemiesController.CreateEnemy(enemyData[j].enemyId, path);
                    enemyData.RemoveAt(j);
                    j--;
                    enemies.Add(enemy);
                }
            }
            yield return new WaitForSeconds(5);
        }
        // 所有的敌人都生成完成了
        while (enemies.Count > 0)
        {
            for (int j = 0; j < enemies.Count; j++)
            {
                Enemy enemy = enemies[j];
                if (enemy == null || !enemy.gameObject.activeSelf)//判断敌人当前是否死亡
                {
                    enemies.RemoveAt(j);
                    j--;
                }
            }

            yield return null;
        }//执行完成，所以敌人死亡

        // 当前游戏结束
        gameController.ControlGameState(GameState.GamingEnd);
    }

    public FightModel GetFightModel()
    {
        //获取战斗模块数据
        FightModel model = Module.GetModel<FightModel>();

        if (model == null)
        {
            //新建一个战斗模块的数据
            model = new FightModel();
            Module.AddModel(model);
        }

        return model;
    }


    public void PlayerHurt()
    {
        FightModel model = GetFightModel();
        model.PlayerHp--;
        if (model.PlayerHp <= 0)
        {
            // 游戏结束,切换状态
            gameController.ControlGameState(GameState.GamingEnd);
        }

        EventManager.TriggerEvent(EventConst.ON_PLAYER_HP_VALUE_CHANGE);

        // 播放玩家受伤的音效
        Module.LoadController<AudiosController>().PlaySound(AudioConst.sound_loose_life);

    }

    public void IncreaseCoin(int coin)//增加金币
    {
        FightModel model = GetFightModel();
        model.PlayerCoin += coin;
        EventManager.TriggerEvent(EventConst.ON_PLAYER_COIN_VALUE_CHANGE);
    }

    public void ReduceCoin(int coin)
    {
        FightModel model = GetFightModel();
        model.PlayerCoin -= coin;
        if (model.PlayerCoin < 0)
        {
            model.PlayerCoin = 0;
        }

        EventManager.TriggerEvent(EventConst.ON_PLAYER_COIN_VALUE_CHANGE);
    }

    public void ClearEnemy()
    {
        while (enemies.Count > 0)
        {
            Enemy enemy = enemies[enemies.Count - 1];
            enemy.Close();
            enemies.RemoveAt(enemies.Count - 1);
        }
    }

    public void ClearFightModel()
    {
        FightModel model = GetFightModel();
        Module.RemoveModel(model);
    }


    public void Clear()
    {
        // 清空敌人
        ClearEnemy();
        // 清空战斗数据
        ClearFightModel();
        // 清空炮塔
        Module.LoadController<TowersController>().ClearTowers();
        // 炮塔底座清空
        ClearTowerPosition();
    }

    public void ClearTowerPosition()
    {
        foreach (var item in positions)
        {
            item.Tower = null;
        }
    }

    public void Replay()
    {
        // 清空游戏数据 
        Clear();

        EventManager.TriggerEvent(EventConst.ON_PLAYER_COIN_VALUE_CHANGE);//调用更新金币UI的事件
        EventManager.TriggerEvent(EventConst.ON_PLAYER_HP_VALUE_CHANGE);
        EventManager.TriggerEvent(EventConst.ON_ENEMY_WAVE_CHANGE);
        // 修改游戏状态
        Module.LoadController<GameController>().ControlGameState(GameState.Gaming_Ready);
    }


    public int CaculateStar()
    {//计算通关成绩

        FightModel model = GetFightModel();
        if (model.PlayerHp >= model.PlayerMaxHp*0.9)
        {
            return 3;
        }
        if (model.PlayerHp >= model.PlayerMaxHp * 0.5)
        {
            return 2;
        }
        return 1;
    }
}
