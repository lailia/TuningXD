using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    public Vector3 CoverPosition => transform.position;
    public bool isOccupied;

    public void ReleaseOccupied()
    {
        isOccupied = false;
    }
}
