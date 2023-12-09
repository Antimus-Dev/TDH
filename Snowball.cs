//Created by: Liam Gilmore
//Snowball projectile for enemies in level 6
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    [SerializeField] private Transform mainCube;
    private Rigidbody rb;

    public bool startDirLeft = false; 

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
        if (startDirLeft)
            rb.AddForce(-1000f, 0, 0);
        else
            rb.AddForce(1000f, 0, 0);
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.parent.transform.position.x) > GameController.instance.StageHalfX)
        {
            int posNeg = 1;
            if (transform.parent.transform.position.x < 0)
            {
                posNeg = -1;
            }
            transform.parent.transform.position += new Vector3(
                (GameController.instance.StageHalfX * 2) * posNeg * -1, 0, 0
                );
        }

        if (Mathf.Abs(transform.parent.transform.position.y) > GameController.instance.StageHalfY)
        {
            int posNeg = 1;
            if (transform.parent.transform.position.y < 0)
            {
                posNeg = -1;
            }
            transform.parent.transform.position += new Vector3(
                0, (GameController.instance.StageHalfY * 2) * posNeg * -1, 0
                );
        }

        if (rb.velocity.magnitude < 500f || rb.angularVelocity.magnitude < 100f)
        {
            if (startDirLeft)
            {
                if (rb.velocity.x > 0)
                    rb.AddForce(500f, 0, 0);
                else
                    rb.AddForce(-500f, 0, 0);
            }
            else
            {
                if (rb.velocity.x < 0)
                    rb.AddForce(-500f, 0, 0);
                else
                    rb.AddForce(500f, 0, 0);
            }
                              
        }
    }
}
