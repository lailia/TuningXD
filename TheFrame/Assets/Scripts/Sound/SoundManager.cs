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

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            Debug.Log("소리 출력 실패");
            return;
        }
        sfxSource.PlayOneShot(clip);

    }

    int footStepCount = 0;

    public AudioClip FootStepClips()
    {
        footStepCount++;
        if (footStepCount == footstepClip.Length)
            footStepCount = 0;

        return footstepClip[footStepCount];
    }
}
