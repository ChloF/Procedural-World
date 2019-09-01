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
            Vector2 hopDir = Random.insideUnitCircle.normalized;
            Hop(hopDir, horizontalForce, verticalForce);
        }
    }

    void Hop(Vector3 direction, float h, float v)
    {
        direction.y = 0;

        rb.AddForce(direction.normalized * h + Vector3.up * v, ForceMode.Impulse);
    }
}
