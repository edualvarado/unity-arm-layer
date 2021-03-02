using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwball : MonoBehaviour
{
    public Rigidbody bullet;
    float elapsed = 0f;
    float elapsed2 = 0f;

    public bool stopBullet = false;
    public float forceBullet = 1f;
    public float massBullet = 1f;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (!stopBullet)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 1f)
            {
                elapsed = elapsed % 1f;
                ThrowBullet();
            }
        }

    }

    void ThrowBullet()
    {
        Rigidbody instance = Instantiate(bullet);
        instance.mass = massBullet;
        instance.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.1f, this.transform.position.z);
        //instance.velocity = new Vector3 (0,0,-5);
        instance.AddForce(-Vector3.forward * forceBullet, ForceMode.Impulse);
        elapsed2 += Time.deltaTime;

        if (elapsed2 >= 5f)
        {
            elapsed2 = elapsed2 % 5f;
            Destroy(instance);

        }

    }
}
