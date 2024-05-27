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
    //�����������е�������
    private List<Enemy> enemies = new List<Enemy>();

    private float waveTimer = 0;//���˵ȴ����

    public void InitScene(Scene scene)
    {
        //��ʼ����ʼս��ui
        InitFightButtons(scene);

        //��ʼ��·��
        InitPaths(scene);

        //��ʼ������λ�� TODO
        InitTowerPositions(scene);
    }

    //��ѯ·������
    public SplineContainer GetPath(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        //����·������ѯʵ��
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
            throw new System.Exception("δ�ڳ����в�ѯ��TowerPositions!");
        }
            
        positions = towerPositionObj.GetComponentsInChildren<TowerPosition>();
        foreach (var item in positions)
        {
            item.InitModule(Module);
        }
    }


    //��ʼ��·��
    private void InitPaths(Scene scene)
    {
        paths.Clear();
        GameObject pathObj = scene.FindRootGameObject("paths");
        if (pathObj == null)
        {
            throw new System.Exception("δ�ڹؿ������в�ѯ��paths!");
        }
        for (int i = 0; i < pathObj.transform.childCount; i++)
        {
            Transform child = pathObj.transform.GetChild(i);//��������·���ӽڵ�
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
                //���·�����Ƿ��ظ�
                throw new System.Exception(string.Format("·�������ظ�:{0}!", child.name));
            }
        }
    }


    private void InitFightButtons(Scene scene)
    {
        fightButtons.Clear();
        //��ȡ���еĿ�ʼս����ť
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
                throw new System.Exception(string.Format("fight_button�����ظ�:{0}", button.name));
            }        

            //���ݳ�ʼ��ģ�飬��Ȼ�ᱨ����deһ��bug
            button.Init(Module);
            fightButtons.Add(button.name, button);
        }
        //foreach (FightButton button in buttons) //�ô����޷��ҵ������������İ�ť����������
        //{
        //    if (fightButtons.ContainsKey(button.name))
        //    {
        //        throw new System.Exception(string.Format("ս����ʼ��ť�����ظ���{0}", button.name));
        //    }
        //    
        //    button.Init(Module);
        //    fightButtons.Add(button.name, button);  
        //}
    }
    //��ѯ��ť
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
    //ͨ��Э�����ɵ���
    public IEnumerator GenerateEnemy()
    {
        int levelId = gameController.GetCurrentPlayLevelId();

        LevelInfo levelInfo = levelsController.GetLevelInfo(levelId);

        if (levelInfo == null)
        {
            throw new System.Exception(string.Format("��ѯ�ؿ���Ϣʧ��:{0}", levelId));
        }

        enemies.Clear();

        //���浱ǰ����������Ҫ���ɵĵ�������
        List<EnemyData> enemyData = new List<EnemyData>();

        for (int i = 0; i < levelInfo.enemyWaves.Count; i++)
        {
            // ���µ�ǰ����
            GetFightModel().CurrentEnemyWave = i + 1;
            // ͨ�������¼����ƴ�����ǰ���θı��¼�
            EventManager.TriggerEvent(EventConst.ON_ENEMY_WAVE_CHANGE);
            //����������ͨ���������ȡ�ģ�������ֱ�Ӳ���
            EnemyWaves wave = levelInfo.enemyWaves[i];
            if (i != 0)//����Ҫ���ɵ�һ������ʱ
            {
                // ��ȡ��ʼս���İ�ť
                FightButton button = GetFightButton(wave.fight_buttton);
                if (button == null)
                {
                    throw new System.Exception(string.Format("��ѯս����ťʧ��:{0}", wave.fight_buttton));
                }
                button.Show();
                button.AddClick(() =>
                {//ע�ᰴť�¼��������ʱ�����̿�ʼ��һ�����ˣ��ùؿ���ʱֱ�ӵ��ڵ��˼��
                    waveTimer = levelsController.GetGenerateEnemyTimeInterval();
                });
                waveTimer = 0;
                while (waveTimer < levelsController.GetGenerateEnemyTimeInterval())
                {
                    yield return null;
                    waveTimer += Time.deltaTime;
                    button.UpdateProgress(waveTimer / levelsController.GetGenerateEnemyTimeInterval());//���°�ť����
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
                //�������е������ݣ����ݵ�ǰʱ���ж��Ƿ�����
                for (int j = 0; j < enemyData.Count; j++)
                {
                    if (enemyData[j].time > timer)
                    {
                        continue;
                    } 
                    SplineContainer path = GetPath(enemyData[j].path);
                    // ��������
                    Enemy enemy = enemiesController.CreateEnemy(enemyData[j].enemyId, path);
                    enemyData.RemoveAt(j);
                    j--;
                    enemies.Add(enemy);
                }
            }
            yield return new WaitForSeconds(5);
        }
        // ���еĵ��˶����������
        while (enemies.Count > 0)
        {
            for (int j = 0; j < enemies.Count; j++)
            {
                Enemy enemy = enemies[j];
                if (enemy == null || !enemy.gameObject.activeSelf)//�жϵ��˵�ǰ�Ƿ�����
                {
                    enemies.RemoveAt(j);
                    j--;
                }
            }

            yield return null;
        }//ִ����ɣ����Ե�������

        // ��ǰ��Ϸ����
        gameController.ControlGameState(GameState.GamingEnd);
    }

    public FightModel GetFightModel()
    {
        //��ȡս��ģ������
        FightModel model = Module.GetModel<FightModel>();

        if (model == null)
        {
            //�½�һ��ս��ģ�������
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
            // ��Ϸ����,�л�״̬
            gameController.ControlGameState(GameState.GamingEnd);
        }

        EventManager.TriggerEvent(EventConst.ON_PLAYER_HP_VALUE_CHANGE);

        // ����������˵���Ч
        Module.LoadController<AudiosController>().PlaySound(AudioConst.sound_loose_life);

    }

    public void IncreaseCoin(int coin)//���ӽ��
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
        // ��յ���
        ClearEnemy();
        // ���ս������
        ClearFightModel();
        // �������
        Module.LoadController<TowersController>().ClearTowers();
        // �����������
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
        // �����Ϸ���� 
        Clear();

        EventManager.TriggerEvent(EventConst.ON_PLAYER_COIN_VALUE_CHANGE);//���ø��½��UI���¼�
        EventManager.TriggerEvent(EventConst.ON_PLAYER_HP_VALUE_CHANGE);
        EventManager.TriggerEvent(EventConst.ON_ENEMY_WAVE_CHANGE);
        // �޸���Ϸ״̬
        Module.LoadController<GameController>().ControlGameState(GameState.Gaming_Ready);
    }


    public int CaculateStar()
    {//����ͨ�سɼ�

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
