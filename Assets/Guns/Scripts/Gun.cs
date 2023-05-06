using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class Gun : ScriptableObject, ICloneable
{
    public GunType type;
    public string gunName;
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;

    public DamageConfig damageConfig;
    public AmmoConfig ammoConfig;
    public ShootConfig shootConfig;
    public TrailConfig trailConfig;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;

    private float lastShootTime;
    private float initialClickTime;
    private float stopShootingTime;
    private bool lastFrameWantedToShoot;

    private ParticleSystem shootSystem;
    private ObjectPool<Bullet> bulletPool;
    private VisualEffect muzzleFlash;
    private ObjectPool<TrailRenderer> trailPool;

    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour)
    {
        this.activeMonoBehaviour = activeMonoBehaviour;
        lastShootTime = 0;
        ammoConfig.currentClipAmmo = ammoConfig.clipSize;
        ammoConfig.currentAmmo = ammoConfig.maxAmmo;

        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        if (!shootConfig.isHitscan)
        {
            bulletPool = new ObjectPool<Bullet>(CreateBullet);
        }

        model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        shootSystem = model.GetComponentInChildren<ParticleSystem>();
        muzzleFlash = model.GetComponentInChildren<VisualEffect>();
    }

    public void Shoot()
    {
        if (Time.time > shootConfig.fireRate + lastShootTime)
        {
            lastShootTime = Time.time;
            shootSystem.Play();
            muzzleFlash.Play();

            Vector3 spreadAmount = shootConfig.GetSpread();
            Vector3 shootDirection = model.transform.parent.forward + spreadAmount;

            ammoConfig.currentClipAmmo--;

            if (shootConfig.isHitscan)
            {
                DoHitscanShoot(shootDirection);
            }
            else
            {
                DoProjectileShoot(shootDirection);
            }
        }
    }

    private void DoHitscanShoot(Vector3 shootDirection)
    {
        if (Physics.Raycast(
                shootSystem.transform.position,
                shootDirection,
                out RaycastHit hit,
                float.MaxValue,
                shootConfig.hitMask
                ))
        {
            activeMonoBehaviour.StartCoroutine(
                PlayTrail(
                    shootSystem.transform.position,
                    hit.point,
                    hit
                )
            );
        }
        else
        {
            activeMonoBehaviour.StartCoroutine(
                PlayTrail(
                    shootSystem.transform.position,
                    shootSystem.transform.position + (shootDirection * trailConfig.missDistance),
                    new RaycastHit()
                )
            );
        }
    }

    private void DoProjectileShoot(Vector3 shootDirection)
    {
        Bullet bullet = bulletPool.Get();
        bullet.gameObject.SetActive(true);
        bullet.OnCollision += HandleBulletCollision;
        bullet.transform.position = shootSystem.transform.position;
        bullet.Spawn(shootDirection * shootConfig.bulletSpawnForce);

        TrailRenderer trail = trailPool.Get();
        if (trail != null)
        {
            trail.transform.SetParent(bullet.transform, false);
            trail.transform.localPosition = Vector3.zero;
            trail.emitting = true;
            trail.gameObject.SetActive(true);
        }

    }

    private void HandleBulletCollision(Bullet bullet, Collision collision)
    {
        TrailRenderer trail = bullet.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
        {
            trail.transform.SetParent(null, true);
            activeMonoBehaviour.StartCoroutine(DelayedDisableTrail(trail));
        }

        bullet.gameObject.SetActive(false);
        bulletPool.Release(bullet);

        if (collision != null)
        {
            ContactPoint contactPoint = collision.GetContact(0);

            HandleBulletImpact(
                Vector3.Distance(contactPoint.point, bullet.spawnLocation),
                contactPoint.point,
                contactPoint.normal,
                contactPoint.otherCollider);
        }
    }

    //stuff for when surface manager and damage is implemented
    private void HandleBulletImpact(
        float distanceTraveled,
        Vector3 hitLocation,
        Vector3 hitNormal,
        Collider hitCollider)
    {
        //SurfaceManager.Instance.HandleImpact(
        //    hitCollider.gameObject,
        //    hitLocation,
        //    hitNormal,
        //    ImpactType,
        //    0
        //);

        if (hitCollider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damageConfig.GetDamage(distanceTraveled));
        }
    }

    public bool CanReload()
    {
        return ammoConfig.CanReload();
    }

    public void EndReload()
    {
        ammoConfig.Reload();
    }

    public void Tick(bool wantsToShoot)
    {
        if (wantsToShoot)
        {
            lastFrameWantedToShoot = true;
            if (ammoConfig.currentClipAmmo > 0)
            {
                Shoot();
            }
        }
        else if (!wantsToShoot && lastFrameWantedToShoot)
        {
            stopShootingTime = Time.time;
            lastFrameWantedToShoot = false;
        }
    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null; //avoid position carry-over from last frame

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                    startPoint,
                    endPoint,
                    Mathf.Clamp01(1 - (remainingDistance / distance))
            );
            remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPoint;

        if (hit.collider != null)
        {
            HandleBulletImpact(distance, endPoint, hit.normal, hit.collider);
        }

        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    private IEnumerator DelayedDisableTrail(TrailRenderer trail)
    {
        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        trail.emitting = false;
        trail.gameObject.SetActive(false);
        trailPool.Release(trail);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.color;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    private Bullet CreateBullet()
    {
        return Instantiate(shootConfig.bulletPrefab);
    }

    public object Clone()
    {
        Gun config = CreateInstance<Gun>();

        //config.impactType = impactType;
        config.type = type;
        config.gunName = gunName;
        config.damageConfig = damageConfig.Clone() as DamageConfig;
        config.shootConfig = shootConfig.Clone() as ShootConfig;
        config.ammoConfig = ammoConfig.Clone() as AmmoConfig;
        config.trailConfig = trailConfig.Clone() as TrailConfig;
        //config.audioConfig = audioConfig.Clone() as AudioConfig;

        config.modelPrefab = modelPrefab;
        config.spawnPoint = spawnPoint;
        config.spawnRotation = spawnRotation;

        return config;
    }
}
