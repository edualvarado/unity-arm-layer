using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectBoneCollision : MonoBehaviour
{
    // Objects
    public Rigidbody rbReceiver;
    public Rigidbody rbTarget;
    public Collider colliderReceiver;


    public Vector3 localContactOriginal;
    public Vector3 localContactOriginalFixed;
    public Vector3 globalContactOriginal;

    public Vector3 localContactTarget;
    public Vector3 localContactTargetFixed;
    public Vector3 globalContactTarget;
    public Vector3 globalContactTargetFixed;

    public Vector3 forceApplied;
    public Vector3 forceAppliedFixed;
    public Vector3 impulseApplied;

    // Start is called before the first frame update
    void Start()
    {
        rbReceiver = GetComponent<Rigidbody>();
        colliderReceiver = GetComponent<Collider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    // We need to transmit the information from the visible body parts to the (from) fully-physical skeleton. 
    private void OnCollisionEnter(Collision other)
    {
        /*
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
            localContactOriginal = this.transform.InverseTransformPoint(globalContactOriginal);
            localContactOriginalFixed = this.transform.localRotation * localContactOriginal;
            Debug.Log("Local Contact Point Original: " + localContactOriginal);
            Debug.Log("Local Contact Point Original Fixed: " + localContactOriginalFixed);
            Debug.DrawRay(this.transform.position, localContactOriginal, Color.red);
            Debug.DrawRay(this.transform.position, localContactOriginalFixed, Color.blue);

        }

        if (other.gameObject.tag == "Obstacle")
        {
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

        }

        */

        rbTarget.velocity = rbReceiver.velocity;




    }

}
