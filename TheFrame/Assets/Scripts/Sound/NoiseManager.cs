using UnityEngine;

public enum noiseType
{
    run,
    fire
}

public struct NoiseData
{
    public Vector3 position;
    public float radius;
    public noiseType type;

    public NoiseData(Vector3 position, float radius, noiseType type)
    {
        this.position = position;
        this.radius = radius;
        this.type = type;
    }
}

public class NoiseManager : MonoBehaviour
{
    private static NoiseManager instance;
    public static NoiseManager Instance => instance;

    public Collider[] colliders = new Collider[10];

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void PlayNoise(NoiseData noiseData)
    {
        LayerMask enemyLayer = LayerMask.NameToLayer("Enemy");
        int layermask = (1 << enemyLayer);

        //Physics.OverlapSphereNonAlloc를 사용하여 배열을 미리 만들어 두는 방법이 좋아보임
        Collider[] hitColliders = Physics.OverlapSphere(noiseData.position, noiseData.radius, layermask);

        foreach (Collider collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            enemy.HeardNoise(noiseData.position, noiseData.type);
        }
    }
}
