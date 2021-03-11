using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerSimpleAnimator : MonoBehaviour
{
    // Need to desactivate Root Motion and Gravity in RB when using this script.

    #region Variables

    [Header("Options")]
    public Rigidbody _body;
    public Transform rootKinematicSkeleton;
    public CharacterController _controller;
    public Vector3 moveDirection;
    public Vector3 _inputs = Vector3.zero;
    public bool shooterCameraMode = false;
    public float moveSpeed = 1.0f;
    public float rotationSpeed = 280f;

    //public float JumpHeight = 2f;
    //public float DashDistance = 5f;

    [Header("Ground")]
    public Transform _groundChecker;
    public bool _isGrounded = true;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public Vector3 _velocity;
    //public float Gravity = -9.81f;

    [Header("Animation")]
    public Animator _anim;
    public float inputMagnitude;

    [Header("Experimental")]
    public bool applyMove = true;
    public bool applyRotation = false;
    public Vector3 m_EulerAngleVelocity = new Vector3(0, 100, 0);
    public bool applyTorque = false;
    public float torque;

    # endregion

    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        _groundChecker = transform.GetChild(transform.childCount - 1); // Since we disable gravity in our RB, we use to check ground

        // Set CoM for lower-body into the hips
        _body.centerOfMass = rootKinematicSkeleton.localPosition;
    }

    void Update()
    {
        // Is grounded? If not, apply gravity.
        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        //if (_isGrounded && _velocity.y < 0)
        //    _velocity.y = 0f;

        // User input
        _inputs = Vector3.zero;
        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");

        // Direction of the character with respect to the input (e.g. W = (0,0,1))
        moveDirection = Vector3.forward * _inputs.z + Vector3.right * _inputs.x;
        //Debug.Log("moveDirection 1: " + moveDirection);

        // 1) Rotate with respect to the camera: Calculate camera projection on ground -> Change direction to be with respect to camera.
        Vector3 projectedCameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Quaternion rotationToCamera = Quaternion.LookRotation(projectedCameraForward, Vector3.up);
        moveDirection = rotationToCamera * moveDirection;
        //Debug.Log("moveDirection 2: " + moveDirection);

        // 2) How to rotate the character: In shooter mode, the character rotates such that always points to the forward of the camera.
        if (shooterCameraMode)
        {
            if (_inputs != Vector3.zero)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToCamera, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (_inputs != Vector3.zero)
            {
                Quaternion rotationToMoveDirection = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToMoveDirection, rotationSpeed * Time.deltaTime);
            }
        }

        // Animation Part 
        if (_inputs != Vector3.zero)
        {
            _anim.SetBool("isWalking", true);
        }
        else
        {
            _anim.SetBool("isWalking", false);
        }

        _anim.SetFloat("InputX", _inputs.x, 0.0f, Time.deltaTime);
        _anim.SetFloat("InputZ", _inputs.z, 0.0f, Time.deltaTime);

        _inputs.Normalize();
        inputMagnitude = _inputs.sqrMagnitude;
        _anim.SetFloat("InputMagnitude", inputMagnitude, 0.0f, Time.deltaTime);

        /*
        if (Input.GetButtonDown("Jump") && _isGrounded)
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
        if (Input.GetButtonDown("Dash"))
        {
            Debug.Log("Dash");
            _velocity += Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime)));
        }
        */

        /*
        _velocity.y += Gravity * Time.deltaTime;

        _velocity.x /= 1 + Drag.x * Time.deltaTime;
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        _velocity.z /= 1 + Drag.z * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
        */

        _controller.Move(moveDirection * Time.deltaTime * moveSpeed);

    }

    private void FixedUpdate()
    {
        // We don't need to push the RB - root motion moves the character though animations.
        if (applyMove)
            _body.MovePosition(_body.position + _inputs * moveSpeed * Time.fixedDeltaTime);

        /*
         * Use Rigidbody.MoveRotation to rotate a Rigidbody, complying with the Rigidbody's interpolation setting.
         * If Rigidbody interpolation is enabled on the Rigidbody, calling Rigidbody.MoveRotation will resulting in a smooth transition between the two rotations in any intermediate frames rendered.
         * This should be used if you want to continuously rotate a rigidbody in each FixedUpdate.
         */
        if (applyRotation)
        {
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.deltaTime);
            _body.MoveRotation(_body.rotation * deltaRotation);
        }

        /*
         * Adds a torque to the rigidbody.
         * Force can be applied only to an active rigidbody. If a GameObject is inactive, AddTorque has no effect.
         */
        if (applyTorque)
        {
            _body.AddTorque(torque * transform.up);
        }
    }
}
