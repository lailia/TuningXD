using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance => instance;

    void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioClip fireClip;
    public AudioClip reloadClip;
    public AudioClip[] footstepClip;
    public AudioClip bgmClip;

    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioSource bgmSource;

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(Vector3 sourcePos, float radius, AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            Debug.Log("소리 출력 실패");
            return;
        }
        sfxSource.PlayOneShot(clip);

        LayerMask enemyLayer = LayerMask.NameToLayer("Enemy");
        int layermask = (1 << enemyLayer);

        Collider[] hitColliders = Physics.OverlapSphere(sourcePos, radius, layermask);

        foreach (Collider collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            enemy.HasHeardSound(sourcePos);
        }
    }
}
