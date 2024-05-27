using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;

public enum DamageType//敌人受伤类型
{
    // 物理攻击
    Physics,
    // 魔法攻击
    Magic
}

public class Enemy : View
{

    private Transform render_parent;

    private Transform hp_value;

    private int _hp;//当前敌人血量

    private bool isRuning = false;

    private float runTime = 0;//要走的时间
    private float runTimer = 0;//已经走过的时间

    private Vector3 renderScale;

    private GameObject hpObj;//生命值实体

    private Animator animator;

    private Collider2D _collider2D;


    public EnemyInfo EnemyInfo { get; private set; }

    public SplineContainer Path { get; private set; }//当前敌人路径


    public int Hp
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            if (_hp < 0)
            {
                _hp = 0;
            }
            UpdateHp();
            if (_hp <= 0)
            {
                OnDie();
            }
        }

    }


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);
        if (param == null || param.Length == 0)
        {
            throw new System.Exception("参数缺失!");
        }
        int EnemyId = (int)param[0];//获取敌人id

        EnemyInfo = Module.LoadController<EnemiesController>().GetEnemyInfo(EnemyId);
        if (EnemyInfo == null)
        {
            throw new System.Exception(string.Format("敌人信息查询失败:{0}！", EnemyId));
        }
        Path = param[1] as SplineContainer;
        if (Path == null)//判断路径参数是否正确
        {
            throw new System.Exception("敌人路径异常!");
        }

        // 把当前游戏物体的位置放置到路径的起点
        float3 point = Path.Spline.Knots.First().Position;//获取的是局部坐标
        transform.position = new Vector3(point.x, point.y, point.z) + Path.transform.position;//在处理移动时需要使用世界坐标，所以需要加上所在物体的世界坐标

        // 设置血量
        Hp = EnemyInfo.hp;

        isRuning = true;

        runTime = Path.CalculateLength() / EnemyInfo.speed;//计算实体走完路径全程需要的时间
        runTimer = 0;


        hpObj.SetActive(true);
        // 播放死亡动画
        animator.SetBool("die", false);
        _collider2D.enabled = true;
    }

    private void Awake()//通过代码对敌人实体进行初始化
    {
        hpObj = transform.Find("hp").gameObject;

        render_parent = transform.Find("render_parent");
        hp_value = transform.Find("hp/value");
        animator = transform.Find("render_parent/renderer").GetComponent<Animator>();
        _collider2D = transform.GetComponent<Collider2D>();
        renderScale = render_parent.localScale;



    }


    private void Update()
    {
        Run();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isRuning = false;
        runTimer = 0;
    }

    //更新血量视图
    public void UpdateHp()
    {
        //通过根据当前血量的数据来控制血量条长度
        hp_value.transform.localScale = new Vector3((float)Hp / EnemyInfo.hp, 1, 1);
    }
    public void OnDie()
    {
        //TODO
        Module.LoadController<FightController>().IncreaseCoin(EnemyInfo.drop_gold);//死亡后增加金币
        hpObj.SetActive(false);

        // 播放死亡动画
        animator.SetBool("die", true);

        isRuning = false;
        runTimer = 0;
        _collider2D.enabled = false;//禁用碰撞体
        // 播放敌人死亡的音效
        Module.LoadController<AudiosController>().PlaySound(AudioConst.sound_enemy_dead);
    }

    public void OnHurt(int damage, DamageType type)
    {

        if (Hp <= 0)
        {
            return;
        }

        switch (type)
        {
            case DamageType.Physics:
                damage -= EnemyInfo.physical_defense;
                break;
            case DamageType.Magic:
                damage -= EnemyInfo.magical_defense;
                break;
        }

        if (damage < 0)
        {
            damage = 0;
        }
        Hp -= damage;
    }

    public void Run()
    {
        if (!isRuning)
        {
            return;
        }

        if (runTimer < runTime)
        {
            
            runTimer += Time.deltaTime;
            Vector3 targetPosition = Path.EvaluatePosition(runTimer / runTime);//通过比值获取实体在路径上的位置
            UpdateDir(targetPosition);
            transform.position = targetPosition;//更新当前位置
        }
        else
        {
            // 移动到终点
            isRuning = false;
            runTimer = 0;
            // 对玩家造成伤害，减少玩家生命值
            Module.LoadController<FightController>().PlayerHurt();
            // 回收,移除实体
            Close();
        }

    }

    //更新敌人的朝向
    private void UpdateDir(Vector3 targetPosition)
    {
        Vector3 dir = targetPosition - transform.position;
        if (dir.x > 0)
        {
            render_parent.localScale = new Vector3(Mathf.Abs(renderScale.x), renderScale.y, renderScale.z);
        }
        else if (dir.x < 0)
        {
            render_parent.localScale = new Vector3(-Mathf.Abs(renderScale.x), renderScale.y, renderScale.z);
        }
    }


}
