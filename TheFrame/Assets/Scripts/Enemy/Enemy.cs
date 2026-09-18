using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    alert,
    patrol,
    watch,
    movement,
    attack,
    reload,
    tracking,
    lastLocationWatch,
    runaway
}

public class Enemy : MonoBehaviour
{
    //적 스탯-----------------------------
    //HP
    private float maxHP;
    private float HP;

    //피해감소율
    private float reduce;

    //주 무기
    private WeaponInstance weapon;

    //순찰 지점
    public List<Vector3> patrolPoint;

    //상태
    [SerializeField]
    private EnemyState state;

    //네비메쉬
    public NavMeshAgent agent;
    public int patrolIndex;
    public bool patrolCheck;

    //시야---------------------------------
    //감각 범위
    public SphereCollider senseCollider;

    //시야각(120도)
    private float fov = 60f;

    //시야 회전 속도
    public float neckRotationSpeed;

    //시야 거리
    public float viewDistance;

    //소리 거리
    public float soundPickDistance;

    //경계---------------------------------------
    //경계 지속 시간
    private float alertTime = 5f;

    //타겟--------------------------------------
    public Transform targetTransform;

    //적 기능-----------------------------
    //피격
    //상태 변경

    //
    //이동
    //사격/재장전
    //사망

    //감지 스크립트------------------------------------
    //시야 감지
    //소리 감지

    //★우선적으로 해야 할 것 : 경계, 순회


    void Start()
    {
        Init();
    }

    public void Init()
    {
        state = EnemyState.patrol;

        patrolPoint = new List<Vector3>();
        patrolPoint.Add(new Vector3(-3f, 0f, 6f));
        patrolPoint.Add(new Vector3(3f, 0f, 6f));
        patrolPoint.Add(new Vector3(11f, 0f, 6f));

        agent.SetDestination(patrolPoint[patrolIndex]);

        senseCollider.radius = viewDistance;
    }

    void Update()
    {
        switch (state)
        {
            case EnemyState.patrol:
                UpdatePatrol();
                break;
            case EnemyState.alert:
                UpdateAlert();
                break;
            case EnemyState.watch:
                UpdateWatch();
                break;
            case EnemyState.movement:
                UpdateMovement();
                break;
        }
    }

    //순회 상태 행동
    void UpdatePatrol()
    {
        //순회 상태부터------------------------------------------------------------------
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (patrolIndex == patrolPoint.Count - 1)
                patrolCheck = true;
            else if (patrolIndex == 0)
                patrolCheck = false;

            if (!patrolCheck)
            {
                patrolIndex++;
            }
            else
            {
                patrolIndex--;
            }

            agent.SetDestination(patrolPoint[patrolIndex]);
            agent.isStopped = true;
            state = EnemyState.alert;
        }
    }

    //경계 상태 행동
    void UpdateAlert()
    {
        //목 트랜스폼이 회전하면 감각콜리더도 같이 회전
        alertTime -= Time.deltaTime;
        if (alertTime > 0f)
        {
            float rotY = Mathf.Sin(Time.time * neckRotationSpeed) * fov;

            Debug.Log(rotY);
            senseCollider.transform.localRotation = Quaternion.Euler(0f, rotY, 0f);
        }
        else if (alertTime <= 0f)
        {
            senseCollider.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            alertTime = 5f;
            state = EnemyState.patrol;
            agent.isStopped = false;
        }
    }

    //주시 상태 행동
    void UpdateWatch()
    {

    }

    //이동 상태 행동
    void UpdateMovement()
    {

    }


}
