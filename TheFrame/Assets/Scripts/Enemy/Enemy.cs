using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum EnemyMission
{
    patrol,
    search,
    watchGuard,
    battle
}

public enum EnemyState
{
    alert,                  //경계
    round,                 //순회
    watch,                  //주시
    movement,               //이동
    attack,                 //공격
    reload,                 //재장전
    tracking,               //추적
    lastLocationWatch,      //마지막 위치 주시
    runaway                 //도망
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

    //상태
    [SerializeField]
    private EnemyMission mission;
    [SerializeField]
    private EnemyState state;

    //이동 관련----------------------------------------------------
    //순찰 지점
    public List<Vector3> patrolPoint;

    //네비메쉬
    public NavMeshAgent agent;
    public int patrolIndex;
    public bool patrolCheck;

    //감각------------------------------------------
    //감각 범위
    public SphereCollider senseCollider;

    //시야각(120도)
    private float fov = 60f;

    //시야 회전 속도
    public float neckRotationSpeed;

    //시야 거리
    public float viewDistance;

    //소리 감지
    private bool isSoundHeard;
    public Vector3 soundHeardPos;

    //경계---------------------------------------
    //경계 지속 시간
    private float alertTime = 5f;

    //타겟--------------------------------------
    public Transform targetTransform;

    //애니메이션------------------------------------------
    public Animator ani;

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
        // mission = 순찰 임무;
    }

    public void Init()
    {
        state = EnemyState.round;

        patrolPoint = new List<Vector3>();
        patrolPoint.Add(new Vector3(-3f, 0f, 6f));
        patrolPoint.Add(new Vector3(3f, 0f, 6f));
        patrolPoint.Add(new Vector3(11f, 0f, 6f));

        agent.SetDestination(patrolPoint[patrolIndex]);
        ani.SetFloat("Y", 1f);

        senseCollider.radius = viewDistance;
    }

    void Update()
    {
        switch(mission)
        {
            case EnemyMission.patrol:
                PatrolMission();
                break;
            case EnemyMission.search:
                SearchMission();
                break;
            case EnemyMission.watchGuard:
                break;
            case EnemyMission.battle:
                break;
        }

        //스테이트를 넣은 이유
        switch (state)
        {
            case EnemyMission.patrol:
                PatrolMission();
                break;
            case EnemyMission.search:
                SearchMission();
                break;
            case EnemyMission.watchGuard:
                break;
            case EnemyMission.battle:
                break;
        }

        //if(순찰 임무)
        //{
        // 순찰 임무에 대한 코드(ex. 순찰 지점으로 이동)
        // navMe

        // 탐색, 전투
        //if(소리를 들었다)
        //  mission = 탐색 임무;

        //if(플레이어를 발견)
        //  mission = 전투 임무;
        //}
        //else if(탐색 임무)
        //{

        //}
        //else if(전투 임무)
        //{
        //  플레이어를 향해 조준!
        //
        //  if(플레이어를 놓침!)
        //      mission = 순찰 임무
        //}
    }

    void PatrolMission()
    {
        switch (state)
        {
            case EnemyState.round:

                UpdateRound();
                break;
            case EnemyState.alert:
                UpdateAlert(EnemyState.round, EnemyMission.patrol);
                break;
        }
    }

    void SearchMission()
    {
        switch (state)
        {
            //여기서 상태 기능을 실행하는게 아니라 시작이나 끝나는 조건을 넣어서 관리하는걸 생각
            case EnemyState.watch:
                UpdateWatch();
                break;
            case EnemyState.movement:
                UpdateMovement();
                break;
            case EnemyState.alert:
                UpdateAlert(EnemyState.round, EnemyMission.patrol);
                break;
        }
    }

    //순찰 미션--------------------------------------------------------------------------
    void UpdateRound()
    {
        //네비메쉬 에이전트가 목표 위치 계산 중인지, 목표 위치가 정해졌는지, 목표 위치와의 거리가 정지 거리보다 작은지 확인
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

            agent.isStopped = true;
            ani.SetFloat("Y", 0f);
            state = EnemyState.alert;
        }
    }

    //경계 상태 행동
    void UpdateAlert(EnemyState changeState, EnemyMission changeMission)
    {
        //목 트랜스폼이 회전하면 감각콜리더도 같이 회전
        alertTime -= Time.deltaTime;
        if (alertTime > 0f)
        {
            float rotY = Mathf.Sin(Time.time * neckRotationSpeed) * fov;

            senseCollider.transform.localRotation = Quaternion.Euler(0f, rotY, 0f);
        }
        else if (alertTime <= 0f)
        {
            senseCollider.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            alertTime = 5f;
            agent.SetDestination(patrolPoint[patrolIndex]);
            agent.isStopped = false;
            ani.SetFloat("Y", 1f);
            state = changeState;
            mission = changeMission;
        }
    }

    //탐색 미션----------------------------------------------------------------------------
    //주시 상태 행동
    void UpdateWatch()
    {
        ani.SetFloat("Y", 1f);
        Vector3 soundDir = (soundHeardPos - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(soundDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.deltaTime);

        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        //만약 소리가 들린 방향과 현재 바라보는 방향의 차이가 1도 미만이면 state를 movement로 바꿈
        if (angle < 1f)
        {
            state = EnemyState.movement;
            agent.SetDestination(soundHeardPos);
            agent.isStopped = false;
        }
    }

    //이동 상태 행동
    void UpdateMovement()
    {
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            state = EnemyState.alert;
            agent.isStopped = true;
            ani.SetFloat("Y", 0f);
        }
    }

    public void HasHeardSound(Vector3 heardSoundPos)
    {
        agent.isStopped = true;
        state = EnemyState.watch;
        mission = EnemyMission.search;
        isSoundHeard = true;
        soundHeardPos = heardSoundPos;
    }

}
