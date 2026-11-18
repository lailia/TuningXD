using UnityEngine;

public class EnemyCoverAI : MonoBehaviour
{
    public LayerMask coverLayer;

    //엄폐물 찾기
    public Transform FindCover(float detectRadius, Transform targetPoint)
    {
        //근처 엄폐물들
        Collider[] covers = Physics.OverlapSphere(
            transform.position, 
            detectRadius, 
            coverLayer);

        //제일 좋은 엄폐 위치
        Transform bestPoint = null;
        float bestScore = float.MaxValue;

        foreach(Collider cover in covers)
        {
            CoverObject coverObject = cover.GetComponent<CoverObject>();

            if (coverObject == null)
                continue;

            foreach(Transform point in coverObject.coverPoints)
            {
                if (point == null)
                    continue;

                //엄폐 위치에서 타겟까지 시야 검사
                Vector3 direction = targetPoint.position - point.position;

                //bool blocked = Physics.Raycast()  //  //  //  //  //  //  //  //  //  //
            }
        }
    }

}
