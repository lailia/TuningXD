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

    private int footstepCount;

    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioSource bgmSource;

    public void FireSoundPlay(AudioSource _sfxSource)
    {
        sfxSource = _sfxSource;
        sfxSource.clip = fireClip;
        sfxSource.Play();
    }

    public void ReloadSoundPlay(AudioSource _sfxSource)
    {
        sfxSource = _sfxSource;
        sfxSource.clip = reloadClip;
        sfxSource.Play();
    }

    public void FootStepSoundPlay(AudioSource _sfxSource)
    {
        sfxSource = _sfxSource;
        sfxSource.clip = footstepClip[footstepCount];
        sfxSource.Play();
        footstepCount++;
        if (footstepCount >= footstepClip.Length)
            footstepCount = 0;
    }

    public void BGMSoundPlay(AudioSource _sfxSource)
    {
        sfxSource = _sfxSource;
        sfxSource.clip = bgmClip;
        sfxSource.Play();
    }

}
