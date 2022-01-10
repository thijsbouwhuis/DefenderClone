using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShootComponent : MonoBehaviour
{

    [SerializeField]
    private float fireCooldown = .5f;

    [SerializeField]
    private int bulletDamage = 20;


    private float currentFireCooldown = 0f;

    private MovementComponent movementComponent;

    [SerializeField]
    public GameObject bulletPrefab;

    private List<GameObject> bullets;

    [SerializeField]
    private float removeDestroyedBulletsTimer = 5f;


    void Awake()
    {
        movementComponent = GetComponent<MovementComponent>();
        Assert.IsNotNull(movementComponent);

        bullets = new List<GameObject>();


        InvokeRepeating(nameof(RemoveDestroyedBullets), removeDestroyedBulletsTimer, removeDestroyedBulletsTimer);
    }


    // Update is called once per frame
    void Update()
    {
        //Shoot if pressing spacebar and off cooldown
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space) || Input.touchCount > 0) && currentFireCooldown <= 0f)
        {
            currentFireCooldown = fireCooldown;
            Shoot();
        }

        //Lower remaining cooldown
        if (currentFireCooldown > 0f)
        {
            currentFireCooldown -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
        //Can do object pooling here, but a short lifespan with firing cooldown should be enough for now
       GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
       bullet.GetComponent<BulletBehaviour>().Fire(movementComponent.GetDirection(), bulletDamage);
       bullets.Add(bullet);
    }

    private void RemoveDestroyedBullets()
    {
        int count = bullets.Count;
        for(int i = 0; i < count; i++)
        {
            if(bullets[i] == null)
            {
                bullets.RemoveAt(i);
                i--;
                count--;
            }
        }
    }

    public void SetBulletsPositionOnChunkReset(Vector3 newPlayerPosition)
    {
        RemoveDestroyedBullets();
        foreach (GameObject bullet in bullets)
        {
           Vector3 offset = transform.position - bullet.transform.position;
            bullet.transform.position = newPlayerPosition - offset;
        }
    }
}
