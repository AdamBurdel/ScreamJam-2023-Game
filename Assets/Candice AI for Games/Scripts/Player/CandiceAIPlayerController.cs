using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Candice AI
using CandiceAIforGames.AI;

[RequireComponent(typeof(CharacterController))]

public class CandiceAIPlayerController : MonoBehaviour
{

    //player movement
    public float speed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    //cameras (supports attached, follow, killcam)
    public Transform playerCameraParent;
    //locals
    private Transform playerCamera;
    private Transform originalTransform;
    //camera movement buffers
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    public bool canRotateCamera = true;
    public int zoomSpeed = 40;
    //camera target, lookat
    public Vector3 targetOffset;
    private Vector3 targetOffsetInitial;
    //zoom & rotation
    public float maxZoomDistance = 20;
    public float minZoomDistance = .6f;
    [HideInInspector]
    public static float currentDistance;
    private float desiredDistance;
    private float rotateVelocity = 0.0f;
    public float rotateVelocityMultiplier = 0.005f;
    private float rotateSmooth = 0.0f;
    public float zoomDampening = 5.0f;
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Transform thisTransform;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        //get transform and controller
        thisTransform = gameObject.transform;
        characterController = GetComponent<CharacterController>();

        //reset y rotation on start
        if (rotation.y == 0f) {
            rotation.y = transform.eulerAngles.y;
        }        

        //get attached camera if any
        playerCamera = playerCameraParent.transform.Find("PlayerCamera");
        if (playerCamera == null) {
            //if none attached, set whatever camera is set as Main
            playerCamera = Camera.main.transform;
        }
        //if player camera is not null (if attached)
        if (playerCameraParent != null) {
            originalTransform = playerCameraParent;
        }

        //store my target offset initially, so whatver the transform is for my target plus offset Vector3s
        targetOffsetInitial = targetOffset;
    }
    
    void Update()
    {        
        //Just run player on Update
        Player();        
    }
    
    void LateUpdate() {

        //Late update for mecha only currently
        //MechaOnly();       
        //camera rotation on lateUpdate
        if (canRotateCamera) {
            if (Input.GetButton("Rotate Camera"))
            {
                if (rotateVelocity == 0.0f) {
                    targetOffset = targetOffsetInitial;
                    playerCamera.rotation = thisTransform.rotation;                    
                }
                targetOffset = new Vector3(targetOffsetInitial.x, targetOffsetInitial.y, maxZoomDistance - currentDistance);
                //rotateVelocity += rotateVelocityMultiplier;                
            }
            else {
                rotateVelocity = 0.0f;
                targetOffset = targetOffsetInitial;
                playerCamera.rotation = thisTransform.rotation;
            }
            if (rotateVelocity != 0.0f) {
                playerCamera.rotation = Quaternion.Euler(35f, playerCamera.rotation.y, playerCamera.rotation.z);
                Rotate(rotateVelocity);
            }
            Zoom();
        }


    }

    void MechaOnly() {

        if (playerCamera == null)
        {
            //if none attached, set whatever camera is set as Main
            playerCamera = Camera.main.transform;
        }

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            //playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            //transform.eulerAngles = new Vector2(0, rotation.y);
        }

        //Camera switch
        if (playerCamera != null)
        {
            //default
            if (playerCamera.transform.parent.gameObject.activeSelf)
            {
                CandicePlayerOverrides.bufferMousePositionz = 30f + currentDistance;
            }

        }
        
    }

    void Player() {

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }

        //Camera switch
        if (playerCamera != null)
        {
            //default
            if (playerCameraParent.gameObject.activeSelf)
            {
                CandicePlayerOverrides.bufferMousePositionz = 10f + currentDistance;
            }

        }
        
    }

    void Rotate(float velocity) {            


        //smoothing
        rotateSmooth += (0.02f + rotateSmooth) * 0.005f;            
        rotateSmooth = Mathf.Clamp(rotateSmooth, 0, 1);

        //look limits
        xDeg += lookSpeed * velocity;
        yDeg = ClampAngle(yDeg, -lookXLimit, lookXLimit);

        //rotation
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = playerCamera.rotation;

        //assign x rotation
        playerCamera.rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f * zoomDampening * 2);

    }

    void Zoom() {
        //assign zoom
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * 0.02f * zoomSpeed * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minZoomDistance, maxZoomDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 0.02f * zoomDampening);
        playerCamera.position = transform.position - (transform.rotation * Vector3.forward * currentDistance + targetOffset); ;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}
