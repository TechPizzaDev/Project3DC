using System;
using System.Collections;
using Unity.VisualScripting;
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
    public AudioConfig audioConfig;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;
    private AudioSource shootingAudioSource;
    private Camera activeCamera;

    private float lastShootTime;
    private float initialClickTime;
    private float stopShootingTime;
    private bool lastFrameWantedToShoot;

    private ParticleSystem shootSystem;
    private ObjectPool<Bullet> bulletPool;
    private VisualEffect muzzleFlash;
    private ObjectPool<TrailRenderer> trailPool;

    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour, Camera activeCamera = null)
    {
        this.activeMonoBehaviour = activeMonoBehaviour;
        this.activeCamera = activeCamera;

        lastShootTime = 0;
        ammoConfig.currentClipAmmo = ammoConfig.clipSize;
        ammoConfig.currentAmmo = ammoConfig.maxAmmo;

        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        if (!shootConfig.isHitscan)
        {
            bulletPool = new ObjectPool<Bullet>(CreateBullet);
        }

        if (modelPrefab != null)
        {
            model = Instantiate(modelPrefab);
        }
        else
        {
            //model = ;   
        }

        //model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        shootSystem = model.GetComponentInChildren<ParticleSystem>();
        shootingAudioSource = model.GetComponent<AudioSource>();
        muzzleFlash = model.GetComponentInChildren<VisualEffect>();
    }

    public void UpdateCamera(Camera activeCamera)
    {
        this.activeCamera = activeCamera;
    }

    public void TryToShoot()
    {
        if (Time.time > shootConfig.fireRate + lastShootTime)
        {
            lastShootTime = Time.time;
            if (ammoConfig.currentClipAmmo == 0)
            {
                audioConfig.PlayOutOfAmmoClip(shootingAudioSource);
                return;
            }

            shootSystem.Play();
            audioConfig.PlayShootingClip(shootingAudioSource, ammoConfig.currentClipAmmo == 1);
            muzzleFlash.Play();

            Vector3 spreadAmount = shootConfig.GetSpread();
            Vector3 shootDirection = Vector3.zero;

            if (shootConfig.shootType == ShootType.fromGun)
            {
                shootDirection = shootSystem.transform.forward;
            }
            else
            {
                shootDirection = activeCamera.transform.forward
                    + activeCamera.transform.TransformDirection(shootDirection);
            }

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
                GetRaycastOrigin(),
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

        if (shootConfig.shootType == ShootType.fromCamera
            && Physics.Raycast(GetRaycastOrigin(), 
                shootDirection,
                out RaycastHit hit,
                float.MaxValue,
                shootConfig.hitMask))
        {
            Vector3 directionToHit = (hit.point - shootSystem.transform.position).normalized;
            model.transform.forward = directionToHit;
            shootDirection = directionToHit;
        }

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

    public Vector3 GetRaycastOrigin()
    {
        Vector3 origin = shootSystem.transform.position;

        if (shootConfig.shootType == ShootType.fromCamera)
        {
            origin = activeCamera.transform.position
                + activeCamera.transform.forward
                * Vector3.Distance(
                    activeCamera.transform.position,
                    shootSystem.transform.position
                );
        }

        return origin;
    }

    public Vector3 GetGunForward()
    {
        return model.transform.forward;
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
            //Debug.Log("Hit Damageable");
            damageable.TakeDamage(damageConfig.GetDamage(distanceTraveled));
            damageable.Detection();
        }
    }

    public bool CanReload()
    {
        return ammoConfig.CanReload();
    }

    public void StartReloading()
    {
        audioConfig.PlayReloadClip(shootingAudioSource);
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
            TryToShoot();
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
        config.audioConfig = audioConfig.Clone() as AudioConfig;

        config.modelPrefab = modelPrefab;
        config.spawnPoint = spawnPoint;
        config.spawnRotation = spawnRotation;

        return config;
    }
}
