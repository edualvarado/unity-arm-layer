using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

    [Header("Dividers")]
    [SerializeField] private GameObject kinematicSpine;
    [SerializeField] private GameObject physicalSpine;
    [SerializeField] private GameObject kinematicSpine1;
    [SerializeField] private GameObject physicalSpine1;
    [SerializeField] private GameObject kinematicSpine2;
    [SerializeField] private GameObject physicalSpine2;
    [SerializeField] private GameObject kinematicSpine3;
    [SerializeField] private GameObject physicalSpine3;
    [SerializeField] private GameObject kinematicLeftShoulder;
    [SerializeField] private GameObject physicalLeftShoulder;
    [SerializeField] private GameObject kinematicRightShoulder;
    [SerializeField] private GameObject physicalRightShoulder;
    [SerializeField] private GameObject hipConnector;

    private Rigidbody _rbHips;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSkeletons();

        _rbHips = rootPhysicalSkeleton.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MatchLegs(kinematicLowerBones, physicalLowerBones);

        // Should we fix by hard-code the root and first spine bone for both skeletons?
        rootPhysicalSkeleton.position = rootKinematicSkeleton.position;
        physicalSpine.transform.position = kinematicSpine.transform.position;
        
        /*
        physicalSpine1.transform.position = kinematicSpine1.transform.position;
        physicalSpine2.transform.position = kinematicSpine2.transform.position;
        physicalSpine3.transform.position = kinematicSpine3.transform.position;
        physicalLeftShoulder.transform.position = kinematicLeftShoulder.transform.position;
        physicalRightShoulder.transform.position = kinematicRightShoulder.transform.position;
        */
    }



    /// <summary>
    /// Save both skeletons in lists (kinematic and physical)
    /// </summary>
    private void InitSkeletons()
    {
        CreateKinematicSkeletons();
        CreatePhysicalSkeletons();
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
    public void MatchUpper(List<Transform> kinematicUpperBones, List<Transform> physicalUpperBones)
    {
        // TODO
    }
}
