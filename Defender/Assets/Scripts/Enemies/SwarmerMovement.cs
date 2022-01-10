using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SwarmerMovement : MonoBehaviour
{

    [SerializeField]
    private float pulseForce = 3f;

    [SerializeField]
    private float maxMagnitude = 25f;
    
    [SerializeField]
    private float decreaseDeviation = 0.02f;

    [SerializeField]
    private float minCooldown = 0.4f;

    [SerializeField]
    private float maxCooldown = 1f;


    [SerializeField]
    private int touchDamage = 20;

    private float decrease = 0.978f;

    private Rigidbody2D rigidBody;

    private GameObject player;

    private bool addImpulseQueued = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rigidBody);

        player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);

        decrease += Random.Range(-decreaseDeviation, decreaseDeviation);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //pulse the swarmer
        if(rigidBody.velocity.magnitude < 1f && addImpulseQueued == false)
        {
            Invoke(nameof(AddImpulse), Random.Range(minCooldown, maxCooldown));
            addImpulseQueued = true;
        }
        //reduce velocity
        else
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            rigidBody.velocity *= 0.97f;
        }
    }

    void AddImpulse()
    {
        Vector2 targetForce = new Vector2(player.transform.position.x - transform.position.x + Random.insideUnitCircle.x, player.transform.position.y - transform.position.y + Random.insideUnitCircle.y) * 1.5f;
        rigidBody.AddForce(Vector2.ClampMagnitude(targetForce * pulseForce, maxMagnitude), ForceMode2D.Impulse);
        addImpulseQueued = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject?.GetComponent<HealthComponent>()?.TakeDamage(touchDamage);
    }
}
