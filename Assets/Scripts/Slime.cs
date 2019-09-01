using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Slime : MonoBehaviour
{
    public float horizontalForce;
    public float verticalForce;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Hop(transform.forward, horizontalForce, verticalForce);
        }
    }

    void Hop(Vector3 direction, float h, float v)
    {
        direction.y = 0;

        rb.AddForce(direction.normalized * h + Vector3.up * v, ForceMode.Impulse);
    }
}
