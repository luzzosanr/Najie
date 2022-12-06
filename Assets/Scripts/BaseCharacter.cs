using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    [Header("Objects")]
    public CharacterController characterController;
    public Camera cam;


    [Header("Character speed")]
    public float MovementSpeed = 1f;
    public float Gravity = 9.8f;
    public float JumpSpeed = 1f;

    [Header("Camera speed")]
    public float horizontalSpeed = 1f;
    public float verticalSpeed = 1f;


    private float velocity = 0;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private bool isSprinting = false;

    // Start is called before the first frame update
    protected void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        cam = Camera.main;
        Cursor.visible = false;
    }

    // Update is called once per frame
    protected void Update()
    {        
        //Run with a key
        if (Input.GetKeyDown(KeyCode.Q) && !isSprinting)
        {
            isSprinting = true;
            MovementSpeed *= 2;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && isSprinting)
        {
            isSprinting = false;
            MovementSpeed /= 2;
        }

        // player movement - forward, backward, left, right
        float angle = Mathf.Deg2Rad * cam.transform.eulerAngles.y;
        Vector3 vectorAngleVertical = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
        Vector3 vectorAngleHorizontal = new Vector3(Mathf.Sin(angle + Mathf.PI / 2), 0f, Mathf.Cos(angle + Mathf.PI / 2));

        characterController.Move((vectorAngleVertical * Input.GetAxis("Vertical") + vectorAngleHorizontal * Input.GetAxis("Horizontal")) * MovementSpeed * Time.deltaTime);
        
        // Gravity
        if (characterController.isGrounded && Input.GetButton("Jump"))
        {
            velocity = JumpSpeed * 0.25f;
        }
        else if(characterController.isGrounded)
        {
            velocity = 0;
        }
        else if (velocity > -Gravity)
        {
            velocity -= Gravity * Time.deltaTime * 0.1f;
        }
        characterController.Move(new Vector3(0, velocity, 0));

        
        float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;
 
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
 
        cam.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
 
    }
}
