using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns.ImpactEffects
{
    public class Explode : ICollisionHandler
    {
        public float radius = 1;
        public AnimationCurve damageFalloff;
        public int baseDamage = 10;
        public int maxEnemiesEffected = 10;

        private Collider[] hitObjects;

        public Explode(float radius, AnimationCurve damageFalloff, int baseDamage, int maxEnemiesEffected)
        {
            this.radius = radius;
            this.damageFalloff = damageFalloff;
            this.baseDamage = baseDamage;
            this.maxEnemiesEffected = maxEnemiesEffected;
            hitObjects = new Collider[maxEnemiesEffected];
        }

        public void HandleImpact(Collider impactedObject, Vector3 hitPosition, Vector3 hitNormal, Gun gun)
        {
            int hits = Physics.OverlapSphereNonAlloc(
                hitPosition,
                radius,
                hitObjects,
                gun.shootConfig.hitMask);
            for (int i = 0; i < hits; i++)
            {
                if (hitObjects[i].TryGetComponent(out IDamageable damageable))
                {
                    float distance = Vector3.Distance(hitPosition, hitObjects[i].ClosestPoint(hitPosition));

                    damageable.TakeDamage(
                        Mathf.CeilToInt(baseDamage * damageFalloff.Evaluate(distance / radius))
                    );
                }
            }
        }
    }
}
    