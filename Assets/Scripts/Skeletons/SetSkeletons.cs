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

    [Header("Interpolated Skeleton")]
    [SerializeField] private Transform rootInterpolatedSkeleton;
    [SerializeField] private GameObject interpolatedSpine;
    public List<Transform> interpolatedLowerBones = new List<Transform>();
    public List<Transform> interpolatedUpperBones = new List<Transform>();

    [Header("Connector")]
    public GameObject hipConnector;

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        InitSkeletons();
    }

    // Start when is called the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Sync lower-body
        //SyncBones(kinematicLowerBones, physicalLowerBones);
        //SyncBones(kinematicLowerBones, interpolatedLowerBones);

        // Should we fix by hard-code the root and first spine bone for both skeletons? -> Produce vibrations
        //rootPhysicalSkeleton.position = rootKinematicSkeleton.position;
        //rootPhysicalSkeleton.rotation = rootKinematicSkeleton.rotation;

        // Only if third skeleton is available
        //rootInterpolatedSkeleton.position = rootKinematicSkeleton.position;
        //rootInterpolatedSkeleton.rotation = rootKinematicSkeleton.rotation;
    }

    /// <summary>
    /// Save both, kinematic and physical skeletons, in their respective lists.
    /// </summary>
    private void InitSkeletons()
    {
        // Create list of skeletons
        CreateKinematicSkeletons();
        CreatePhysicalSkeletons();
        CreateInterpolatedSkeleton();
    }

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
    /// Divide the interpolated visible skeleton in upper- and lower-body parts.
    /// </summary>
    private void CreateInterpolatedSkeleton()
    {
        FindPhysicalLowerSkeletonConstant(rootInterpolatedSkeleton);
        for (int i = 0; i < physicalLowerBones.Count; i++)
        {
            FindPhysicalLowerSkeletonConstant(interpolatedLowerBones[i]);
        }

        FindPhysicalUpperSkeletonConstant(interpolatedUpperBones[0]);
        for (int i = 1; i < interpolatedUpperBones.Count; i++)
        {
            FindPhysicalUpperSkeletonConstant(interpolatedUpperBones[i]);
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
    /// Deep search thought the interpolated visible lower-skeleton
    /// </summary>
    /// <param name="rootPhysicalConstant"></param>
    public void FindPhysicalLowerSkeletonConstant(Transform rootPhysicalConstant)
    {
        int count = rootPhysicalConstant.childCount;
        for (int i = 0; i < count; i++)
        {
            if (rootPhysicalConstant.GetChild(i).gameObject == interpolatedSpine)
            {
                interpolatedUpperBones.Add(rootPhysicalConstant.GetChild(i));
            }
            else if (rootPhysicalConstant.GetChild(i).gameObject == hipConnector)
            {
                continue;
            }
            else
            {
                interpolatedLowerBones.Add(rootPhysicalConstant.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Deep search thought the interpolated visible upper skeleton
    /// </summary>
    /// <param name="rootPhysicalConstant"></param>
    public void FindPhysicalUpperSkeletonConstant(Transform rootPhysicalConstant)
    {
        int count = rootPhysicalConstant.childCount;
        for (int i = 0; i < count; i++)
        {
            interpolatedUpperBones.Add(rootPhysicalConstant.GetChild(i));
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
            trf.rotation = toBones[idx].rotation;
            idx++;
        }
    }
}
