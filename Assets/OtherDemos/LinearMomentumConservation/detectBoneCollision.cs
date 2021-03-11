using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectBoneCollision : MonoBehaviour
{
    // Objects
    private Rigidbody rbReceiver;
    private SphereCollider colliderReceiver;
    public Rigidbody rbTarget;
    public bool experimentalMode = true;

    private Vector3 localContactOriginal;
    private Vector3 localContactOriginalFixed;
    private Vector3 globalContactOriginal;
    private Vector3 localContactOriginalFixed2;
    private Vector3 localOriginalCOM;
    private Vector3 globalOriginalCOM;
    private Vector3 localOriginalCenterCollider;
    private Vector3 globalOriginalCenterCollider;

    private Vector3 localContactTarget;
    private Vector3 localContactTargetFixed;
    private Vector3 globalContactTarget;
    private Vector3 globalContactTargetFixed;
    private Vector3 globalTargetCOM;

    private Vector3 forceApplied;
    private Vector3 forceAppliedFixed;
    private Vector3 impulseApplied;

    // Start is called before the first frame update
    void Start()
    {
        rbReceiver = GetComponent<Rigidbody>();
        colliderReceiver = GetComponent<SphereCollider>();
    }

    private void Update()
    {


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        globalOriginalCOM = this.transform.TransformPoint(rbReceiver.centerOfMass);
        globalTargetCOM = rbTarget.gameObject.transform.TransformPoint(rbTarget.centerOfMass);
        //Debug.DrawLine(Vector3.zero, globalOriginalCOM, Color.red);
    }

    // We need to transmit the information from the visible body parts to the (from) fully-physical skeleton. 
    private void OnCollisionEnter(Collision other)
    {
        if (experimentalMode)
        {
            if (other.gameObject.tag == "Obstacle")
            {
                // Global Contact Vector of original object
                globalContactOriginal = other.GetContact(0).point;
                Debug.Log("Contact Point: " + globalContactOriginal);
                Debug.DrawRay(Vector3.zero, globalContactOriginal, Color.white);


                // We calcuate the force
                impulseApplied = -(other.impulse);
                forceApplied = -(other.impulse / Time.fixedDeltaTime);
                Debug.Log("Impulse " + other.impulse);
                Debug.Log("Force " + forceApplied);
                Debug.DrawRay(globalContactOriginal, forceApplied, Color.green);


                // Local Contact for original object -- Review

                // We get the local contact point with respect the COM of that RigidBody
                Debug.DrawLine(Vector3.zero, globalOriginalCOM, Color.red);
                localContactOriginal = globalContactOriginal - globalOriginalCOM;
                Debug.Log("globalContactOriginal " + globalContactOriginal);
                Debug.Log("globalOriginalCOM " + globalOriginalCOM);
                Debug.Log("localContactOriginal " + localContactOriginal);
                Debug.DrawRay(globalOriginalCOM, localContactOriginal, Color.blue);

                /*
                localContactOriginal = this.transform.InverseTransformPoint(globalContactOriginal);
                localContactOriginalFixed = this.transform.localRotation * localContactOriginal;
                Debug.Log("Local Contact Point Original: " + localContactOriginal);
                Debug.Log("Local Contact Point Original Fixed: " + localContactOriginalFixed);
                Debug.DrawRay(this.transform.position, localContactOriginal, Color.red);
                Debug.DrawRay(this.transform.position, localContactOriginalFixed, Color.blue);
                */

            }


            if (other.gameObject.tag == "Obstacle")
            {
                Debug.DrawLine(Vector3.zero, globalTargetCOM, Color.red);
                Debug.DrawRay(globalTargetCOM, forceApplied, Color.green);

                //rbTarget.AddRelativeForce(forceApplied, ForceMode.Impulse);
                rbTarget.AddForce(forceApplied, ForceMode.Impulse);

                /*
                Debug.DrawLine(Vector3.zero, globalTargetCOM, Color.red);
                localContactTarget = localContactOriginal;
                Debug.DrawRay(globalTargetCOM, localContactTarget, Color.blue);
                globalContactTarget = globalTargetCOM + localContactTarget;
                Debug.DrawRay(Vector3.zero, globalContactTarget, Color.white);
                */

                /*
                // Local Contact from target object
                localContactTarget = localContactOriginalFixed;
                localContactTargetFixed = rbTarget.transform.rotation * localContactTarget;
                Debug.Log("Local Contact Pointer Target: " + localContactTarget);
                Debug.Log("Local Contact Pointer Target Fixed: " + localContactTargetFixed);
                Debug.DrawRay(rbTarget.transform.position, localContactTarget, Color.red);
                Debug.DrawRay(rbTarget.transform.position, localContactTargetFixed, Color.blue);

                // Convert the hit into global
                globalContactTarget = rbTarget.transform.TransformPoint(localContactTarget);
                //globalContactTargetFixed = rbTarget.transform.TransformPoint(localContactTargetFixed);
                Debug.DrawRay(Vector3.zero, globalContactTarget, Color.white);
                //Debug.DrawRay(Vector3.zero, globalContactTargetFixed, Color.white);

                forceAppliedFixed = rbTarget.transform.rotation * forceApplied;
                Debug.DrawRay(globalContactTarget, forceAppliedFixed, Color.green);

                rbTarget.AddForceAtPosition(forceAppliedFixed, globalContactTarget);
                */

            }
        }
        
        else
        {
            rbTarget.velocity = rbReceiver.velocity;

        }

    }

    void OnDrawGizmosSelected()
    {
        globalOriginalCOM = this.transform.TransformPoint(rbReceiver.centerOfMass);
        globalTargetCOM = rbTarget.gameObject.transform.TransformPoint(rbTarget.centerOfMass);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(globalOriginalCOM, 0.01f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(globalTargetCOM, 0.01f);
    }

}
