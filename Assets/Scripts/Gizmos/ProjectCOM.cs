using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectCOM : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBodyKinematics;
    public Vector3 globalCOMKinematics; 
    public Vector3 globalCOMPhysics;
    public Vector3 globalCOMInterpolated;


    private SetSkeletons setSkeletons;
    public List<Transform> physicalUpperBones, physicalLowerBones;
    public List<Transform> interpolatedUpperBones, interpolatedLowerBones;


    private void Start()
    {
        setSkeletons = FindObjectOfType<SetSkeletons>();

        // Get upper physical bones
        physicalUpperBones = setSkeletons.physicalUpperBones;
        physicalLowerBones = setSkeletons.physicalLowerBones;

        // Get upper interpolated bones
        interpolatedUpperBones = setSkeletons.interpolatedUpperBones;
        interpolatedLowerBones = setSkeletons.interpolatedLowerBones;
    }

    private void Update()
    {
        // Simple COM comming from the single collider in the kinematic model
        globalCOMKinematics = rigidBodyKinematics.worldCenterOfMass;

        // Calculate COM from each single RigidBody in the physical model
        CalculateGlobalCOMPhysics();

        // Calculate COM from each single RigidBody in the interpolated model
        CalculateGlobalCOMInterpolated();

    }

    /// <summary>
    /// Calculates COM from each single RigidBody.
    /// </summary>
    private void CalculateGlobalCOMPhysics()
    {
        float c = 0f;
        globalCOMPhysics = Vector3.zero;

        foreach (Transform tfm in physicalUpperBones)
        {
            if(tfm.gameObject.GetComponent<Rigidbody>())
            {
                Rigidbody rb = tfm.gameObject.GetComponent<Rigidbody>();
                globalCOMPhysics += rb.worldCenterOfMass * rb.mass;
                c += rb.mass;
            }
        }

        globalCOMPhysics /= c;
    }

    private void CalculateGlobalCOMInterpolated()
    {
        float c = 0f;
        globalCOMInterpolated = Vector3.zero;

        foreach (Transform tfm in interpolatedUpperBones)
        {
            if (tfm.gameObject.GetComponent<Rigidbody>())
            {
                Rigidbody rb = tfm.gameObject.GetComponent<Rigidbody>();
                globalCOMInterpolated += rb.worldCenterOfMass * rb.mass;
                c += rb.mass;
            }
        }

        globalCOMInterpolated /= c;
    }

    /// <summary>
    /// Draw COM projections on floor
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(globalCOMKinematics.x, 0f, globalCOMKinematics.z), 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(globalCOMPhysics.x, 0f, globalCOMPhysics.z), 0.05f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(new Vector3(globalCOMInterpolated.x, 0f, globalCOMInterpolated.z), 0.05f);

        UnityEditor.Handles.DrawWireDisc(new Vector3(globalCOMKinematics.x, 0f, globalCOMKinematics.z), Vector3.up, 1f);
    }
}
