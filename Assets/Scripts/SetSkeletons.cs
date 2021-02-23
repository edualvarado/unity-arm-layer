using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Animations.Rigging;
using System;

public class SetSkeletons : MonoBehaviour
{
    [Header("Kinematic Skeleton")]
    [SerializeField] private Transform rootKinematicSkeleton;
    public List<Transform> kinematicLowerBones = new List<Transform>();
    public List<Transform> kinematicUpperBones = new List<Transform>();

    [Header("Physical Skeleton")]
    [SerializeField] private Transform rootPhysicalSkeleton;
    public List<Transform> physicalLowerBones = new List<Transform>();
    public List<Transform> physicalUpperBones = new List<Transform>();

    [Header("Physical Skeleton (Constant)")]
    [SerializeField] private Transform rootPhysicalSkeletonConstant;
    public List<Transform> physicalLowerBonesConstant = new List<Transform>();
    public List<Transform> physicalUpperBonesConstant = new List<Transform>();

    [Header("Dividers")]
    //[SerializeField] private Transform rootPhysicalSkeletonConstant;
    [SerializeField] private GameObject kinematicSpine;
    [SerializeField] private GameObject physicalSpine;
    [SerializeField] private GameObject physicalSpineConstant;
    //[SerializeField] private GameObject kinematicSpine1;
    //[SerializeField] private GameObject physicalSpine1;
    //[SerializeField] private GameObject physicalSpine1Constant;
    [SerializeField] private GameObject kinematicLeftForeArm;
    [SerializeField] private GameObject physicalLeftForeArm;
    [SerializeField] private GameObject physicalLeftForeArmConstant;

    [SerializeField] private GameObject hipConnector;

    [Range(0, 1f)] public float blendingFactor = 0f;

    //public Rigidbody _rbHips;
    public Transform cube;

    AffineTransform affine_rt_from;
    AffineTransform affine_rt_to;
    AffineTransform T;

    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererSurfaceDisplay;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererJointsDisplay;
    [SerializeField] private Transform[] bonesNew;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSkeletons();
        foreach (Transform trm in skinnedMeshRendererSurfaceDisplay.bones)
        {
            Debug.Log("Bone: " + trm.name);
        }
        skinnedMeshRendererSurfaceDisplay.bones = bonesNew;
        skinnedMeshRendererJointsDisplay.bones = bonesNew;

        //_rbHips = rootPhysicalSkeleton.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        // Always match (for now) lower-body
        MatchLegs(kinematicLowerBones, physicalLowerBones);
        MatchLegs(kinematicLowerBones, physicalLowerBonesConstant);
           
        // Blending using parameter
        BlendingUpperBody(kinematicUpperBones, physicalUpperBones, physicalUpperBonesConstant);
        
        // TEST: Was using it to keep to physical bodies in the same position/rotation
        //MatchUpperPhysical(physicalUpperBonesConstant, physicalUpperBones);

        // Should we fix by hard-code the root and first spine bone for both skeletons?
        rootPhysicalSkeleton.position = rootKinematicSkeleton.position;

        // TEST
        // 1. Just fix
        //physicalSpine.transform.position = kinematicSpine.transform.position;

        // TEST
        /*
        // 2. Affine Transformation Spine
        affine_rt_from.translation = physicalSpine.transform.position;
        affine_rt_from.rotation = physicalSpine.transform.rotation;

        affine_rt_to.translation = kinematicSpine.transform.position;
        affine_rt_to.rotation = kinematicSpine.transform.rotation;

        T.translation = (1 - blendingFactor) * affine_rt_from.translation + blendingFactor * affine_rt_to.translation;
        T.rotation = Quaternion.Lerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactor);

        physicalSpineConstant.transform.position = T.translation;
        physicalSpineConstant.transform.rotation = T.rotation;

        // 2. Affine Transformation LeftForeArm
        affine_rt_from.translation = physicalLeftForeArm.transform.position;
        affine_rt_from.rotation = physicalLeftForeArm.transform.rotation;

        affine_rt_to.translation = kinematicLeftForeArm.transform.position;
        affine_rt_to.rotation = kinematicLeftForeArm.transform.rotation;

        T.translation = (1 - blendingFactor) * affine_rt_from.translation + blendingFactor * affine_rt_to.translation;
        T.rotation = Quaternion.Lerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactor);

        physicalLeftForeArmConstant.transform.position = T.translation;
        physicalLeftForeArmConstant.transform.rotation = T.rotation;
        */

        // TEST
        /*
        physicalSpine1.transform.position = kinematicSpine1.transform.position;
        physicalSpine2.transform.position = kinematicSpine2.transform.position;
        physicalSpine3.transform.position = kinematicSpine3.transform.position;
        physicalLeftShoulder.transform.position = kinematicLeftShoulder.transform.position;
        physicalRightShoulder.transform.position = kinematicRightShoulder.transform.position;
        */
    }

    private void FixedUpdate()
    {
        
    }

    /// <summary>
    /// Save both skeletons in lists (kinematic and physical)
    /// </summary>
    private void InitSkeletons()
    {
        CreateKinematicSkeletons();
        CreatePhysicalSkeletons();
        CreatePhysicalSkeletonsConstant();

    }

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

    private void CreatePhysicalSkeletonsConstant()
    {
        FindPhysicalLowerSkeletonConstant(rootPhysicalSkeletonConstant);
        for (int i = 0; i < physicalLowerBones.Count; i++)
        {
            FindPhysicalLowerSkeletonConstant(physicalLowerBonesConstant[i]);
        }

        FindPhysicalUpperSkeletonConstant(physicalUpperBonesConstant[0]);
        for (int i = 1; i < physicalUpperBonesConstant.Count; i++)
        {
            FindPhysicalUpperSkeletonConstant(physicalUpperBonesConstant[i]);
        }
    }

    /// <summary>
    /// Deep search thought the kinematic lower skeleton
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
    /// Deep search thought the kinematic upper skeleton
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
    /// Deep search thought the physical lower skeleton
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
    /// Deep search thought the kinematic upper skeleton
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
    /// Deep search thought the physical lower skeleton
    /// </summary>
    /// <param name="rootPhysical"></param>
    public void FindPhysicalLowerSkeletonConstant(Transform rootPhysicalConstant)
    {
        int count = rootPhysicalConstant.childCount;
        for (int i = 0; i < count; i++)
        {
            if (rootPhysicalConstant.GetChild(i).gameObject == physicalSpineConstant)
            {
                physicalUpperBonesConstant.Add(rootPhysicalConstant.GetChild(i));
            }
            else if (rootPhysicalConstant.GetChild(i).gameObject == hipConnector)
            {
                continue;
            }
            else
            {
                physicalLowerBonesConstant.Add(rootPhysicalConstant.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Deep search thought the kinematic upper skeleton
    /// </summary>
    /// <param name="rootPhysical"></param>
    public void FindPhysicalUpperSkeletonConstant(Transform rootPhysicalConstant)
    {
        int count = rootPhysicalConstant.childCount;
        for (int i = 0; i < count; i++)
        {
            physicalUpperBonesConstant.Add(rootPhysicalConstant.GetChild(i));
        }
    }


    /// <summary>
    /// Matches both kinematic and physical legs.
    /// </summary>
    /// <param name="kinematicLowerBones"></param>
    /// <param name="physicalLowerBones"></param>
    public void MatchLegs(List<Transform> kinematicLowerBones, List<Transform> physicalLowerBones)
    {
        int idx = 0;
        foreach (Transform trf in physicalLowerBones)
        {
            trf.position = kinematicLowerBones[idx].position;
            idx++;
        }
    }

    /// <summary>
    /// Matches both kinematic and physical upper bones.
    /// </summary>
    /// <param name="kinematicUpperBones"></param>
    /// <param name="physicalUpperBones"></param>
    public void MatchUpperPhysical(List<Transform> physicalUpperBonesConstant, List<Transform> physicalUpperBones)
    {
        int idx = 0;
        foreach (Transform trf in physicalUpperBonesConstant)
        {
            trf.position = physicalUpperBones[idx].position;
            trf.rotation = physicalUpperBones[idx].rotation;
            idx++;
        }
    }

    private void BlendingUpperBody(List<Transform> kinematicUpperBones, List<Transform> physicalUpperBones, List<Transform> physicalUpperBonesConstant)
    {
        int idx = 0;
        foreach (Transform trf in physicalUpperBonesConstant)
        {
            affine_rt_from.translation = physicalUpperBones[idx].transform.position;
            affine_rt_from.rotation = physicalUpperBones[idx].transform.rotation;

            affine_rt_to.translation = kinematicUpperBones[idx].transform.position;
            affine_rt_to.rotation = kinematicUpperBones[idx].transform.rotation;

            T.translation = (1 - blendingFactor) * affine_rt_from.translation + blendingFactor * affine_rt_to.translation;
            T.rotation = Quaternion.Lerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactor);

            trf.position = T.translation;
            trf.rotation = T.rotation;

            idx++;

        }
    }
}
