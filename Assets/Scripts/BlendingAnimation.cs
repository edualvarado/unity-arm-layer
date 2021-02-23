using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendingAnimation : MonoBehaviour
{
    public SetSkeletons setSkeletons;
    public List<Transform> physicalUpperBody;
    public List<Transform> kinematicUpperBody;
    //[Range(0,1f)] public float blendingFactor = 0f;

    //public ConfigurableJoint hipsCJ;

    // Start is called before the first frame update
    void Start()
    {
        setSkeletons = this.GetComponent<SetSkeletons>();
        physicalUpperBody = setSkeletons.physicalUpperBones;
        kinematicUpperBody = setSkeletons.kinematicUpperBones;

        
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine("BlendingUpperBody");
    }


    /*
    IEnumerator BlendingUpperBody()
    {
        //cube.transform.position = Vector3.Lerp(Vector3.zero, Vector3.one, blendingFactor);
         
        int idx = 0;
        foreach(Transform trf in physicalUpperBody)
        {
            trf.position = Vector3.Lerp(trf.position, kinematicUpperBody[idx].position, blendingFactor);
            idx++;
            if (idx == 3)
                break;
            yield return null;
        }
    }
    */


}
