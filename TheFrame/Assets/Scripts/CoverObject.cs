using UnityEngine;

public class CoverObject : MonoBehaviour
{
    public CoverPoint[] coverPoints;

    private void OnDrawGizmos()
    {
        if (coverPoints == null)
            return;

        Gizmos.color = Color.green;

        foreach(CoverPoint point in coverPoints)
        {
            if (point == null)
                return;

            Gizmos.DrawSphere(point.CoverPosition, 0.2f);
        }
    }
}
