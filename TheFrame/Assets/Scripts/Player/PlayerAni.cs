using UnityEngine;

public class PlayerAni : MonoBehaviour
{
    public void FootStep()
    {
        //움직일때 사운드 넣기
        SoundManager.Instance.PlaySFX(SoundManager.Instance.FootStepClips());
        NoiseManager.Instance.PlayNoise(new NoiseData(transform.position, 10f, noiseType.run));
    }
}
