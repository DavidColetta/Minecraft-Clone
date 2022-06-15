using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public bool isGrounded;
    public bool isSprinting;
    public float walkSpeed = 6f;
    public float sprintSpeed = 12f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;
    public float width = 0.15f;
    public float boundsTolerance = 0.1f;
    private Transform cam;
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0f;
    private bool jumpRequest;
    protected override void Awake() {
        base.Awake();
        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update() {
        GetPlayerInputs();

        CalculateVelocity();

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);
    }
    private void GetPlayerInputs(){
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint")) {
            isSprinting = true;
        }
        if (Input.GetButtonUp("Sprint")) {
            isSprinting = false;
        }
        if (Input.GetButtonDown("Jump"))
            jumpRequest = true;
    }
    private void CalculateVelocity(){
        //Affect momentum with gravity.
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;
        if (isSprinting)//Check if sprinting
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * sprintSpeed * Time.deltaTime;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * walkSpeed * Time.deltaTime;
        //Apply Vertical momentum
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back)){
            velocity.z = 0;
        }
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left)){
            velocity.x = 0;
        }
        if (velocity.y < 0)
            velocity.y = CheckDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = CheckUpSpeed(velocity.y);
        /*velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * walkSpeed * Time.deltaTime;
        velocity += Vector3.up * gravity * Time.deltaTime;
        velocity.y = CheckDownSpeed(velocity.y);*/
    }
    private float CheckDownSpeed(float downSpeed){
        if (World.CheckForVoxel(transform.position.x - width,transform.position.y+downSpeed,transform.position.z - width)
            || World.CheckForVoxel(transform.position.x + width,transform.position.y+downSpeed,transform.position.z - width)
            || World.CheckForVoxel(transform.position.x + width,transform.position.y+downSpeed,transform.position.z + width)
            || World.CheckForVoxel(transform.position.x - width,transform.position.y+downSpeed,transform.position.z + width)){
                isGrounded = true;
                return 0f;
        }
        isGrounded = false;
        return downSpeed;
    }
    private float CheckUpSpeed(float upSpeed){
        if (World.CheckForVoxel(transform.position.x - width,transform.position.y+1.9f+upSpeed,transform.position.z - width)
            || World.CheckForVoxel(transform.position.x + width,transform.position.y+1.9f+upSpeed,transform.position.z - width)
            || World.CheckForVoxel(transform.position.x + width,transform.position.y+1.9f+upSpeed,transform.position.z + width)
            || World.CheckForVoxel(transform.position.x - width,transform.position.y+1.9f+upSpeed,transform.position.z + width)){
                return 0f;
        }
        return upSpeed;
    }
    public bool front{
        get{
            if (World.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z+width) ||
                World.CheckForVoxel(transform.position.x, transform.position.y + 1f, transform.position.z+width)){
                    return true;
            }
            return false;
        }
    }
    public bool back{
        get{
            if (World.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z-width) ||
                World.CheckForVoxel(transform.position.x, transform.position.y + 1f, transform.position.z-width)){
                    return true;
            }
            return false;
        }
    }
    public bool left{
        get{
            if (World.CheckForVoxel(transform.position.x-width, transform.position.y, transform.position.z) ||
                World.CheckForVoxel(transform.position.x-width, transform.position.y + 1f, transform.position.z)){
                    return true;
            }
            return false;
        }
    }
    public bool right{
        get{
            if (World.CheckForVoxel(transform.position.x+width, transform.position.y, transform.position.z) ||
                World.CheckForVoxel(transform.position.x+width, transform.position.y + 1f, transform.position.z)){
                    return true;
            }
            return false;
        }
    }
}
