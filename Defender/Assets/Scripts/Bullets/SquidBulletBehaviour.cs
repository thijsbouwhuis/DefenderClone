using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBulletBehaviour : BulletBehaviour
{
    [SerializeField]
    float minDeviation = .8f;

    [SerializeField]
    float maxDeviation = 1.2f;
    public void Fire(int damage)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player)
        {
            float shootxDirection = player.transform.position.x > transform.position.x ? 1f : -1f;
            float shootyDirection = player.transform.position.y > transform.position.y ? 1f : -1f;
            rigidBody.velocity = new Vector2(speed * shootxDirection, speed * Random.Range(minDeviation, maxDeviation) * shootyDirection);
            this.damage = damage;
        }
    }
    protected override void Destroy()
    {
        GetComponent<EntityChunkComponent>()?.GetOwner().NotifyEntityDestroyed(gameObject);
        base.Destroy();
    }
}
