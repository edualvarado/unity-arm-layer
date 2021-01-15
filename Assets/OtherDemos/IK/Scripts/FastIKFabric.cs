using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FastIKFabric : MonoBehaviour
{

    /// <summary>
    /// Chain length of bones
    /// </summary>
    /// 
    public int ChainLength = 2;

    /// <summary>
    /// Target the chain should bent to
    /// </summary>
    public Transform target;
    public Transform pole;

    /// <summary>
    /// Solver iterations per update
    /// </summary>
    public int interations = 10;

    /// <summary>
    /// Distance when the solver stops
    /// </summary>
    public float delta = 0.001f;

    /// <summary>
    /// Strength of going back to the start position
    /// </summary>
    [Range(0, 1f)]
    public float snapBackStrength = 1f;


    protected float[] bonesLength; // Target to Origin
    protected float completeLength;
    protected Transform[] bones;
    protected Vector3[] positions;
    protected Vector3[] startDirectionSucc;
    protected Quaternion[] startRotationBone;
    protected Quaternion startRotationTarget;
    protected Quaternion startRotationRoot;

    // Start is called before the first frame update
    private void Awake()
    {
        Init();
    }

    void Init()
    {
        bones = new Transform[ChainLength + 1]; // if we hace x chains, then bones are chains + 1
        positions = new Vector3[ChainLength + 1];
        bonesLength = new float[ChainLength]; // Chains length
        startDirectionSucc = new Vector3[ChainLength + 1];
        startRotationBone = new Quaternion[ChainLength + 1];

        // init fields
        if (target == null)
        {
            target = new GameObject(gameObject.name + "Target").transform;
            target.position = transform.position;
        }
        startRotationTarget = target.rotation;
        completeLength = 0;

        //init data
        var current = this.transform;
        for (var i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            startRotationBone[i] = current.rotation;

            if(i == bones.Length - 1)
            {
                //leaf
                startDirectionSucc[i] = target.position - current.position;
            }
            else // midbone
            {
                startDirectionSucc[i] = bones[i + 1].position - current.position;
                bonesLength[i] = (bones[i + 1].position - current.position).magnitude;
                completeLength += bonesLength[i];
            }

            current = current.parent;
        }

    }

    void LateUpdate()
    {
        ResolveIK();
    }

    void ResolveIK()
    {
        if (target == null)
        {
            Debug.Log("ASASD2");
            return;
        }


        if (bonesLength.Length != ChainLength)
            Init();

        //get position
        for (int i = 0; i < bones.Length; i++)
        {
            positions[i] = bones[i].position;
        }

        var rootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        var rootRotDiff = rootRot * Quaternion.Inverse(startRotationRoot);

        // calculation. 1st is possible to reach?
        if ((target.position - bones[0].position).sqrMagnitude >= completeLength * completeLength)
        {
            // strech it
            var direction = (target.position - positions[0]).normalized;
            for (int i = 1; i<positions.Length; i++) // set everythinf after root
            {
                positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
            }
        }
        else
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + rootRotDiff * startDirectionSucc[i], snapBackStrength);
            }

            for (int ite = 0; ite < interations; ite++)
            {
                //back, decreaseing hte positions - skip root bone (i not 0)
                for (int i = positions.Length -1; i>0; i--)
                {
                    if (i == positions.Length - 1)
                    {
                        positions[i] = target.position; // if is close enough, end effecter must be on target
                    }
                    else
                    {
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i]; // set in line on distance
                    }
                }

                // forward
                for (int i = 1; i < positions.Length; i++)
                {
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bonesLength[i-1];
                }

                //close enough?
                if ((positions[positions.Length - 1] - target.position).sqrMagnitude >= delta * delta)
                {
                    break;
                }
            }

            // move towards pole
            if (pole != null)
            {

                for (int i = 1; i < positions.Length - 1; i++)
                {
                    var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                    var projectedPole = plane.ClosestPointOnPlane(pole.position);
                    var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                    var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                    positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1]; // DONT FULLY UNDERSTAND THIS LINE
                }
            }

            // set position and rotation
            for (int i = 0; i < positions.Length; i++)
            {
                if (i == positions.Length - 1)
                    bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
                else
                    bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) * startRotationBone[i];
                bones[i].position = positions[i];

            }

        }



        //set position
        for (int i = 0; i < positions.Length; i++)
        {
            bones[i].position = positions[i];
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        var current = this.transform;
        for (int i = 0; i < ChainLength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            
            current = current.parent;

        }
    }
}
