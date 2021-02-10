using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyControllerSimple : MonoBehaviour
{
    #region Variables

    public float Speed = 5f;
    //public float JumpHeight = 2f;
    //public float GroundDistance = 0.2f;
    //public float DashDistance = 5f;
    //public LayerMask Ground;

    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    //private bool _isGrounded = true;
    //private Transform _groundChecker;

    public bool applyMove = true;
    
    public bool applyRotation = false;
    public Vector3 m_EulerAngleVelocity = new Vector3(0, 100, 0);

    public bool applyTorqueX = false;
    public bool applyTorqueY = false;
    public bool applyTorqueZ = false;

    public float torque;


    #endregion

    void Start()
    {
        _body = GetComponent<Rigidbody>();
        //_groundChecker = transform.GetChild(0);
    }

    void Update()
    {
        //_isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        _inputs = Vector3.zero;
        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");
        if (_inputs != Vector3.zero)
            transform.forward = _inputs;

        /*
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        if (Input.GetButtonDown("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime)));
            _body.AddForce(dashVelocity, ForceMode.VelocityChange);
        }
        */
    }

    void FixedUpdate()
    {
        if (applyMove)
            _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);


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
        if (applyTorqueY)
        {
            _body.AddTorque(torque * transform.up);
        }

        if (applyTorqueX)
        {
            _body.AddTorque(torque * transform.right);
        }

        if (applyTorqueZ)
        {
            _body.AddTorque(torque * transform.forward);
        }




    }
}
