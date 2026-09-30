using UnityEngine;

public class EnemyDetectionZone : MonoBehaviour
{
    public bool isTargetDetected { get; private set; }
    public Transform targetTransform { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTargetDetected = true;
            targetTransform = other.transform;
        }
    }
}
