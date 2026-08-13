using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public float destroyTime;
    private float currentTime = 0f;

    private GameObject bulletHolePrefab;
    private ObjectPoolManager poolManager;
    public ParticleSystem particle;

    void Start()
    {
        
    }

    public void Init(GameObject bulletHoleObject,ObjectPoolManager poolManager)
    {
        this.bulletHolePrefab = bulletHoleObject;
        this.poolManager = poolManager;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime>=destroyTime)
        {
            poolManager.ReleaseObject(bulletHolePrefab, gameObject);
            currentTime = 0f;
        }
    }

    public void EffectPlay()
    {
        if (particle != null)
            particle.Play();
    }
}
