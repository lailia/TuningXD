using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public float speed;
    private float destroyTime = 1f;
    private float currentTime = 0f;

    private GameObject bulletPrefab;
    private ObjectPoolManager poolManager;

    void Start()
    {

    }

    public void Init(GameObject bulletPrefab, ObjectPoolManager poolManager)
    {
        this.bulletPrefab = bulletPrefab;
        this.poolManager = poolManager;
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        currentTime += Time.deltaTime;
        if (currentTime >= destroyTime  )
        {
            poolManager.ReleaseObject(bulletPrefab, gameObject);
            currentTime = 0f;
        }
    }

}
