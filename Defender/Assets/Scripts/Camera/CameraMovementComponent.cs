using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementComponent : MonoBehaviour
{
    [SerializeField]
    private float minOffset = 100f;
    [SerializeField]
    private float maxOffset = 400f;

    [SerializeField]
    private MovementComponent playerMovement;

    float lerpTargetOffset = 0f;
    float curLerpTime = 0f;
    [SerializeField]
    private float maxLerpTime = 0.5f;

    private float startLerpPos = 0f;

    private float AdditionalOffset = 0f;


    Direction CameraDirection = Direction.Right;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //Check if current direction = direction of last frame
        //if yes, go through movement and do min/max offset shit
        if (playerMovement.GetDirection() == CameraDirection)
        {
            AdditionalOffset = 3 * (playerMovement.GetXVelocity() / playerMovement.GetMaxAcceleration());

            //if currently lerping, continue
            if (curLerpTime != 0f)
            {
                CameraLerp();
            }
            else
            {
                MoveCamera();
            }
        }
        else
            StartCameraLerp();

    }

    private void StartCameraLerp()
    {
       
        curLerpTime = 0f;
        startLerpPos = transform.position.x;
        CameraLerp();
        CameraDirection = playerMovement.GetDirection();
    }
    private void CameraLerp()
    {
        lerpTargetOffset = (maxOffset - AdditionalOffset) * (CameraDirection == Direction.Left ? -1f : 1f);
        float newXPos = Mathf.Lerp(startLerpPos, playerMovement.transform.position.x + lerpTargetOffset, curLerpTime / maxLerpTime);
        curLerpTime += Time.deltaTime;
        transform.position = new Vector3(newXPos, transform.position.y, transform.position.z);
        //Finish and reset lerp
        if (curLerpTime >= maxLerpTime)
        {
            curLerpTime = 0f;
            lerpTargetOffset = 0f;
            startLerpPos = 0f;
        }
    }

    private void MoveCamera()
    {
        transform.position = new Vector3(playerMovement.transform.position.x + (maxOffset - AdditionalOffset) * (CameraDirection == Direction.Left ? -1f : 1f), transform.position.y, transform.position.z);
    }
}
