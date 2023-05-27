using UnityEngine;

public interface IDamageable
{
    public int CurrentHealth { get; }
    public int MaxHealth { get; }

    public delegate void TakeDamageEvent(GameObject sender, int damage);
    public event TakeDamageEvent OnTakeDamage;

    public delegate void DeathEvent(GameObject sender, Vector3 position);
    public event DeathEvent OnDeath;

    public void TakeDamage(int damage);
    public void Detection();
}
