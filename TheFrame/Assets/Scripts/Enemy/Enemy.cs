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
    private List<Vector3> patrolPoint;
    //상태
    [SerializeField]
    private EnemyState state;
    //시야각(120도)
    private float fov;
    //청각 범위
    private SphereCollider sphereCollider;

    //네비메쉬
    public NavMeshAgent agent;
    public int patrolIndex;
    public bool patrolCheck;

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
        patrolPoint = new List<Vector3>();
        patrolPoint.Add(new Vector3(-3f, 0f, 6f));
        patrolPoint.Add(new Vector3(3f, 0f, 6f));
        patrolPoint.Add(new Vector3(11f, 0f, 6f));

        agent.SetDestination(patrolPoint[patrolIndex]);
    }

    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (patrolIndex == 2)
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
        }
    }

    //순회 상태 행동
    void UpdatePatrol()
    {
        //순회 상태부터------------------------------------------------------------------
        //if(!agent.pathPending&&agent.hasPath)
    }

    //경계 상태 행동
    void UpdateAlert()
    {

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
