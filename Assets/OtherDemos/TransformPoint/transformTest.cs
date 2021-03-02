using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transformTest : MonoBehaviour
{
    public Transform parentCube;
    public Vector3 thePosition;
    public Vector3 theInversePosition;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Transform parentCube: " + parentCube.position);
        Debug.Log("Local Transform parentCube " + parentCube.localPosition);

        Debug.Log("Transform childCube: " + this.transform.position);
        Debug.Log("Local Transform childCube " + this.transform.localPosition);

        // Takes a local point relative to parentCube and calculates its world position
        thePosition = parentCube.TransformPoint(Vector3.right * 2);
        Debug.Log("parentCube.TransformPoint(Vector3.right * 2) Local to Global: " + thePosition);

        // Takes a global point and converts it to a local point relative to parentCube.
        theInversePosition = parentCube.InverseTransformPoint(Vector3.up * 3);
        Debug.Log("parentCube.InverseTransformPoint(Vector3.up * 3) Glocal to Local: " + theInversePosition);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
