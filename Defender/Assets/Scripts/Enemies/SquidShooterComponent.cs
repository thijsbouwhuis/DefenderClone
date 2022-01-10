using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidShooterComponent : MonoBehaviour
{
    [SerializeField]
    private float minShootCooldown = 3f;

    [SerializeField]
    private float maxShootCooldown = 5f;

    [SerializeField]
    private int bulletDamage = 20;

    [SerializeField]
    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        InvokeShootWithCooldown();
    }

    private void InvokeShootWithCooldown()
    {
        Invoke(nameof(Shoot), Random.Range(minShootCooldown, maxShootCooldown));
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
