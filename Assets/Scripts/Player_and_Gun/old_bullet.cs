using UnityEngine;

public class old_Bullet : MonoBehaviour
{
    [SerializeField]
    private int maxBounceCount;

    public int MaxBounceCount
    {
        get { return maxBounceCount; }
        set { maxBounceCount = value; }
    }

    [SerializeField]
    private int bounceCount;

    public GameObject? caster;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out UnitHealth health))
        {
            health.DealDamage(10f);
        }

        if (other.gameObject.TryGetComponent(out EnemyDetection detection))
        {
            detection.AlertOnHit(caster);
        }

        bounceCount++;

        if (bounceCount >= maxBounceCount)
        {
            DestroyBullet();
        }
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
