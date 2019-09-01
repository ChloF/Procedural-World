using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Slime : MonoBehaviour
{
    public float horizontalForce;
    public float verticalForce;
    public bool alive = true;
    public float moveChance;
    public float tickRate = 1;
    public bool IsGrounded
    {
        get
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            LayerMask ground = LayerMask.NameToLayer("Environment");

            return Physics.Raycast(ray, 0.1f, ground);
        }
    }
    private WaitForSeconds tick;
    private Rigidbody rb;

    private void Start()
    {
        alive = true;
        rb = GetComponent<Rigidbody>();
        tick = new WaitForSeconds(1 / tickRate);

        StartCoroutine(Live());
    }

    IEnumerator Live()
    {
        while (alive)
        {
            OnTick();
            yield return tick;
        }

        StartCoroutine(Die());
        yield return null;
    }

    IEnumerator Die()
    {
        Destroy(this.gameObject);
        yield return null;
    }

    private void OnTick()
    {
        if(Random.value < moveChance)
        {
            if (IsGrounded)
            {
                float theta = Random.value * 2 * Mathf.PI;
                Vector3 hopDir = new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));
                Hop(hopDir, horizontalForce, verticalForce);
            }
        }
    }

    void Hop(Vector3 direction, float h, float v)
    {
        rb.AddForce(direction.normalized * h + Vector3.up * v, ForceMode.Impulse);
    }
}
