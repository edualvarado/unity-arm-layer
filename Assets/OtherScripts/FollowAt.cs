using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAt : MonoBehaviour
{
    public GameObject followAt;
    public Vector3 cameraOffset = new Vector3(0f,2f,-2f);

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = followAt.transform.position + cameraOffset * 1f;
    }
}
