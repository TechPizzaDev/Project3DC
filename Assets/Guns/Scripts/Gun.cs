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

    /// <summary>
    /// Spawns the gun model and sets up the gun
    /// </summary>
    /// <param name="parent">Parent for the gun model</param>
    /// <param name="activeMonoBehaviour">An active MonoBehaviour that can have Coroutines attached</param>
    /// <param name="activeCamera">The camera to raycast from. Required if <see cref="ShootConfig.shootType"/> = <see cref="ShootType.fromCamera"/></param>
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
            Debug.Log(bulletPool);
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

    /// <summary>
    /// used to override the Camera provided in <see cref="Spawn(Transform, MonoBehaviour, Camera)"/>
    /// </summary>
    /// <param name="activeCamera"></param>
    public void UpdateCamera(Camera activeCamera)
    {
        this.activeCamera = activeCamera;
    }

    /// <summary>
    /// shoots the gun based on firerate. Also applies bullet spread, plays audio based on audioconfig and plays muzzleflash
    /// </summary>
    public void TryToShoot()
    {
        if (Time.time - lastShootTime - shootConfig.fireRate > Time.deltaTime)
        {
            float lastDuration = Mathf.Clamp(
                0,
                (stopShootingTime - initialClickTime),
                shootConfig.maxSpreadTime
            );
            float lerpTime = ( shootConfig.recoilRecoverySpeed - (Time.time - stopShootingTime))
                / shootConfig.recoilRecoverySpeed;

            initialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
            Debug.Log("initialclicktime " + initialClickTime);
        }

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

            Vector3 spreadAmount = shootConfig.GetSpread(Time.time - initialClickTime);
            Vector3 shootDirection = Vector3.zero;
            model.transform.forward += model.transform.TransformDirection(spreadAmount);
            if (shootConfig.shootType == ShootType.fromGun)
            {
                shootDirection = shootSystem.transform.forward;
            }
            else
            {
                shootDirection = activeCamera.transform.forward
                    + activeCamera.transform.TransformDirection(spreadAmount);
            }

            if (ammoConfig.ammoType != AmmoType.infinite)
            {
                ammoConfig.currentClipAmmo--;
            }

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

    /// <summary>
    /// Performs a raycast to determine if the bullet hits something. Spawns a trailrenderer and will apply
    /// damage after the trailrenderer has finished playing
    /// See <see cref="PlayTrail(Vector3, Vector3, RaycastHit)"/> for impact logic
    /// </summary>
    /// <param name="shootDirection"></param>
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

    /// <summary>
    /// Creates a bullet instance that is launched in the direction of <paramref name="shootDirection"/>
    /// with the velocity of <see cref="ShootConfig.bulletSpawnForce"/>
    /// </summary>
    /// <param name="shootDirection"></param>
    private void DoProjectileShoot(Vector3 shootDirection)
    {
        Bullet bullet = bulletPool.Get();
        bullet.gameObject.SetActive(true);
        bullet.OnCollision += HandleBulletCollision;

        //When shooting real projectiles from the camera the gun needs to be turned towards the hit point
        //of the raycast from the camera. Otherwise aim is off.
        if (shootConfig.shootType == ShootType.fromCamera
            && Physics.Raycast(GetRaycastOrigin(), 
                shootDirection,
                out RaycastHit hit,
                float.MaxValue,
                shootConfig.hitMask))
        {
            Debug.DrawRay(GetRaycastOrigin(), shootDirection * hit.distance, Color.red, 3f);
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

    /// <summary>
    /// returns the origin of the raycast based on the <see cref="ShootConfig.shootType"/>
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// returns the forward direction of the gun model
    /// </summary>
    /// <returns></returns>
    public Vector3 GetGunForward()
    {
        return model.transform.forward;
    }

    /// <summary>
    /// Callback handler for <see cref="Bullet.OnCollision"/>. Disables the TrailRenderer,
    /// releases the bullet from bulletpool and applies impact effects if <paramref name="collision"/> is not null.
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="collision"></param>
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
            Debug.Log(collision.gameObject.name);
            ContactPoint contactPoint = collision.GetContact(0);

            HandleBulletImpact(
                Vector3.Distance(contactPoint.point, bullet.spawnLocation),
                contactPoint.point,
                contactPoint.normal,
                contactPoint.otherCollider);
        }
    }

    /// <summary>
    /// Applies damage if a damageable object was hit
    /// </summary>
    /// <param name="distanceTraveled"></param>
    /// <param name="hitLocation"></param>
    /// <param name="hitNormal"></param>
    /// <param name="hitCollider"></param>
    private void HandleBulletImpact(
        float distanceTraveled,
        Vector3 hitLocation,
        Vector3 hitNormal,
        Collider hitCollider)
    {
        if (hitCollider.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log("Hit Damageable");
            damageable.TakeDamage(damageConfig.GetDamage(distanceTraveled));

            //if damageable is an enemy, call detection
            if (hitCollider.gameObject.layer == 11)
            {
                Debug.Log("Hit Enemy");
                damageable.Detection();
            }
        }
    }

    /// <summary>
    /// Checks if gun can be reloaded
    /// </summary>
    /// <returns></returns>
    public bool CanReload()
    {
        return ammoConfig.CanReload();
    }

    /// <summary>
    /// Plays the reloading audio clip if assigned.
    /// Expected to be called on the first frame of the reload animation.
    /// </summary>
    public void StartReloading()
    {
        audioConfig.PlayReloadClip(shootingAudioSource);
    }

    /// <summary>
    /// Handle ammo after reload animation
    /// </summary>
    public void EndReload()
    {
        ammoConfig.Reload();
    }

    /// <summary>
    /// Expected to be called every frame
    /// </summary>
    /// <param name="wantsToShoot">Whether the player is trying to shoot or not</param>
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

    /// <summary>
    /// Plays a bullet trail from the start point to the end point.
    /// </summary>
    /// <param name="startPoint">Starting point for trail</param>
    /// <param name="endPoint">End point for trail</param>
    /// <param name="hit">The hit object</param>
    /// <returns></returns>
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

    /// <summary>
    /// Disables the trail after <see cref="TrailConfig.duration"/> seconds and
    /// releases it from the <see cref="trailPool"/>
    /// </summary>
    /// <param name="trail"></param>
    /// <returns></returns>
    private IEnumerator DelayedDisableTrail(TrailRenderer trail)
    {
        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        trail.emitting = false;
        trail.gameObject.SetActive(false);
        trailPool.Release(trail);
    }

    /// <summary>
    /// Creates a trail renderer for use in the object pool
    /// </summary>
    /// <returns>A live TrailRenderer GameObject</returns>
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

    /// <summary>
    /// Creates a bullet for use in the object pool
    /// </summary>
    /// <returns>A live bullet GameObject</returns>
    private Bullet CreateBullet()
    {
        return Instantiate(shootConfig.bulletPrefab);
    }

    public object Clone()
    {
        Gun config = CreateInstance<Gun>();
        
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
