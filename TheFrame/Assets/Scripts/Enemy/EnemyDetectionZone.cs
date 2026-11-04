using UnityEngine;

public class EnemyDetectionZone : MonoBehaviour
{
    //눈 위치
    public Transform eyeTransform;


    //타겟이 발견됐는지 확인
    public bool isTargetDetected { get; private set; }

    //발견되었던 타겟이 사라졌는지 확인
    private bool isTargetCheck = false;

    //감지 범위 안에 적이 있는지 확인
    [SerializeField]
    private bool isTargetInDetectZone = false;

    //타겟의 마지막 위치
    public Vector3 targetLastPos;

    //타겟의 transform
    public Transform targetTransform { get; private set; }

    //타겟의 머리, 몸통 위치
    [SerializeField]
    private Transform targetHeadTransform;
    [SerializeField]
    private Transform targetBodyTransform;

    //시야
    public SphereCollider detectionCollider;

    //시야각(120도)
    private float fov;

    private float detectTime;

    private void Update()
    {
        TargetLastPos();

        if (isTargetInDetectZone)
            CheckTargetDetect();
    }

    public void Init(float viewDistance)
    {
        detectionCollider.radius = viewDistance;
        fov = 60f;
        detectTime = 0.2f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetTransform = other.transform;
            targetHeadTransform = targetTransform.Find("Head").GetComponent<Transform>();
            targetBodyTransform = targetTransform.Find("Body").GetComponent<Transform>();
            isTargetInDetectZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            targetTransform = null;
            targetHeadTransform = null;
            targetBodyTransform = null;
            isTargetDetected = false;
            isTargetInDetectZone = false;
        }
    }

    //target 발견했는지 확인하는 메서드
    public void CheckTargetDetect()
    {
        if (detectTime <= 0f)
        {
            Vector3 direction = targetTransform.position - transform.position;
            direction.y = 0f;

            float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

            if (angle <= fov && angle >= -fov)
            {
                //타겟이 시야 각 내에 있다면 사이에 장애물이 있는지 확인
                isTargetDetected = CanSeePlayer();
            }
            else
            {
                isTargetDetected = false;
            }
            detectTime = 0.2f;
        }
        else
        {
            detectTime -= Time.deltaTime;
        }
    }

    //플레이어의 머리와 몸통을 발견했는가
    bool CanSeePlayer()
    {
        return CanSeePoint(targetHeadTransform) || CanSeePoint(targetBodyTransform);
    }

    bool CanSeePoint(Transform target)
    {
        Vector3 direction = target.position - eyeTransform.position;

        if (Physics.Raycast(eyeTransform.position, direction.normalized, out RaycastHit hit))
        {
            return hit.transform.tag == "Player";
        }
        return false;
    }

    public void TargetLastPos()
    {
        //타겟를 발견할 때 타겟 체크가 false라면 한번만 타겟 체크 true
        if (isTargetDetected && !isTargetCheck)
            isTargetCheck = true;

        //타겟 체크가 true일 때 타겟을 놓치면 targetLastPos 설정
        if(isTargetCheck && !isTargetDetected)
        {
            targetLastPos = targetTransform.position;
        }
    }
}
