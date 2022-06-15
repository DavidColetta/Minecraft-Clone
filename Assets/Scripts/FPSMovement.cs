using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
[SelectionBase]
public class FPSMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float mouseSensitivity = 500f;
    public Transform cameraTransform;
    private float xRotation = 0f;
    private CharacterController controller;
    public float jumpHeight = 3f;
    public float gravity = -19.62f;
    Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (velocity.y < 0){
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded)
                velocity.y = -6f;
        }

        //Look around
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation,0f,0f);
        transform.Rotate(Vector3.up * mouseX);

        //Move
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        move.Normalize();
        controller.Move(move*moveSpeed*Time.deltaTime);

        if (isGrounded && Input.GetButton("Jump")){
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            isGrounded = false;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
