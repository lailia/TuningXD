using System.Drawing;
using UnityEngine;

public class EnemyCoverAI : MonoBehaviour
{
    public LayerMask coverLayer;

    //엄폐물 찾기
    public CoverPoint FindCover(float detectRadius, Vector3 targetPoint)
    {
        //근처 엄폐물들
        Collider[] covers = Physics.OverlapSphere(
            transform.position,
            detectRadius,
            coverLayer);

        //제일 좋은 엄폐 위치
        CoverPoint bestPoint = null;
        float bestScore = float.MinValue;

        foreach (Collider cover in covers)
        {
            CoverObject coverObject = cover.GetComponent<CoverObject>();

            if (coverObject == null)
                continue;

            foreach (CoverPoint point in coverObject.coverPoints)
            {
                if (point == null || point.isOccupied)
                    continue;

                //타겟과 거리가 가장 먼 엄폐 포인트를 찾음
                float targetDis = Vector3.Distance(targetPoint, point.CoverPosition);
                float enemyDis = Vector3.Distance(transform.position, point.CoverPosition);

                float score = (targetDis * 1.5f) - enemyDis;

                if (bestScore < score)
                {
                    bestScore = score;
                    bestPoint = point;
                }
            }
        }

        bestPoint.isOccupied = true;

        return bestPoint;
    }

    //public CoverPoint FindCover(float detectRadius, Vector3 targetPoint)
    //{
    //    //근처 엄폐물들
    //    Collider[] covers = Physics.OverlapSphere(
    //        transform.position,
    //        detectRadius,
    //        coverLayer);

    //    //제일 좋은 엄폐 위치
    //    CoverObject bestObject = null;
    //    CoverPoint bestPoint = null;
    //    float bestScore = float.MaxValue;

    //    foreach (Collider cover in covers)
    //    {
    //        CoverObject coverObject = cover.GetComponent<CoverObject>();

    //        if (coverObject == null)
    //            continue;

    //        //자신과 거리가 가장 가까운 엄폐물 오브젝트를 찾음
    //        float score = Vector3.Distance(transform.position,
    //            coverObject.transform.position);

    //        if (bestScore > score)
    //        {
    //            bestObject = coverObject;
    //            bestScore = score;
    //        }
    //    }

    //    bestScore = float.MinValue;

    //    foreach (CoverPoint point in bestObject.coverPoints)
    //    {
    //        if (point == null || point.isOccupied)
    //            continue;

    //        //타겟과 거리가 가장 먼 엄폐 포인트를 찾음
    //        float score = Vector3.Distance(targetPoint, point.CoverPosition);

    //        if (bestScore < score)
    //        {
    //            bestScore = score;
    //            bestPoint = point;
    //        }
    //    }

    //    bestPoint.isOccupied = true;

    //    return bestPoint;
    //}

}
