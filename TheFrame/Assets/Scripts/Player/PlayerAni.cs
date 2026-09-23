using UnityEngine;

public class PlayerAni : MonoBehaviour
{
    public AudioSource audioSource;

    [SerializeField]
    private int footStepCount = 0;

    public void FootStep()
    {
        //움직일때 사운드 넣기
        SoundManager.Instance.MakePlayerNoise(transform.position, 20f, SoundManager.Instance.footstepClip[footStepCount]);

        footStepCount++;
        if (footStepCount == SoundManager.Instance.footstepClip.Length)
            footStepCount = 0;


    }
}
