using UnityEngine;

public class EnemyDetectionZone : MonoBehaviour
{
    //눈 위치
    public Transform eyeTransform;

    //타겟을 시야로 발견했을 때 확인
    public bool isTargetDetected { get; private set; }

    //발견되었던 타겟이 사라졌는지 확인
    private bool isTargetLost = false;      //여기 이름 변경하기/////////////////////////////////

    //감지 범위 안에 적이 있는지만 확인
    [SerializeField]
    private bool isTargetInDetectZone = false;

    //타겟의 마지막 위치
    public Vector3 targetLastPos;

    //타겟의 transform
    public Transform targetTransform;

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
            if (isTargetLost)
            {
                targetLastPos = targetTransform.position;
                isTargetLost = false;
            }
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

            if (isTargetDetected && !isTargetLost)
            {
                isTargetLost = true;
            }
            else if (!isTargetDetected && isTargetLost)
            {
                targetLastPos = targetTransform.position;
                isTargetLost = false;
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
}
