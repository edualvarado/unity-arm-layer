using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerSimpleAnimator : MonoBehaviour
{

    #region Variables

    [Header("Components")]
    public CharacterController _controller;
    public Rigidbody _body; // NEW
    public Transform rootKinematicSkeleton;

    [Header("Motion")]
    public float Speed = 5f;
    //public float JumpHeight = 2f;
    //public float Gravity = -9.81f;
    //public float GroundDistance = 0.2f;
    //public float DashDistance = 5f;
    //public LayerMask Ground;
    //public Vector3 Drag;
    public Vector3 _inputs = Vector3.zero; // NEW
    public Vector3 _velocity;

    [Header("Grounder")]
    public bool _isGrounded = true;
    public Transform _groundChecker;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;

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
        _controller = GetComponent<CharacterController>();
        _body = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();

        _groundChecker = transform.GetChild(transform.childCount - 1); // Since we disable gravity in our RB, we use to check ground

        // Set CoM for lower-body into the hips
        _body.centerOfMass = rootKinematicSkeleton.localPosition;
    }

    void Update()
    {


        _isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(_inputs.x, 0, _inputs.z);
        _controller.Move(move * Time.deltaTime * Speed);
        if (move != Vector3.zero)
            transform.forward = move;

        // Animation
        _anim.SetFloat("InputX", _inputs.x, 0.0f, Time.deltaTime);
        _anim.SetFloat("InputZ", _inputs.z, 0.0f, Time.deltaTime);
        inputMagnitude = new Vector2(_inputs.x, _inputs.z).sqrMagnitude;
        inputMagnitude = Mathf.Clamp(inputMagnitude, 0, 1);
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
    }

    private void FixedUpdate()
    {

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
