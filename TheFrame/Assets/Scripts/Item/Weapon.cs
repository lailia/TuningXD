using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

public enum WeaponType
{
    rifle,
    shotgun,
    sniper
}

[CreateAssetMenu(fileName ="Weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ItemData
{
    public WeaponType weaponType;

    public float Damage;                   //공격력
    public float Rpm;                     //연사력
    public float Recoil;                 //반동
    public float Accuracy;                //명중률
    public float Magazine;               //탄창 용량
    public float Pellet;                    //한번 발사 시 발사될 총알 갯수
    public float Range;                   //사거리
    public float ReloadTime;              //재장전 시간
    public float Noise;                   //소음거리

    //엑셀같은 데이터 저장공간에서 데이터 가져와서 변수에 대입하는 방법 생각

}
