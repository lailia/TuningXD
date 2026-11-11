using UnityEngine;

public interface IDamageable
{
    void TakeDamage(DamageInfo damageInfo);
}

public struct DamageInfo
{
    public float damage;
    public Vector3 attackerPosition;

    public DamageInfo(float damage, Vector3 attackerPosition)
    {
        this.damage = damage;
        this.attackerPosition = attackerPosition;
    }
}
