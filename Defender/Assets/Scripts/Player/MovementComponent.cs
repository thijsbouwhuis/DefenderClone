using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public enum Direction { Left, Right };

public class MovementComponent : MonoBehaviour
{
    [SerializeField]
    private float accelerationX = 300.0f;

    [SerializeField]
    private float speedY = 5.0f;

    [SerializeField]
    private float deceleration = 2.0f;

    [SerializeField]
    private float maxAcceleration = 800.0f;

    [SerializeField]
    private Joystick joyStick;

    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rigidBody;

    private Vector2 cameraBounds;


    Direction curDirection = Direction.Right;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);

        rigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rigidBody);

        Assert.IsNotNull(joyStick);
    }
    // Start is called before the first frame update
    void Start()
    {
        cameraBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        if (xInput == 0)
        {
            xInput = joyStick.Horizontal;
        }
        if(yInput == 0)
        {
            yInput = joyStick.Vertical;
        }
        if (xInput == 0 && yInput == 0)
            return;

        //Set new Y transform
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y + yInput * Time.deltaTime * speedY, cameraBounds.y * -1f + 0.2f, cameraBounds.y -0.2f ), transform.position.z);

        //Direction flipping
        if (xInput > 0 && curDirection == Direction.Left)
        {
            curDirection = Direction.Right;
            spriteRenderer.transform.localScale = new Vector3(spriteRenderer.transform.localScale.x * -1, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
        }
        if (xInput < 0 && curDirection == Direction.Right)
        {
            curDirection = Direction.Left;
            spriteRenderer.transform.localScale = new Vector3(spriteRenderer.transform.localScale.x * -1, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
        }
    }

    //Use fixedupdate for physics
    private void FixedUpdate()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        if (xInput == 0)
        {
            xInput = joyStick.Horizontal;
        }


        Vector3 velocity = transform.InverseTransformDirection(rigidBody.velocity);

        //Counter movement
        if (Mathf.Abs(velocity.x) > 0.01f && Mathf.Abs(xInput) < 0.05f || (velocity.x < -0.01f && xInput > 0) || (velocity.x > 0.01f && xInput < 0))
        {
            rigidBody.AddForce(transform.right * deceleration * -velocity.x);
        }

        //Normal movement
        if (Mathf.Abs(velocity.x) < maxAcceleration)
            rigidBody.AddForce(transform.right * xInput * Time.fixedDeltaTime * accelerationX);
    }

    public Direction GetDirection()
    {
        return curDirection;
    }

    public float GetXVelocity()
    {
        return Mathf.Abs(rigidBody.velocity.x);
    }

    public float GetMaxAcceleration()
    {
        return maxAcceleration;
    }

    public void SetPositionOnChunkReset(Vector3 newPosition)
    {
        GetComponent<ShootComponent>()?.SetBulletsPositionOnChunkReset(newPosition);
        transform.position = newPosition;

    }
}
