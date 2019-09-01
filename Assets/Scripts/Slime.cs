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
    public float tickRate = 10;
    public float visionDistance;
    public float groundCheckDist;
    public bool IsGrounded
    {
        get
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            return Physics.Raycast(ray, groundCheckDist, environmentMask);
        }
    }

    private LayerMask environmentMask;
    private WaitForSeconds tick;
    private Rigidbody rb;

    private void Start()
    {
        alive = true;
        environmentMask = ~LayerMask.NameToLayer("Environment");
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
        if(Random.value < moveChance && IsGrounded)
        {
            bool canMoveInDirection = false;
            Vector3 hopDir = Vector3.zero;

            while (!canMoveInDirection)
            {
                float theta = Random.value * 2 * Mathf.PI;
                hopDir = new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));

                Ray visionRay = new Ray(transform.position + Vector3.up, hopDir);
                canMoveInDirection = !Physics.Raycast(visionRay, visionDistance, environmentMask);
                Debug.DrawRay(transform.position + Vector3.up, hopDir * visionDistance, canMoveInDirection ? Color.green : Color.red, 1f);
            }

            Hop(hopDir, horizontalForce, verticalForce);
        }
    }

    void Hop(Vector3 direction, float h, float v)
    {
        rb.AddForce(direction.normalized * h + Vector3.up * v, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, visionDistance);
    }
}