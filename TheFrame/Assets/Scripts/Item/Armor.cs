using System.Text;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public enum ArmorType
{
    lightArmor,
    heavyArmor
}

[CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/Armor")]
public class Armor : ItemData
{
    public ArmorType armorType;

    public float DamageReduce;         //피해 감소
    public float ExtraHealth;             //추가 체력
    public float MaxAmmo;                 //추가 탄약
    public float ExtraSpeed;              //속도 변화

}
