using UnityEngine;

public class CoverObject : MonoBehaviour
{
    public Transform[] coverPoints;

    private void OnDrawGizmos()
    {
        if (coverPoints == null)
            return;

        Gizmos.color = Color.green;

        foreach(Transform point in coverPoints)
        {
            if (point == null)
                return;

            Gizmos.DrawSphere(point.position, 0.2f);
        }
    }
}
