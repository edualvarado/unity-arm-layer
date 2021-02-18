using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyController : MonoBehaviour
{
    #region Variables
    [Header("Components")]
    public Rigidbody _body;
    public Animator _anim;
    private Transform _groundChecker;

    [Header("Movement Information")]
    public Vector3 _inputs = Vector3.zero;
    public float inputMagnitude;
    public float inputMagnitudeNormalized;

    [Header("Thresholds of movement")]
    [Range(0f,2f)]public float speedRigidBody = 1.4f;
    public float groundDistance = 0.2f;
    public LayerMask ground;
    public bool _isGrounded = true;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _groundChecker = this.transform.GetChild(transform.childCount - 1); // Sphere must be the last child of the parent.
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);

        _inputs = Vector3.zero;
        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");
        if (_inputs != Vector3.zero)
            transform.forward = _inputs;

        // Set Floats into the animator equal to the inputs.
        _anim.SetFloat("InputZ", _inputs.z, 0.0f, Time.deltaTime);
        _anim.SetFloat("InputX", _inputs.x, 0.0f, Time.deltaTime);

        // Calculate InputMagnitude (Blend (Speed)) - Better for controller axis - How much you are pressing the key or joystick.
        // Calculate the squared magnitude instead of the magnitude is much faster (it doesn´t need to do the square root).
        inputMagnitude = new Vector2(_inputs.x, _inputs.z).sqrMagnitude;
        inputMagnitudeNormalized = Mathf.Clamp(inputMagnitude, 0, 1);

        // Physically move player w/ animations
        _anim.SetFloat("InputMagnitude", inputMagnitude, 0.0f, Time.deltaTime);

    }

    void FixedUpdate()
    {
        // Move kinematically RB
        _body.MovePosition(_body.position + _inputs * (inputMagnitudeNormalized * speedRigidBody) * Time.fixedDeltaTime);
    }
}
