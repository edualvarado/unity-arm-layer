using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Animations.Rigging;
using System;

public class SetSkeletons : MonoBehaviour
{
    #region Variables
    [Header("Kinematic Skeleton")]
    public Transform rootKinematicSkeleton;
    public GameObject kinematicSpine;
    //public GameObject kinematicSpine1;
    public List<Transform> kinematicLowerBones = new List<Transform>();
    public List<Transform> kinematicUpperBones = new List<Transform>();

    [Header("Physical Skeleton")]
    public Transform rootPhysicalSkeleton;
    public GameObject physicalSpine;
    //public GameObject physicalSpine1;
    public List<Transform> physicalLowerBones = new List<Transform>();
    public List<Transform> physicalUpperBones = new List<Transform>();

    
    [Header("Variable Skeleton")]
    [SerializeField] private Transform rootVariableSkeleton;
    [SerializeField] private GameObject variableSpine;
    public List<Transform> variableLowerBones = new List<Transform>();
    public List<Transform> variableUpperBones = new List<Transform>();

    /* TEST */
    [Header("Variable Physical Skeleton")]
    public Transform rootVariablePhysicalSkeleton;
    public GameObject variablePhysicalSpine;
    //public GameObject physicalSpine1;
    public List<Transform> variablePhysicalLowerBones = new List<Transform>();
    public List<Transform> variablePhysicalUpperBones = new List<Transform>();
    

    [Header("Connector")]
    public GameObject hipConnector;

    //public Rigidbody _rbHips;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        InitSkeletons();

        //_rbHips = rootPhysicalSkeleton.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Always match (for now) lower-body
        SyncBones(kinematicLowerBones, physicalLowerBones);

        // Only if third skeleton is available
        SyncBones(kinematicLowerBones, variableLowerBones);

        // TEST
        SyncBones(kinematicLowerBones, variablePhysicalLowerBones);
        //SyncBones(variableUpperBones, variablePhysicalUpperBones);

        // Should we fix by hard-code the root and first spine bone for both skeletons?
        rootPhysicalSkeleton.position = rootKinematicSkeleton.position;

        // Only if third skeleton is available
        rootVariableSkeleton.position = rootKinematicSkeleton.position;

        // TEST
        // Only if third skeleton is available
        rootVariablePhysicalSkeleton.position = rootKinematicSkeleton.position;

        // Don't need to attach first spine
        //physicalSpine.transform.position = kinematicSpine.transform.position;
        //variableSpine.transform.position = kinematicSpine.transform.position;
        //physicalSpine1.transform.position = kinematicSpine1.transform.position;

    }

    /// <summary>
    /// Save both, kinematic and physical skeletons, in their respective lists.
    /// </summary>
    private void InitSkeletons()
    {
        // Create list of skeletons
        CreateKinematicSkeletons();
        CreatePhysicalSkeletons();
        CreateVariableSkeletons();

        // TEST
        CreateVariablePhysicalSkeletons();

        // Posible error -> Putting colliders manually first to see if it works
        // Let initialize the colliders of the upper-body and then disable them, since we use the variable upper-body for collisions.
        //DisableUpperColliders();
    }

    /* Not used
    private void DisableUpperColliders()
    {
        int idx = 0;
        foreach (Transform tfm in physicalUpperBones)
        {

            if(tfm.gameObject.GetComponent<BoxCollider>())
            {
                BoxCollider copy = tfm.gameObject.GetComponent<BoxCollider>();
                Vector3 size = copy.size;
                Vector3 center = copy.center;

                /// ->

                variableUpperBones[idx].gameObject.AddComponent<BoxCollider>();
                variableUpperBones[idx].gameObject.GetComponent<BoxCollider>().size = size;
                variableUpperBones[idx].gameObject.GetComponent<BoxCollider>().center = center;

                ///

                //tfm.gameObject.GetComponent<BoxCollider>().enabled = !tfm.gameObject.GetComponent<BoxCollider>().enabled;
            }
            else if(tfm.gameObject.GetComponent<CapsuleCollider>())
            {
                CapsuleCollider copy = tfm.gameObject.GetComponent<CapsuleCollider>();
                Vector3 center = copy.center;
                float radius = copy.radius;
                float height = copy.height;
                int direction = copy.direction;

                variableUpperBones[idx].gameObject.AddComponent<CapsuleCollider>();
                variableUpperBones[idx].gameObject.GetComponent<CapsuleCollider>().center = center;
                variableUpperBones[idx].gameObject.GetComponent<CapsuleCollider>().radius = radius;
                variableUpperBones[idx].gameObject.GetComponent<CapsuleCollider>().height = height;
                variableUpperBones[idx].gameObject.GetComponent<CapsuleCollider>().direction = direction;

                //tfm.gameObject.GetComponent<CapsuleCollider>().enabled = !tfm.gameObject.GetComponent<CapsuleCollider>().enabled;

            }
            else if(tfm.gameObject.GetComponent<SphereCollider>())
            {
                SphereCollider copy = tfm.gameObject.GetComponent<SphereCollider>();
                Vector3 center = copy.center;
                float radius = copy.radius;

                variableUpperBones[idx].gameObject.AddComponent<SphereCollider>();
                variableUpperBones[idx].gameObject.GetComponent<SphereCollider>().center = center;
                variableUpperBones[idx].gameObject.GetComponent<SphereCollider>().radius = radius;

                //tfm.gameObject.GetComponent<SphereCollider>().enabled = !tfm.gameObject.GetComponent<SphereCollider>().enabled;
            }

            idx++;
        }
    }
    */

    /// <summary>
    /// Divide the kinematic skeleton in upper- and lower-body parts.
    /// </summary>
    private void CreateKinematicSkeletons()
    {
        FindKinematicLowerSkeleton(rootKinematicSkeleton);
        for (int i = 0; i < kinematicLowerBones.Count; i++)
        {
            FindKinematicLowerSkeleton(kinematicLowerBones[i]);
        }

        FindKinematicUpperSkeleton(kinematicUpperBones[0]);
        for (int i = 1; i < kinematicUpperBones.Count; i++)
        {
            FindKinematicUpperSkeleton(kinematicUpperBones[i]);
        }
    }

    /// <summary>
    /// Divide the physical skeleton in upper- and lower-body parts.
    /// </summary>
    private void CreatePhysicalSkeletons()
    {
        FindPhysicalLowerSkeleton(rootPhysicalSkeleton);
        for (int i = 0; i < physicalLowerBones.Count; i++)
        {
            FindPhysicalLowerSkeleton(physicalLowerBones[i]);
        }

        FindPhysicalUpperSkeleton(physicalUpperBones[0]);
        for (int i = 1; i < physicalUpperBones.Count; i++)
        {
            FindPhysicalUpperSkeleton(physicalUpperBones[i]);
        }
    }

    
    /// <summary>
    /// Divide the variable visible skeleton in upper- and lower-body parts.
    /// </summary>
    private void CreateVariableSkeletons()
    {
        FindPhysicalLowerSkeletonConstant(rootVariableSkeleton);
        for (int i = 0; i < physicalLowerBones.Count; i++)
        {
            FindPhysicalLowerSkeletonConstant(variableLowerBones[i]);
        }

        FindPhysicalUpperSkeletonConstant(variableUpperBones[0]);
        for (int i = 1; i < variableUpperBones.Count; i++)
        {
            FindPhysicalUpperSkeletonConstant(variableUpperBones[i]);
        }
    }

    // TEST
    private void CreateVariablePhysicalSkeletons()
    {
        FindVariablePhysicalLowerSkeletonConstant(rootVariablePhysicalSkeleton);
        for (int i = 0; i < variablePhysicalLowerBones.Count; i++)
        {
            FindVariablePhysicalLowerSkeletonConstant(variablePhysicalLowerBones[i]);
        }

        FindVariablePhysicalUpperSkeletonConstant(variablePhysicalUpperBones[0]);
        for (int i = 1; i < variablePhysicalUpperBones.Count; i++)
        {
            FindVariablePhysicalUpperSkeletonConstant(variablePhysicalUpperBones[i]);
        }
    }


    /// <summary>
    /// Deep search thought the kinematic lower-skeleton
    /// </summary>
    /// <param name="rootKinematic"></param>
    public void FindKinematicLowerSkeleton(Transform rootKinematic)
    {
        int count = rootKinematic.childCount;
        for (int i = 0; i < count; i++)
        {
            if (rootKinematic.GetChild(i).gameObject == kinematicSpine)
            {
                kinematicUpperBones.Add(rootKinematic.GetChild(i));
            }
            else if (rootKinematic.GetChild(i).gameObject == hipConnector)
            {
                continue;
            }
            else
            {
                kinematicLowerBones.Add(rootKinematic.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Deep search thought the kinematic upper-skeleton
    /// </summary>
    /// <param name="rootKinematic"></param>
    public void FindKinematicUpperSkeleton(Transform rootKinematic)
    {
        int count = rootKinematic.childCount;
        for (int i = 0; i < count; i++)
        {
            kinematicUpperBones.Add(rootKinematic.GetChild(i));
        }
    }

    /// <summary>
    /// Deep search thought the physical lower-skeleton
    /// </summary>
    /// <param name="rootPhysical"></param>
    public void FindPhysicalLowerSkeleton(Transform rootPhysical)
    {
        int count = rootPhysical.childCount;
        for (int i = 0; i < count; i++)
        {
            if (rootPhysical.GetChild(i).gameObject == physicalSpine)
            {
                physicalUpperBones.Add(rootPhysical.GetChild(i));
            }
            else if (rootPhysical.GetChild(i).gameObject == hipConnector)
            {
                continue;
            }
            else
            {
                physicalLowerBones.Add(rootPhysical.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Deep search thought the physical upper-skeleton
    /// </summary>
    /// <param name="rootPhysical"></param>
    public void FindPhysicalUpperSkeleton(Transform rootPhysical)
    {
        int count = rootPhysical.childCount;
        for (int i = 0; i < count; i++)
        {
            physicalUpperBones.Add(rootPhysical.GetChild(i));
        }
    }

    
    /// <summary>
    /// Deep search thought the variable visible lower-skeleton
    /// </summary>
    /// <param name="rootPhysicalConstant"></param>
    public void FindPhysicalLowerSkeletonConstant(Transform rootPhysicalConstant)
    {
        int count = rootPhysicalConstant.childCount;
        for (int i = 0; i < count; i++)
        {
            if (rootPhysicalConstant.GetChild(i).gameObject == variableSpine)
            {
                variableUpperBones.Add(rootPhysicalConstant.GetChild(i));
            }
            else if (rootPhysicalConstant.GetChild(i).gameObject == hipConnector)
            {
                continue;
            }
            else
            {
                variableLowerBones.Add(rootPhysicalConstant.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Deep search thought the variable visible upper skeleton
    /// </summary>
    /// <param name="rootPhysicalConstant"></param>
    public void FindPhysicalUpperSkeletonConstant(Transform rootPhysicalConstant)
    {
        int count = rootPhysicalConstant.childCount;
        for (int i = 0; i < count; i++)
        {
            variableUpperBones.Add(rootPhysicalConstant.GetChild(i));
        }
    }


    // TEST
    /// <summary>
    /// Deep search thought the variable visible lower-skeleton
    /// </summary>
    /// <param name="rootPhysicalConstant"></param>
    public void FindVariablePhysicalLowerSkeletonConstant(Transform rootVariablePhysicalConstant)
    {
        int count = rootVariablePhysicalConstant.childCount;
        for (int i = 0; i < count; i++)
        {
            if (rootVariablePhysicalConstant.GetChild(i).gameObject == variablePhysicalSpine)
            {
                variablePhysicalUpperBones.Add(rootVariablePhysicalConstant.GetChild(i));
            }
            else if (rootVariablePhysicalConstant.GetChild(i).gameObject == hipConnector)
            {
                continue;
            }
            else
            {
                variablePhysicalLowerBones.Add(rootVariablePhysicalConstant.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Deep search thought the variable visible upper skeleton
    /// </summary>
    /// <param name="rootPhysicalConstant"></param>
    public void FindVariablePhysicalUpperSkeletonConstant(Transform rootVariablePhysicalConstant)
    {
        int count = rootVariablePhysicalConstant.childCount;
        for (int i = 0; i < count; i++)
        {
            variablePhysicalUpperBones.Add(rootVariablePhysicalConstant.GetChild(i));
        }
    }

    /// <summary>
    /// Synchronizes bones.
    /// </summary>
    /// <param name="toBones"></param>
    /// <param name="fromBones"></param>
    public void SyncBones(List<Transform> toBones, List<Transform> fromBones)
    {
        int idx = 0;
        foreach (Transform trf in fromBones)
        {
            trf.position = toBones[idx].position;
            idx++;
        }
    }
}
