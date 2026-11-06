using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

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

    //해당 위치로 이동해야하는지 체크
    private bool isMoving;
    //대상을 바라봐야하는지 체크
    private bool isRotation;

    //회전에 사용될 변수들
    private float angle = 0f;
    private Vector3 targetDir = new Vector3();
    private Quaternion targetRotation = new Quaternion();

    //주 무기
    public Weapon weaponData;

    //상태
    [SerializeField]
    private EnemyMission mission;
    [SerializeField]
    private EnemyState state;

    //상태 시작 확인
    [SerializeField]
    private bool startState;

    //네비메쉬
    public NavMeshAgent agent;

    //애니메이션
    public Animator ani;

    //감각 범위
    public EnemyDetectionZone detectionZone;

    //무기 핸들러
    public EnemyWeaponHandler enemyWeaponHandler;

    //소리 감지 위치
    public Vector3 noiseHeardPos;

    //경계 지속 시간
    private float alertTime = 5f;

    //순찰 미션 관련----------------------------------------------------
    //순찰 지점
    public List<Vector3> patrolPoint;
    public int patrolIndex;
    public bool patrolCheck;

    //전투 미션 관련--------------------------------------
    //현재 갈 위치
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
        state = EnemyState.alert;

        patrolPoint = new List<Vector3>();
        patrolPoint.Add(new Vector3(0f, 0f, 20f));
        patrolPoint.Add(new Vector3(5f, 0f, 20f));

        startState = true;

        //적을 순찰지점 마지막 위치로 이동
        transform.position = patrolPoint[patrolPoint.Count - 1];

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
            case EnemyMission.search:
                if (detectionZone.isTargetDetected)
                {
                    mission = EnemyMission.normalBattle;
                    state = EnemyState.attack;
                    startState = true;
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
            case EnemyState.attack:
                UpdateAttack();
                break;
            case EnemyState.reload:
                UpdateReload(Time.deltaTime);
                break;
            case EnemyState.tracking:
                UpdateTracking();
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
    //경계 상태 행동
    void UpdateAlert()
    {
        if(startState)
        {
            //경계 시간 초기화
            alertTime = 5f;
            //이동 멈춤
            agent.isStopped = true;
            //이동 애니메이션 멈춤
            ani.SetFloat("Y", 0f);
            ani.SetBool("Alert", true);
            //처음 시작 조건 멈춤
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

            ani.SetBool("Alert", false);
            startState = true;
            switch (mission)
            {
                case EnemyMission.patrol:
                case EnemyMission.search:
                case EnemyMission.normalBattle:
                    mission = EnemyMission.patrol;
                    state = EnemyState.round;
                    break;
                case EnemyMission.watchGuard:
                    state = EnemyState.watch;
                    break;
            }
        }
    }

    //순회 상태 행동
    void UpdateRound()
    {
        //시작할 때 한번만 실행이 필요할 때
        //if (startState)
        //{
        //    startState = false;
        //}

        Vector3 nowPatrolPoint = patrolPoint[patrolIndex];

        SetTargetPosition(nowPatrolPoint);

        //이동 중인지 체크 후 이동이 끝났다면 경계로 변경
        if (CheckArrival(nowPatrolPoint))
        {
            //패트롤 인덱스 변경
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

    //탐색 미션----------------------------------------------------------------------------
    //주시 상태 행동

    void UpdateWatch()
    {
        //시작 할 때 한번만
        if (startState)
        {
            ani.SetBool("Alert", false);
            startState = false;
        }

        //만약 소리가 들린 방향과 현재 바라보는 방향의 차이가 1도 미만이면 state를 movement로 바꿈
        if (!TargetRotation(noiseHeardPos))
        {
            startState = true;
            targetPos = noiseHeardPos;
            state = EnemyState.tracking;
        }
    }

    void UpdateTracking()
    {
        if (startState)
        {
            ani.SetBool("Alert", false);
            startState = false;
        }

        if (detectionZone.isTargetDetected)
        {
            startState = true;
            state = EnemyState.attack;
            return;
        }

        SetTargetPosition(targetPos);

        if (CheckArrival(targetPos)) // 추적하다가 '목적지에 도달하면' 경계 상태가 된다.
        {
            startState = true;
            state = EnemyState.alert;
        }
    }

    //공격 상태 실행
    void UpdateAttack()
    {
        if (startState)
        {
            ani.SetFloat("Y", 0f);
            ani.SetBool("Alert", false);
            agent.isStopped = true;
            startState = false;
        }

        //만약 타겟 감지가 실패하면
        if (!detectionZone.isTargetDetected)
        {
            targetPos = detectionZone.targetLastPos;
            state = EnemyState.tracking;
            startState = true;
            return;
        }

        //회전이 필요하면 회전
        TargetRotation(detectionZone.targetTransform.position);

        //발사가 가능하면 발사
        if (enemyWeaponHandler.Fire(detectionZone.targetTransform.position))
        {
            ani.SetTrigger("Shoot");
        }

        //재장전이 필요하면 실행
        if (enemyWeaponHandler.IsReload)
        {
            state = EnemyState.reload;
            startState = true;
        }
    }

    //재장전 상태 실행
    void UpdateReload(float deltaTime)
    {
        if (startState)
        {
            //혹시 이동 애니메이션이 실행되고 있었다면 이동 애니메이션 멈춤
            ani.SetFloat("Y", 0f);
            ani.SetBool("Alert", false);
            ani.SetBool("Reloading", true);
            agent.isStopped = true;
            startState = false;
        }

        //만약 타겟 감지가 실패하면
        if (!detectionZone.isTargetDetected)
        {
            ani.SetBool("Reloading", false);
            targetPos = detectionZone.targetLastPos;
            startState = true;
            state = EnemyState.tracking;
            return;
        }

        //회전이 필요하면 회전
        TargetRotation(detectionZone.targetTransform.position);

        //재장전이 끝나면
        if (!enemyWeaponHandler.IsReload)
        {
            ani.SetBool("Reloading", false);
            startState = true;
            state = EnemyState.attack;
        }
    }

    //목적지를 설정하여 이동하는 함수
    void SetTargetPosition(Vector3 targetPos)
    {
        //만약 설정 위치가 있으며 그 위치와 받아온 위치가 다르면 목적지 재설정
        if (agent.hasPath && (agent.destination != targetPos))
        {
            isMoving = false;
        }

        //움직이는 중이 아니라면
        if (!isMoving)
        {
            //위치 설정
            agent.SetDestination(targetPos);
            //멈춤 상태 해제
            agent.isStopped = false;
            //이동 애니메이션
            ani.SetFloat("Y", 1f);
            isMoving = true;
        }
    }

    //설정한 목적지에 도착했는지 확인하는 함수
    bool CheckArrival(Vector3 pos)
    {
        //만약 설정한 위치에 도달했다면
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            //멈춤 상태 설정
            agent.isStopped = true;
            isMoving = false;
            return true;
        }

        return false;
    }

    //타겟을 바라보도록 회전
    bool TargetRotation(Vector3 pos)
    {
        //설정된 회전값과 받아온 회전값이 다르다면
        if (targetDir != (pos - transform.position))
        {
            //받아온 회전값으로 설정
            targetDir = pos - transform.position;
            targetRotation = Quaternion.LookRotation(targetDir);
            //이동 애니메이션 설정
            ani.SetFloat("Y", 1f);
            isRotation = true;
        }

        //만약 회전하는 중이라면
        if(isRotation)
        {
            //오브젝트의 회전값 변경
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);

            //각도 값 계산
            angle = Quaternion.Angle(transform.rotation, targetRotation);
        }

        //만약 각도가 1도 이하라면
        if (angle < 1f)
        {
            //이동 애니메이션 멈춤
            ani.SetFloat("Y", 0f);
            isRotation = false;
            return false;
        }
        return true;
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
        startState = true;
        //소리를 들었을 때 적 유형에 따라 미션 변경

        // switch(적 유형)
        mission = EnemyMission.search;
        state = EnemyState.watch;

        noiseHeardPos = heardSoundPos;
    }

}
