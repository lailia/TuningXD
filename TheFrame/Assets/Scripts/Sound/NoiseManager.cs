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

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void PlayNoise(NoiseData noiseData)
    {
        LayerMask enemyLayer = LayerMask.NameToLayer("Enemy");
        int layermask = (1 << enemyLayer);

        Collider[] hitColliders = Physics.OverlapSphere(noiseData.position, noiseData.radius, layermask);

        foreach (Collider collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            enemy.HasHeardSound(noiseData.position, noiseData.type);
        }
    }
}
