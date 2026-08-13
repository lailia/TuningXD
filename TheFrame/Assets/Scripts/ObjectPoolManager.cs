using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [Header("BulletPool")]
    [SerializeField]
    private Dictionary<GameObject, ObjectPool<GameObject>> poolDic = new();

    private int defaultSize = 30;

    //오브젝트 풀 새로 생성하는 함수
    public ObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            () => Instantiate(prefab),
            obj => obj.SetActive(true),
            obj => obj.SetActive(false),
            obj => Destroy(obj),
            true,
            defaultSize
            );
    }

    public GameObject GetObject(GameObject prefab)
    {
        GameObject obj;
        if (poolDic.ContainsKey(prefab))
        {
            obj = poolDic[prefab].Get();
        }
        else
        {
            poolDic.Add(prefab, CreatePool(prefab));
            obj = poolDic[prefab].Get();
        }
        return obj;
    }

    public GameObject GetObject(GameObject prefab, out ObjectPool<GameObject> pool)
    {
        GameObject obj;
        if (!poolDic.ContainsKey(prefab))
        {
            pool = CreatePool(prefab);
            poolDic.Add(prefab, pool);
        }
        else
        {
            pool = poolDic[prefab];
        }

        obj = poolDic[prefab].Get();

        return obj;
    }

    public void ReleaseObject(GameObject prefab, GameObject obj)
    {
        poolDic[prefab].Release(obj);
    }
}
