using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum EnemyMission
{
    patrol,
    search,
    watchGuard,
    normalBattle,
    stationaryBattle
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
    public Weapon weaponData;

    //상태
    [SerializeField]
    private EnemyMission mission;
    [SerializeField]
    private EnemyState state;

    //상태 시작 확인
    private bool startState;

    //네비메쉬
    public NavMeshAgent agent;

    //애니메이션
    public Animator ani;

    //감각 범위
    public EnemyDetectionZone detectionZone;

    //무기 핸들러
    public EnemyWeaponHandler enemyWeaponHandler;

    //소리 감지
    public Vector3 noiseHeardPos;

    //경계 지속 시간
    private float alertTime = 5f;

    //순찰 미션 관련----------------------------------------------------
    //순찰 지점
    public List<Vector3> patrolPoint;
    public int patrolIndex;
    public bool patrolCheck;

    //전투 미션 관련--------------------------------------
    //마지막 위치
    private Vector3 targetPos;

    //적 기능-----------------------------
    //피격
    //상태 변경

    //이동
    //사격/재장전
    //사망

    //감지 스크립트------------------------------------
    //시야 감지
    //소리 감지

    void Start()
    {
        Init();
        // mission = 순찰 임무;
    }

    //테스트용 임의 설정
    public void Init()
    {
        state = EnemyState.round;

        patrolPoint = new List<Vector3>();
        patrolPoint.Add(new Vector3(5f, 0f, 20f));
        patrolPoint.Add(new Vector3(0f, 0f, 20f));

        detectionZone.Init(15f);

        WeaponInstance weapon = new WeaponInstance(weaponData);
        weapon.Init();

        enemyWeaponHandler.Init(weapon);
    }

    //EnemyManager에서 생성할 때 사용하게 상세 설정 가능하게
    //public void Init(List<Vector3> patrolPoint, float viewDistance, WeaponInstance weapon)
    //{
    //    state = EnemyState.round;

    //    this.patrolPoint = patrolPoint;

    //    agent.SetDestination(patrolPoint[patrolIndex]);
    //    ani.SetFloat("Y", 1f);

    //    detectionZone.Init(viewDistance);

    //    enemyWeaponHandler.Init(weapon);
    //}

    void Update()
    {
        switch (mission)
        {
            case EnemyMission.patrol:
                if (detectionZone.isTargetDetected)
                {
                    mission = EnemyMission.normalBattle;
                    state = EnemyState.attack;
                }
                break;
            case EnemyMission.search:
                if (detectionZone.isTargetDetected)
                {
                    mission = EnemyMission.normalBattle;
                    state = EnemyState.attack;
                }
                break;
            case EnemyMission.watchGuard:
                break;
            case EnemyMission.normalBattle:
                break;
        }

        //스테이트를 넣은 이유
        switch (state)
        {
            case EnemyState.alert:
                UpdateAlert();
                break;
            case EnemyState.round:
                UpdateRound();
                break;
            case EnemyState.watch:
                UpdateWatch();
                break;
            case EnemyState.movement:
                UpdateMovement();
                break;
            case EnemyState.attack:
                UpdateAttack();
                break;
            case EnemyState.reload:
                UpdateReload(Time.deltaTime);
                break;
            case EnemyState.tracking:
                break;
            case EnemyState.lastLocationWatch:
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

    //순찰 미션--------------------------------------------------------------------------
    void UpdateRound()
    {
        if(startState)
        {
            agent.SetDestination(patrolPoint[patrolIndex]);
            agent.isStopped = false;
            ani.SetFloat("Y", 1f);
            startState = false;
        }

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

            startState = true;
            state = EnemyState.alert;
        }
    }

    //경계 상태 행동
    void UpdateAlert()
    {
        if(startState)
        {
            alertTime = 5f;
            agent.isStopped = true;
            ani.SetFloat("Y", 0f);
            startState = false;
        }

        alertTime -= Time.deltaTime;
        if (alertTime > 0f)
        {
            //감각 콜리더 감지 각도 증가
        }
        else if (alertTime <= 0f)
        {
            //감각 콜리더 감지 각도 원상 복귀

            startState = true;
            switch (mission)
            {
                case EnemyMission.patrol:
                case EnemyMission.search:
                    mission = EnemyMission.patrol;
                    state = EnemyState.round;
                    break;
                case EnemyMission.watchGuard:
                    state = EnemyState.watch;
                    break;
            }
        }
    }

    //탐색 미션----------------------------------------------------------------------------
    //주시 상태 행동
    void UpdateWatch()
    {
        if (startState)
        {
            /////////////////////////////////////////////////////////////스테이트 시작 작성
        }

        ani.SetFloat("Y", 1f);
        Vector3 soundDir = noiseHeardPos - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(soundDir);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);

        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        //만약 소리가 들린 방향과 현재 바라보는 방향의 차이가 1도 미만이면 state를 movement로 바꿈
        if (angle < 1f)
        {
            agent.SetDestination(noiseHeardPos);
            agent.isStopped = false;
            alertTime = 5f;
            state = EnemyState.movement;
        }
    }

    //이동 상태 행동
    void UpdateMovement()
    {
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            ani.SetFloat("Y", 0f);
            state = EnemyState.alert;
        }
    }

    //공격 상태 실행
    void UpdateAttack()
    {
        if (detectionZone.isTargetDetected)
        {
            agent.isStopped = true;

            targetPos = detectionZone.targetTransform.position;

            ani.SetFloat("Y", 0f);

            Vector3 targetPoint = targetPos - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);

            if (enemyWeaponHandler.Fire(detectionZone.targetTransform.position))
            {
                ani.SetTrigger("Shoot");
            }

            if (enemyWeaponHandler.IsReload)
            {
                ani.SetBool("Reloading", true);
                state = EnemyState.reload;
            }
        }
        else
        {
            
        }
    }

    //재장전 상태 실행
    void UpdateReload(float deltaTime)
    {
        //타겟이 감지된 상태일 때
        if(detectionZone.isTargetDetected)
        {
            //혹시 이동 애니메이션이 실행되고 있었다면 이동 애니메이션 멈춤
            ani.SetFloat("Y", 0f);

            //타겟의 포지션 가져옴
            Vector3 targetPos = detectionZone.targetTransform.position;

            //타겟의 방향
            Vector3 targetPoint = targetPos - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint);

            //타겟을 바라봄
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);

            //재장전 중이 아니라면
            if (!enemyWeaponHandler.IsReload)
            {
                ani.SetBool("Reloading", false);
                state = EnemyState.attack;
            }
        }
    }

    //소음을 들었을 때 실행
    public void HeardNoise(Vector3 heardSoundPos, noiseType type)
    {
        switch(type)
        {
            case noiseType.run:
                break;
            case noiseType.fire:
                break;
        }

        agent.isStopped = true;
        //isNoiseHeard = true;
        //소리를 들었을 때 적 유형에 따라 미션 변경

        // switch(적 유형)
        mission = EnemyMission.search;
        state = EnemyState.watch;

        noiseHeardPos = heardSoundPos;
    }

    void DetectTarget()
    {
        //detectionzone의 isdetect와 battleswitch가 다르면 실행
        //적을 발견했을 때 한번만 실행할 위치
        //미션을 적 유형에 따라 배틀로 변경, 상태를 공격으로 변경

        //bission = battle, state = attack
        //공격 할 적의 위치 받기
        //공격할 적의 transform, 적이 사라졌을 때의 마지막 위치를 저장할 position

        agent.isStopped = true;

        //switch(적 유형)
        mission = EnemyMission.normalBattle;
        state = EnemyState.attack;

    }

}
