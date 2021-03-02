using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class InterpolationSkeletons : MonoBehaviour
{
    #region Variables

    private SetSkeletons setSkeletons;
    private List<Transform> physicalUpperBones; //, physicalLowerBones;
    private List<Transform> kinematicUpperBones; //, kinematicLowerBones;
    private List<Transform> variableUpperBones; //, variableLowerBones;

    // TEST
    private List<Transform> variablePhysicalUpperBones; //, variableLowerBones;

    [Header("Interpolation")]
    [Range(0, 1f)] public float blendingFactor = 0f;
    [Range(0, 1f)] public float blendingFactorRed = 0f;

    public SkinnedMeshRenderer skinnedMeshRendererSurfaceUpperBody;
    public SkinnedMeshRenderer skinnedMeshRendererJointsUpperBody;
    public List<Transform> bonesCorrectOrder;
    //public List<Transform> bonesSkinnedOrder;
    public Transform variableArmature;
    public Material variableSkeletonMaterial;

    private AffineTransform affine_rt_from;
    private AffineTransform affine_rt_to;
    private AffineTransform T;

    #endregion

    void Start()
    {
        RetrieveBones();

        SkinnedMeshRenderer variableSkinnedMeshRendererSurfaceUpperBody;
        SkinnedMeshRenderer variableSkinnedMeshRendererJointsUpperBody;

        variableSkinnedMeshRendererSurfaceUpperBody = Instantiate(skinnedMeshRendererSurfaceUpperBody, this.transform);
        variableSkinnedMeshRendererJointsUpperBody = Instantiate(skinnedMeshRendererJointsUpperBody, this.transform);
        
        skinnedMeshRendererSurfaceUpperBody.enabled = false;
        skinnedMeshRendererJointsUpperBody.enabled = false;
        variableSkinnedMeshRendererSurfaceUpperBody.enabled = true;
        variableSkinnedMeshRendererJointsUpperBody.enabled = true;

        foreach (Transform tfm in skinnedMeshRendererSurfaceUpperBody.bones)
        {
            bonesCorrectOrder.Add(TransformDeepChildExtension.FindDeepChild(variableArmature, tfm.name));
        }

        variableSkinnedMeshRendererSurfaceUpperBody.rootBone = variableArmature.GetChild(0).transform;
        variableSkinnedMeshRendererJointsUpperBody.rootBone = variableArmature.GetChild(0).transform;

        variableSkinnedMeshRendererSurfaceUpperBody.bones = bonesCorrectOrder.ToArray();
        variableSkinnedMeshRendererJointsUpperBody.bones = bonesCorrectOrder.ToArray();


        variableSkinnedMeshRendererSurfaceUpperBody.material = variableSkeletonMaterial;
    }

    /// <summary>
    /// Get Bones from SetSkeletons class
    /// </summary>
    private void RetrieveBones()
    {
        setSkeletons = this.GetComponent<SetSkeletons>();

        physicalUpperBones = setSkeletons.physicalUpperBones;
        //physicalLowerBones = setSkeletons.physicalLowerBones;

        kinematicUpperBones = setSkeletons.kinematicUpperBones;
        //kinematicLowerBones = setSkeletons.kinematicLowerBones;

        // Commented!
        variableUpperBones = setSkeletons.variableUpperBones;
        //variableLowerBones = setSkeletons.variableLowerBones;

        // TEST
        variablePhysicalUpperBones = setSkeletons.variablePhysicalUpperBones;

    }

    // Update is called once per frame
    void Update()
    {
        // Blend the visible skeleton
        StartCoroutine(BlendingUpperBody(kinematicUpperBones, physicalUpperBones, variableUpperBones));

        // We also blend the red physical skeleton
        //StartCoroutine(BlendingUpperBody(kinematicUpperBones, physicalUpperBones, variablePhysicalUpperBones));
        StartCoroutine(BlendingUpperBodyRedSync(variablePhysicalUpperBones, variableUpperBones));

        //TEST
        //StartCoroutine(BlendingUpperBody(kinematicUpperBones, variablePhysicalUpperBones, variableUpperBones));
        //StartCoroutine(BlendingUpperBodyRed(kinematicUpperBones, variableUpperBones, variablePhysicalUpperBones));
        //StartCoroutine(BlendingUpperBodyRedSync(variablePhysicalUpperBones, variableUpperBones));

    }

    /// <summary>
    /// Linear Interpolation for bone transformations/quaternions between physical and kinematic skeleton.
    /// </summary>
    /// <param name="kinematicUpperBones"></param>
    /// <param name="physicalUpperBones"></param>
    /// <param name="physicalUpperBonesConstant"></param>
    IEnumerator BlendingUpperBody(List<Transform> kinematicUpperBones, List<Transform> physicalUpperBones, List<Transform> physicalUpperBonesConstant)
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

        yield return null;
    }

    IEnumerator BlendingUpperBodyRed(List<Transform> kinematicUpperBones, List<Transform> physicalUpperBones, List<Transform> physicalUpperBonesConstant)
    {

        int idx = 0;
        foreach (Transform trf in physicalUpperBonesConstant)
        {
            affine_rt_from.translation = physicalUpperBones[idx].transform.position;
            affine_rt_from.rotation = physicalUpperBones[idx].transform.rotation;

            affine_rt_to.translation = kinematicUpperBones[idx].transform.position;
            affine_rt_to.rotation = kinematicUpperBones[idx].transform.rotation;

            T.translation = (1 - blendingFactorRed) * affine_rt_from.translation + blendingFactorRed * affine_rt_to.translation;
            T.rotation = Quaternion.Lerp(affine_rt_from.rotation, affine_rt_to.rotation, blendingFactorRed);

            trf.position = T.translation;
            trf.rotation = T.rotation;

            idx++;
        }

        yield return null;
    }

    IEnumerator BlendingUpperBodyRedSync(List<Transform> fromBones, List<Transform> toBones)
    {

        int idx = 0;
        foreach (Transform trf in fromBones)
        {
            trf.position = toBones[idx].position;
            trf.rotation = toBones[idx].rotation;
            idx++;
        }

        yield return null;
    }

}
