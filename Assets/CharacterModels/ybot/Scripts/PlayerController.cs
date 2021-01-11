using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("Movement Information")]
    public float InputX; // Range between -1 and 1 for horizontal axis (sides).
    public float InputZ; // Range between -1 and 1 for vertical axis (forward).
    public Vector3 desiredMoveDirection;

    [Header("Thresholds of movement")]
    public float allowPlayerRotation; // When we want the player to start rotating.
    public float Speed;

    [Header("Rotation Information")]
    public bool blockRotationPlayer; // For static animation forward.
    public float desiredRotationSpeed;

    public Animator anim;
    public CharacterController controller;
    public Camera cam;

    private float gravity;
    private Vector3 characterVelocity; // Measures the velocity of the character at each time.

    // To check if it is grounded.
    [Header("External Method to check isGrounded")]
    [Range(0, 2f)] public float groundDistance = 0.2f;
    public LayerMask ground; // In Inspector, Unity capitalizes the label to "Ground".
    [SerializeField] private bool isGrounded;
    private Transform groundChecker;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // For groundCheck.
        groundChecker = this.transform.GetChild(transform.childCount - 1); // Sphere must be the last child of the parent.
        gravity = Physics.gravity.y; // Uses gravity from physics system.

        // Getting components from Inspector.
        anim = GetComponent<Animator>();
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // For Character controlling.
        InputMagnitude();

        // Ground Check: isGrounded is true if the "imaginary" sphere on the empty gameObject hits some collider. If so, gravity stops increasing.
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);
        if (isGrounded && characterVelocity.y < 0)
        {
            characterVelocity.y = 0f;
        }
        //Debug.Log("Velocity (Y) inc. Gravity: " + characterVelocity.y);

        // If it not grounded , apply gravity and move in Y.
        characterVelocity.y += gravity * Time.deltaTime;
        controller.Move(characterVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Method to calculate input vectors.
    /// </summary>
    void InputMagnitude()
    {
        // Calculate Input Vectors
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        // Set Floats into the animator equal to the inputs.
        anim.SetFloat("InputZ", InputZ, 0.0f, Time.deltaTime);
        anim.SetFloat("InputX", InputX, 0.0f, Time.deltaTime);

        // Calculate InputMagnitude (Blend (Speed)) - Better for controller axis - How much you are pressing the key or joystick.
        // Calculate the squared magnitude instead of the magnitude is much faster (it doesn´t need to do the square root).
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        // Physically move player w/ animations
        if (Speed > allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", Speed, 0.0f, Time.deltaTime);
            PlayerMoveAndRotation();
        }
        else if (Speed < allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", Speed, 0.0f, Time.deltaTime);
        }
    }

    /// <summary>
    /// Method in charge of moving the player.
    /// </summary>
    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        // Set normalized unit vectors for the camera - var could replace Vector3
        // Vector3.right is a vector facing the world right. It will always be (1, 0, 0)
        // transform.right is a vector facing the local-space right, meaning it is a vector that faces to the right of your object. 
        // This vector will be different depending on which way your object with the transform is facing.
        var camera = Camera.main;
        Vector3 forward = cam.transform.forward; // Z direction with respect to the global axis. 
        Vector3 right = cam.transform.right; // X direction with respect to the global axis.

        // Convert to 2D planar coordinates and normalize.
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // forward and right are normalized vectors.
        desiredMoveDirection = forward * InputZ + right * InputX;

        // Rotates the character between the current angle (transform.rotation) to the forward direction of the vector in which you are moving according to InputX and InputZ.
        // It can be modified to certain time or speed.
        if (blockRotationPlayer == false)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
        }
    }
}
