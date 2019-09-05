using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class CameraFly : MonoBehaviour
{
    public float speed;
    public float rotationSensitivity;
    
    //Keep track of total rotation for clamping.
    private float totalRotX;

    private Rigidbody rb;

    private void Start()
    {
        totalRotX = 0;
        transform.rotation = Quaternion.identity;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 force = new Vector3(Input.GetAxis("X"), Input.GetAxis("Y"), Input.GetAxis("Z"));
        force = transform.TransformDirection(force); //Convert force from local space to world space.
        force *= speed * Time.fixedDeltaTime * 100;

        rb.AddForce(force);

        float rotX = -Input.GetAxis("Mouse Y") * rotationSensitivity * Time.fixedDeltaTime;
        float rotY = Input.GetAxis("Mouse X") * rotationSensitivity * Time.fixedDeltaTime;
        
        //Clamp rotation between 60 degrees and -70 degrees.
        if((totalRotX < 60f || rotX < 0) && (totalRotX > -70f || rotX > 0))
        {
            totalRotX += rotX;
        }
        else
        {
            rotX = 0;
        }

        transform.Rotate(Vector3.right * rotX, Space.Self);
        transform.Rotate(Vector3.up * rotY, Space.World);
    }
}
