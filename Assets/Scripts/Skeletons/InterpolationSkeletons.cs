using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class InterpolationSkeletons : MonoBehaviour
{
    #region Variables

    private SetSkeletons setSkeletons;
    public List<Transform> physicalUpperBones, physicalLowerBones;
    public List<Transform> kinematicUpperBones, kinematicLowerBones;
    public List<Transform> interpolatedUpperBones, interpolatedLowerBones;

    [Header("Interpolation Coefficient")]
    public bool interpolateArms = false;
    [Range(0, 1f)] public float blendingFactorSpine = 0f;
    [Range(0, 8)] public int rangeCoefficient = 0;
    [Range(0, 12)] public int rangeCoefficientWithArms = 0;
    //public bool interpolateLeftArm;
    //public bool interpolateRightArm;
    //[Range(0, 1f)] public float blendingFactorArms = 0f;
    //[Range(0, 1f)] public float blendingFactorLeftArm = 0f;
    //[Range(0, 1f)] public float blendingFactorRightArm = 0f;
    public bool resetSkeletonToKinematic;
    public bool resetSkeletonToPhysics;


    /// <summary>
    /// Creates an array instance of the enum HumanBodyBones with all the important spine bones we need to use.
    /// This is usefull to select which bones (and in which order) want to interpolate.
    /// </summary>
    private static HumanBodyBones[] humanSpine = new HumanBodyBones[] {
            HumanBodyBones.Hips,
            HumanBodyBones.Spine,
            HumanBodyBones.Chest,
            HumanBodyBones.UpperChest,
            HumanBodyBones.Neck,
            HumanBodyBones.Head,

            HumanBodyBones.LeftShoulder,
            HumanBodyBones.RightShoulder,

            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm
        };

    private static HumanBodyBones[] humanLeftArm = new HumanBodyBones[] {
            HumanBodyBones.LeftShoulder,
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.LeftLowerArm
        };

    private static HumanBodyBones[] humanRightArm = new HumanBodyBones[] {
            HumanBodyBones.RightShoulder,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm
        };

    private readonly static int bonesSpine = humanSpine.Length;
    private readonly static int bonesLeftArm = humanLeftArm.Length;
    private readonly static int bonesRightArm = humanRightArm.Length;  

    public Transform[] variableKinematicUpperBones;
    public Transform[] variablePhysicalUpperBones;
    public Transform[] variableInterpolatedUpperBones;

    public Transform[] variableKinematicLeftArm;
    public Transform[] variablePhysicalLeftArm;
    public Transform[] variableInterpolatedLeftArm;

    public Transform[] variableKinematicRightArm;
    public Transform[] variablePhysicalRightArm;
    public Transform[] variableInterpolatedRightArm;

    public Animator _kinAnim;
    public Animator _phyAnim;
    public Animator _intAnim;

    private AffineTransform affine_rt_from;
    private AffineTransform affine_rt_to;
    private AffineTransform T;

    #endregion

    void Start()
    {
        _intAnim = GetComponent<Animator>();

        // Get lists from SetSkeletons
        RetrieveBones();
    }

    public static int Bone2Index(HumanBodyBones bone)
    {
        switch (bone)
        {
            case HumanBodyBones.Hips: return 0;
            case HumanBodyBones.Chest: return 1;
            case HumanBodyBones.Head: return 2;
            case HumanBodyBones.RightLowerLeg: return 3;
            case HumanBodyBones.LeftLowerLeg: return 4;
            case HumanBodyBones.RightUpperLeg: return 5;
            case HumanBodyBones.LeftUpperLeg: return 6;
            case HumanBodyBones.RightLowerArm: return 7;
            case HumanBodyBones.LeftLowerArm: return 8;
            case HumanBodyBones.RightUpperArm: return 9;
            case HumanBodyBones.LeftUpperArm: return 10;
        }
        return -1;
    }

    /// <summary>
    /// Get Bones from SetSkeletons class
    /// </summary>
    private void RetrieveBones()
    {
        setSkeletons = this.GetComponent<SetSkeletons>();

        physicalUpperBones = setSkeletons.physicalUpperBones;
        physicalLowerBones = setSkeletons.physicalLowerBones;

        kinematicUpperBones = setSkeletons.kinematicUpperBones;
        kinematicLowerBones = setSkeletons.kinematicLowerBones;

        interpolatedUpperBones = setSkeletons.interpolatedUpperBones;
        interpolatedLowerBones = setSkeletons.interpolatedLowerBones;
    }

    // Update is called once per frame
    void Update()
    {
        
        // Extract at each time the position of each key bone skeleton.
        ExtractTransformsBones();

        // First version: Blend the visible skeleton
        //StartCoroutine(BlendingUpperBodyOld(kinematicUpperBones, physicalUpperBones, interpolatedUpperBones));

        // Second version: Only interpolates the key bones from the enum.
        StartCoroutine(BlendingKeyUpperBody(variableKinematicUpperBones, variablePhysicalUpperBones, variableInterpolatedUpperBones));
    }

    private void ExtractTransformsBones()
    {
        variableKinematicUpperBones = new Transform[bonesSpine];
        variablePhysicalUpperBones = new Transform[bonesSpine];
        variableInterpolatedUpperBones = new Transform[bonesSpine];

        variableKinematicLeftArm = new Transform[bonesLeftArm];
        variablePhysicalLeftArm = new Transform[bonesLeftArm];
        variableInterpolatedLeftArm = new Transform[bonesLeftArm];

        variableKinematicRightArm = new Transform[bonesRightArm];
        variablePhysicalRightArm = new Transform[bonesRightArm];
        variableInterpolatedRightArm = new Transform[bonesRightArm];

        for (int i = 0; i < bonesSpine; i++)
        {
            variableKinematicUpperBones[i] = _kinAnim.GetBoneTransform(humanSpine[i]);
            variablePhysicalUpperBones[i] = _phyAnim.GetBoneTransform(humanSpine[i]);
            variableInterpolatedUpperBones[i] = _intAnim.GetBoneTransform(humanSpine[i]);
        }

        for (int i = 0; i < bonesLeftArm; i++)
        {
            variableKinematicLeftArm[i] = _kinAnim.GetBoneTransform(humanLeftArm[i]);
            variablePhysicalLeftArm[i] = _phyAnim.GetBoneTransform(humanLeftArm[i]);
            variableInterpolatedLeftArm[i] = _intAnim.GetBoneTransform(humanLeftArm[i]);
        }

        for (int i = 0; i < bonesRightArm; i++)
        {
            variableKinematicRightArm[i] = _kinAnim.GetBoneTransform(humanRightArm[i]);
            variablePhysicalRightArm[i] = _phyAnim.GetBoneTransform(humanRightArm[i]);
            variableInterpolatedRightArm[i] = _intAnim.GetBoneTransform(humanRightArm[i]);
        }
    }

    
    /// <summary>
    /// Linear/Spherical Interpolation for bone transformations/quaternions between physical and kinematic skeleton.
    /// </summary>
    /// <param name="variableKinematicUpperBones"></param>
    /// <param name="variablePhysicalUpperBones"></param>
    /// <param name="variableInterpolatedUpperBones"></param>
    /// <returns></returns>
    IEnumerator BlendingKeyUpperBody(Transform[] variableKinematicUpperBones, Transform[] variablePhysicalUpperBones, Transform[] variableInterpolatedUpperBones)
    {
        if (!interpolateArms)
        {
            for (int i = 0; i < bonesSpine; i++)
            {
                variableInterpolatedUpperBones[i].position = variableKinematicUpperBones[i].position;
                variableInterpolatedUpperBones[i].rotation = variableKinematicUpperBones[i].rotation;
            }
        }

        if (interpolateArms)
        {
            for (int i = 0; i < bonesSpine; i++)
            {
                variableInterpolatedUpperBones[i].position = variableKinematicUpperBones[i].position;
                variableInterpolatedUpperBones[i].rotation = variableKinematicUpperBones[i].rotation;
            }
        }


        if (!interpolateArms)
        {
            for (int i = 0; i < rangeCoefficient; i++)
            {
                Debug.Log("Now interpolating:  " + humanSpine[i]);
                affine_rt_from.translation = variablePhysicalUpperBones[i].position;
                affine_rt_from.rotation = variablePhysicalUpperBones[i].rotation;

                affine_rt_to.translation = variableKinematicUpperBones[i].position;
                affine_rt_to.rotation = variableKinematicUpperBones[i].rotation;

                T.translation = (1 - blendingFactorSpine) * affine_rt_from.translation + blendingFactorSpine * affine_rt_to.translation;
                T.rotation = Quaternion.Slerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactorSpine);

                variableInterpolatedUpperBones[i].position = T.translation;
                variableInterpolatedUpperBones[i].rotation = T.rotation;
            } 
        }
        else
        {
            for (int i = 0; i < rangeCoefficientWithArms; i++)
            {
                Debug.Log("Now interpolating:  " + humanSpine[i]);
                affine_rt_from.translation = variablePhysicalUpperBones[i].position;
                affine_rt_from.rotation = variablePhysicalUpperBones[i].rotation;

                affine_rt_to.translation = variableKinematicUpperBones[i].position;
                affine_rt_to.rotation = variableKinematicUpperBones[i].rotation;

                T.translation = (1 - blendingFactorSpine) * affine_rt_from.translation + blendingFactorSpine * affine_rt_to.translation;
                T.rotation = Quaternion.Slerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactorSpine);

                variableInterpolatedUpperBones[i].position = T.translation;
                variableInterpolatedUpperBones[i].rotation = T.rotation;
            }
        }

        /*
        if(interpolateLeftArm)
        {
            for (int i = 0; i < bonesLeftArm; i++)
            {
                affine_rt_from.translation = variablePhysicalLeftArm[i].position;
                affine_rt_from.rotation = variablePhysicalLeftArm[i].rotation;

                affine_rt_to.translation = variableKinematicLeftArm[i].position;
                affine_rt_to.rotation = variableKinematicLeftArm[i].rotation;

                T.translation = (1 - blendingFactorArms) * affine_rt_from.translation + blendingFactorArms * affine_rt_to.translation;
                T.rotation = Quaternion.Slerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactorArms);

                variableInterpolatedLeftArm[i].position = T.translation;
                variableInterpolatedLeftArm[i].rotation = T.rotation;
            }
        }

        if (interpolateRightArm)
        {
            for (int i = 0; i < bonesRightArm; i++)
            {
                affine_rt_from.translation = variablePhysicalRightArm[i].position;
                affine_rt_from.rotation = variablePhysicalRightArm[i].rotation;

                affine_rt_to.translation = variableKinematicRightArm[i].position;
                affine_rt_to.rotation = variableKinematicRightArm[i].rotation;

                T.translation = (1 - blendingFactorArms) * affine_rt_from.translation + blendingFactorArms * affine_rt_to.translation;
                T.rotation = Quaternion.Slerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactorArms);

                variableInterpolatedRightArm[i].position = T.translation;
                variableInterpolatedRightArm[i].rotation = T.rotation;
            }
        }
        */

        if(resetSkeletonToKinematic)
        {
            for (int i = 0; i < bonesSpine; i++)
            {
                variableInterpolatedUpperBones[i].position = variableKinematicUpperBones[i].position;
                variableInterpolatedUpperBones[i].rotation = variableKinematicUpperBones[i].rotation;
            }
        }

        if (resetSkeletonToPhysics)
        {
            for (int i = 0; i < bonesSpine; i++)
            {
                variableInterpolatedUpperBones[i].position = variablePhysicalUpperBones[i].position;
                variableInterpolatedUpperBones[i].rotation = variablePhysicalUpperBones[i].rotation;
            }
        }

        // TODO: Fix when changing the rangeCoefficient

        yield return null;
    }

    /*
    IEnumerator BlendingAllUpperBody(List<Transform> kinematicUpperBones, List<Transform> physicalUpperBones, List<Transform> interpolatedUpperBones)
    {
        for (int i = 0; i < rangeCoefficient; i++)
        {
            affine_rt_from.translation = physicalUpperBones[i].position;
            affine_rt_from.rotation = physicalUpperBones[i].rotation;

            affine_rt_to.translation = kinematicUpperBones[i].position;
            affine_rt_to.rotation = kinematicUpperBones[i].rotation;

            T.translation = (1 - blendingFactor) * affine_rt_from.translation + blendingFactor * affine_rt_to.translation;
            T.rotation = Quaternion.Slerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactor);

            interpolatedUpperBones[i].position = T.translation;
            interpolatedUpperBones[i].rotation = T.rotation;
        }

        yield return null;
    }
    */

    /*
    /// <summary>
    /// Linear Interpolation for bone transformations/quaternions between physical and kinematic skeleton.
    /// </summary>
    /// <param name="kinematicUpperBones"></param>
    /// <param name="physicalUpperBones"></param>
    /// <param name="interpolatedUpperBones"></param>
    IEnumerator BlendingUpperBodyOld(List<Transform> kinematicUpperBones, List<Transform> physicalUpperBones, List<Transform> interpolatedUpperBones)
    {

        int idx = 0;
        foreach (Transform trf in interpolatedUpperBones)
        {
            affine_rt_from.translation = physicalUpperBones[idx].transform.position;
            affine_rt_from.rotation = physicalUpperBones[idx].transform.rotation;

            affine_rt_to.translation = kinematicUpperBones[idx].transform.position;
            affine_rt_to.rotation = kinematicUpperBones[idx].transform.rotation;

            T.translation = (1 - blendingFactor) * affine_rt_from.translation + blendingFactor * affine_rt_to.translation;
            T.rotation = Quaternion.Slerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactor);

            trf.position = T.translation;
            trf.rotation = T.rotation;

            idx++;
        }

        yield return null;
    }
    */
}
