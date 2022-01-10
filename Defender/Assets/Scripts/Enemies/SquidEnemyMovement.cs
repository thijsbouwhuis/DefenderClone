using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidEnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;

    [SerializeField]
    private float maxY = -2f;

    [SerializeField]
    private float minY = -4f;

    [SerializeField]
    private float xDirectionRollCooldown = 3f;

    [SerializeField]
    private int touchDamage = 20;

    private int xDirection = 1;
    
    [SerializeField]
    private float yDirectionRollCooldown = 6f;
    
    private float yDirection = 0f;

    private Vector3 MoveToTarget = new Vector3(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(SetRandomXDirection), xDirectionRollCooldown, xDirectionRollCooldown);
        InvokeRepeating(nameof(SetRandomYDirection), yDirectionRollCooldown, yDirectionRollCooldown);
    }

    //33% chance to change direction
    void SetRandomXDirection()
    {
        //from -1 to 2 cause 2 is exclusive so it goes to -1, 1 inclusive
        int newDirection = Random.Range(-1, 2);
        if(newDirection != 0)
        {
            xDirection = newDirection;
        }
    }

    void SetRandomYDirection()
    {
        //from -1 to 2 cause 2 is exclusive so it goes to -1, 1 inclusive
        float newDirection = Random.Range(-1, 2);
        if (newDirection != 0)
        {
            yDirection = newDirection;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(MoveToTarget.y != 0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, MoveToTarget, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, MoveToTarget) < 0.001f)
            {
                MoveToTarget = new Vector3(0f, 0f, 0f);
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime * xDirection, Mathf.Clamp(transform.position.y + Time.deltaTime * yDirection, minY, maxY), transform.position.z);      
        }
    }

    public void SetMoveToTarget(Vector3 newTarget)
    {
        MoveToTarget = newTarget;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject?.GetComponent<HealthComponent>()?.TakeDamage(touchDamage);
    }
}
