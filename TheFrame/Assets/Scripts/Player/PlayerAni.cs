using UnityEngine;

public class PlayerAni : MonoBehaviour
{
    public AudioSource audioSource;

    public void FootStep()
    {
        //움직일때 사운드 넣기
        Debug.Log("footstep");
        SoundManager.Instance.FootStepSoundPlay(audioSource);
    }
}
