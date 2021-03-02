using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anchorTest : MonoBehaviour
{
    public bool autoConfigureConnectedAnchor;
    public Rigidbody connectedBody;
    public Vector3 anchor;
    public Vector3 anchorWorldPosition;

    public Vector3 connectedAnchor;
    public Vector3 connectedAnchorWorldPosition;

    public Vector3 positionError;


    // Start is called before the first frame update
    void Start()
    {
        var ourBody = GetComponent<Rigidbody>();
        anchor = new Vector3(0, 1, 0);

        if (autoConfigureConnectedAnchor)
        {
            anchorWorldPosition = ourBody.transform.TransformPoint(this.anchor);
            Debug.Log("anchorWorldPosition " + anchorWorldPosition);
            
            this.connectedAnchor = this.connectedBody != null ?
                this.connectedBody.transform.InverseTransformPoint(anchorWorldPosition) :
                anchorWorldPosition;
            Debug.Log("connectedAnchor " + connectedAnchor);

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var ourBody = GetComponent<Rigidbody>();

        // Finding a world position of this.anchor.
        anchorWorldPosition = ourBody.transform.TransformPoint(this.anchor);

        // Finding a world position of this.connectedAnchor. Note, that it's
        // already stored in this.connectedAnchor if this.connectedBody is null.
        connectedAnchorWorldPosition = this.connectedBody != null ?
            this.connectedBody.transform.TransformPoint(this.connectedAnchor) :
            this.connectedAnchor;

        // Here is how far we should move our body in order to co-locate 
        // this.anchor and this.connectedAnchor

        positionError = connectedAnchorWorldPosition - anchorWorldPosition;

        ourBody.position += positionError;

    }
}
