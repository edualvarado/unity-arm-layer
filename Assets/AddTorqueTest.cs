using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTorqueTest : MonoBehaviour
{
    
    public Transform kinematicHip;
    public Transform physicalHip;
    /*
    public Transform kinematicSpine;
    public Transform physicalSpine;
    public Transform kinematicHead;
    public Transform physicalHead;
    public ConfigurableJoint cJoint1;
    public ConfigurableJoint cJoint2;
    public ConfigurableJoint cJoint3;
    */

    public List<Transform> upperKinematicBody;
    public List<Transform> upperPhysicsBody;

    public List<ConfigurableJoint> upperConfJoints;

    public List<Quaternion> targetRotations;

    public SetSkeletons setSkeletons;

    private ConfigurableJoint springJoint;
    float spring = 400;
    float damper = 10;

    // Start is called before the first frame update
    void Start()
    {

        setSkeletons = this.GetComponent<SetSkeletons>();

        upperPhysicsBody.Add(physicalHip);
        upperKinematicBody.Add(kinematicHip);
        upperConfJoints.Add(physicalHip.gameObject.GetComponent<ConfigurableJoint>());

        // Provisional
        Invoke("ExtractJoints", 1f);

    }

    private void ExtractJoints()
    {
        int idx = 1;
        foreach (Transform tfm in setSkeletons.physicalUpperBones)
        {
            if (tfm.gameObject.GetComponent<ConfigurableJoint>())
            {
                springJoint = tfm.gameObject.GetComponent<ConfigurableJoint>();
                upperPhysicsBody.Add(tfm);
                upperConfJoints.Add(springJoint);
                upperKinematicBody.Add(setSkeletons.kinematicUpperBones[idx]);
            }

            idx++;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ExtractTargetRotations();

        /*
        var targetRotation1 = kinematicHip.transform.localRotation;
        var targetRotation2 = kinematicSpine.transform.localRotation;
        var targetRotation3 = kinematicHead.transform.localRotation;

        cJoint1.targetRotation = targetRotation1;
        cJoint2.targetRotation = targetRotation2;
        cJoint3.targetRotation = targetRotation3;
        */

    }

    private void ExtractTargetRotations()
    {
        int idx = 0;
        foreach (Transform tfm in upperKinematicBody)
        {
            //targetRotations.Add(tfm.localRotation);
            upperConfJoints[idx].targetRotation = tfm.localRotation;
            idx++;
        }
    }
}
