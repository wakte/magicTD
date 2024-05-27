using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;

public enum DamageType//������������
{
    // ������
    Physics,
    // ħ������
    Magic
}

public class Enemy : View
{

    private Transform render_parent;

    private Transform hp_value;

    private int _hp;//��ǰ����Ѫ��

    private bool isRuning = false;

    private float runTime = 0;//Ҫ�ߵ�ʱ��
    private float runTimer = 0;//�Ѿ��߹���ʱ��

    private Vector3 renderScale;

    private GameObject hpObj;//����ֵʵ��

    private Animator animator;

    private Collider2D _collider2D;


    public EnemyInfo EnemyInfo { get; private set; }

    public SplineContainer Path { get; private set; }//��ǰ����·��


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
            throw new System.Exception("����ȱʧ!");
        }
        int EnemyId = (int)param[0];//��ȡ����id

        EnemyInfo = Module.LoadController<EnemiesController>().GetEnemyInfo(EnemyId);
        if (EnemyInfo == null)
        {
            throw new System.Exception(string.Format("������Ϣ��ѯʧ��:{0}��", EnemyId));
        }
        Path = param[1] as SplineContainer;
        if (Path == null)//�ж�·�������Ƿ���ȷ
        {
            throw new System.Exception("����·���쳣!");
        }

        // �ѵ�ǰ��Ϸ�����λ�÷��õ�·�������
        float3 point = Path.Spline.Knots.First().Position;//��ȡ���Ǿֲ�����
        transform.position = new Vector3(point.x, point.y, point.z) + Path.transform.position;//�ڴ����ƶ�ʱ��Ҫʹ���������꣬������Ҫ���������������������

        // ����Ѫ��
        Hp = EnemyInfo.hp;

        isRuning = true;

        runTime = Path.CalculateLength() / EnemyInfo.speed;//����ʵ������·��ȫ����Ҫ��ʱ��
        runTimer = 0;


        hpObj.SetActive(true);
        // ������������
        animator.SetBool("die", false);
        _collider2D.enabled = true;
    }

    private void Awake()//ͨ������Ե���ʵ����г�ʼ��
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

    //����Ѫ����ͼ
    public void UpdateHp()
    {
        //ͨ�����ݵ�ǰѪ��������������Ѫ��������
        hp_value.transform.localScale = new Vector3((float)Hp / EnemyInfo.hp, 1, 1);
    }
    public void OnDie()
    {
        //TODO
        Module.LoadController<FightController>().IncreaseCoin(EnemyInfo.drop_gold);//���������ӽ��
        hpObj.SetActive(false);

        // ������������
        animator.SetBool("die", true);

        isRuning = false;
        runTimer = 0;
        _collider2D.enabled = false;//������ײ��
        // ���ŵ�����������Ч
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
            Vector3 targetPosition = Path.EvaluatePosition(runTimer / runTime);//ͨ����ֵ��ȡʵ����·���ϵ�λ��
            UpdateDir(targetPosition);
            transform.position = targetPosition;//���µ�ǰλ��
        }
        else
        {
            // �ƶ����յ�
            isRuning = false;
            runTimer = 0;
            // ���������˺��������������ֵ
            Module.LoadController<FightController>().PlayerHurt();
            // ����,�Ƴ�ʵ��
            Close();
        }

    }

    //���µ��˵ĳ���
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
