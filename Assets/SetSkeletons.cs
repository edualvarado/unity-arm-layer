using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SetSkeletons : MonoBehaviour
{
    [Header("Skeletons")]
    [SerializeField] private Transform rootKinematicSkeleton;
    public List<Transform> kinematicBones = new List<Transform>();
    [SerializeField] private Transform rootPhysicalSkeleton;
    public List<Transform> physicalBones = new List<Transform>();

    
    [Header("Physical Arms")]
    [SerializeField] private GameObject physicalLeftArm;
    [SerializeField] private GameObject physicalRightArm;
    public List<Transform> physicalLeftArmBones = new List<Transform>();
    public List<Transform> physicalRightArmBones = new List<Transform>();
    
    [Header("Kinematic Arms")]
    [SerializeField] private GameObject kinematicLeftArm;
    [SerializeField] private GameObject kinematicRightArm;
    public List<Transform> kinematicLeftArmBones = new List<Transform>();
    public List<Transform> kinematicRightArmBones = new List<Transform>();

    [Header("Auxiliary Shoulders")]
    [SerializeField] private Transform leftPhysicalShoulder;
    [SerializeField] private Transform rightPhysicalShoulder;
    [SerializeField] private Transform leftKinematicMatchingPart;
    [SerializeField] private Transform rightKinematicMatchingPart;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSkeletons();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        MatchSkeletons(kinematicBones, physicalBones);
    }

    /// <summary>
    /// Save both skeletons in lists (kinematic and physical)
    /// </summary>
    private void InitSkeletons()
    {
        FindKinematicSkeleton(rootKinematicSkeleton);
        for (int i = 0; i < kinematicBones.Count; i++)
        {
            FindKinematicSkeleton(kinematicBones[i]);
        }

        FindPhysicalSkeleton(rootPhysicalSkeleton);
        for (int i = 0; i < physicalBones.Count; i++)
        {
            FindPhysicalSkeleton(physicalBones[i]);
        }

    }

    /// <summary>
    /// Deep search thought the kinematic skeleton and divide arms and body
    /// </summary>
    /// <param name="rootKinematic"></param>
    public void FindKinematicSkeleton(Transform rootKinematic)
    {
        int count = rootKinematic.childCount;
        for (int i = 0; i < count; i++)
        {
            if (rootKinematic.GetChild(i).gameObject == kinematicLeftArm)
            {
                kinematicLeftArmBones.Add(rootKinematic.GetChild(i));

            }
            else if (rootKinematic.GetChild(i).gameObject == kinematicRightArm)
            {
                kinematicRightArmBones.Add(rootKinematic.GetChild(i));
            }
            else
            {
                kinematicBones.Add(rootKinematic.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Deep search thought the physical skeleton and divide arms and body
    /// </summary>
    public void FindPhysicalSkeleton(Transform rootPhysical)
    {
        int count = rootPhysical.childCount;
        for (int i = 0; i < count; i++)
        {
            if (rootPhysical.GetChild(i).gameObject == physicalLeftArm)
            {
                physicalLeftArmBones.Add(rootPhysical.GetChild(i));

            }
            else if (rootPhysical.GetChild(i).gameObject == physicalRightArm)
            {
                physicalRightArmBones.Add(rootPhysical.GetChild(i));
            }
            else
            {
                physicalBones.Add(rootPhysical.GetChild(i));
            }
        }
    }

    /// <summary>
    /// Matches both kinematic and physical skeletons except for the arms, which are still physically-based.
    /// </summary>
    /// <param name="kinematicBones"></param>
    /// <param name="physicalBones"></param>
    public void MatchSkeletons(List<Transform> kinematicBones, List<Transform> physicalBones)
    {
        rootPhysicalSkeleton.position = rootKinematicSkeleton.position;

        leftPhysicalShoulder.position = leftKinematicMatchingPart.position;
        rightPhysicalShoulder.position = rightKinematicMatchingPart.position;

        int idx = 0;
        foreach (Transform trf in physicalBones)
        {
            trf.position = kinematicBones[idx].position;
            idx++;
        }
    }

}
