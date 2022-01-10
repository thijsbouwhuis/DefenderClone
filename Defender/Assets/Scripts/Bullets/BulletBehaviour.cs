using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class BulletBehaviour : MonoBehaviour
{
    [SerializeField]
    protected float speed = 40f;

    [SerializeField]
    private float lifespan = 2f;

    protected Rigidbody2D rigidBody;

    private CapsuleCollider2D collider;
    
    protected int damage = 0;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rigidBody);

        collider = GetComponent<CapsuleCollider2D>();
        Assert.IsNotNull(collider);

        Invoke(nameof(Destroy), lifespan);
    }

    protected virtual void Destroy()
    {       
        Destroy(gameObject);
    }

    public void Fire(Direction direction, int damage)
    {
        rigidBody.velocity = transform.right * speed * (direction == Direction.Right? 1f : -1f);
        this.damage = damage;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject?.GetComponent<HealthComponent>()?.TakeDamage(damage);
        Destroy();
    }
}
