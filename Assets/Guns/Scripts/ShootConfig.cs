using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Configuration", order = 2)]
public class ShootConfig : ScriptableObject, System.ICloneable
{
    public bool isHitscan = true;
    public Bullet bulletPrefab;
    public float bulletSpawnForce = 1000;
    public LayerMask hitMask;
    public float fireRate = 0.25f;

    public ShootType shootType = ShootType.fromGun;

    public float recoilRecoverySpeed = 1f;
    public float maxSpreadTime = 1f;
    public BulletSpreadType spreadType = BulletSpreadType.Simple;
    [Header("Simple Spread")]
    public Vector3 Spread = new Vector3 (0.1f, 0.1f, 0.1f);
    [Header("Texture-Based Spread")]
    [Range(0.001f, 5f)]
    public float spreadMultiplier = 0.1f;
    public Texture2D spreadTexture;


    public Vector3 GetSpread(float shootTime = 0)
    {
        Vector3 spread = Vector3.zero;
        if (spreadType == BulletSpreadType.Simple)
        {
            spread = Vector3.Lerp(
                Vector3.zero,
                new Vector3(
                    Random.Range(
                        -Spread.x,
                        Spread.x
                    ),
                    Random.Range(
                        -Spread.y,
                        Spread.y
                    ),
                    Random.Range(
                        -Spread.z,
                        Spread.z
                    )
                ),
                Mathf.Clamp01(shootTime / maxSpreadTime)
            );
        }
        else if (spreadType == BulletSpreadType.TextureBased)
        {
            spread = GetTextureDirection(shootTime);
            spread *= spreadMultiplier;
        }

        Debug.Log("spread " + spread);
        return spread;
    }

    private Vector3 GetTextureDirection(float shootTime)
    {
        Vector2 halfSize = new Vector2(spreadTexture.width / 2f, spreadTexture.height / 2f);
        int halfSquareExtents = Mathf.CeilToInt(
            Mathf.Lerp(
                1,
                halfSize.x,
                Mathf.Clamp01(shootTime / maxSpreadTime)
            )
        );

        int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
        int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

        Color[] sampleColors = spreadTexture.GetPixels(
            minX,
            minY,
            halfSquareExtents * 2,
            halfSquareExtents *2
        );

        float[] colorsAsGray = System.Array.ConvertAll(sampleColors, (color) => color.grayscale);
        float totalGrayValue = colorsAsGray.Sum();

        float gray = Random.Range(0, totalGrayValue);
        int i = 0;
        for (; i < colorsAsGray.Length; i++)
        {
            gray -= colorsAsGray[i];
            if (gray <= 0)
            {
                break;
            }
        }

        int x = minX + i % (halfSquareExtents * 2);
        int y = minY + i / (halfSquareExtents * 2);

        Vector2 targetPosition = new Vector2(x, y);
        Vector2 direction = (targetPosition - halfSize) / halfSize.x;

        return direction;
    }

    public object Clone()
    {
        ShootConfig config = CreateInstance<ShootConfig>();

        Utilities.CopyValues(this, config);

        return config;
    }

}
