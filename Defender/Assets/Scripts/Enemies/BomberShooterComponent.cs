using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BomberShooterComponent : MonoBehaviour
{
    [SerializeField]
    private float minShootCooldown = .2f;

    [SerializeField]
    private float maxShootCooldown = 2f;

    [SerializeField]
    private int bulletDamage = 50;

    [SerializeField]
    public GameObject bulletPrefab;

    void Awake()
    {
        Assert.IsNotNull(bulletPrefab);
    }

    void Start()
    {
        InvokeShootWithCooldown();
    }

    private void InvokeShootWithCooldown()
    {
        float randomCd = Random.Range(minShootCooldown, maxShootCooldown);
        Invoke(nameof(Shoot), randomCd);
    }
    private void Shoot()
    {
        //Can do object pooling here, but a short lifespan with firing cooldown should be enough for now
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.GetComponent<SquidBulletBehaviour>().Fire(bulletDamage);
        bullet.GetComponent<EntityChunkComponent>().SetOwner(GetComponent<EntityChunkComponent>().GetOwner());
        GetComponent<EntityChunkComponent>().GetOwner().OnBulletSpawned(bullet);
        InvokeShootWithCooldown();
    }
}
