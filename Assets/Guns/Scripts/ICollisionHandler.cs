using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns.ImpactEffects
{
    public interface ICollisionHandler
    {
        void HandleImpact(
            Collider impactedObject,
            Vector3 hitPosition,
            Vector3 hitNormal,
            Gun gun
        );
    }
}
