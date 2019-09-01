using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class CameraFly : MonoBehaviour
{
    public float speed;
    public float rotationSensitivity;

    private float totalRotX;

    private Rigidbody rb;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        totalRotX = 0;
        transform.rotation = Quaternion.identity;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 force = new Vector3(Input.GetAxis("X"), Input.GetAxis("Y"), Input.GetAxis("Z"));
        force = transform.TransformDirection(force);
        force *= speed * Time.deltaTime * 100;

        rb.AddForce(force);

        float rotX = -Input.GetAxis("Mouse Y") * rotationSensitivity * Time.deltaTime;
        float rotY = Input.GetAxis("Mouse X") * rotationSensitivity * Time.deltaTime;

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
