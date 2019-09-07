using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Range(0, 1)]
    public float hunger = 0;
    public float hungerRate;
    public bool alive = true;
    public float tickRate = 10;
    public float visionDistance;
    public float groundCheckDistance = 0.2f;
    public bool IsGrounded
    {
        get
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            return Physics.Raycast(ray, groundCheckDistance, environmentMask);
        }
    }

    [HideInInspector]
    public LayerMask environmentMask;

    public virtual void Start()
    {
        StartCoroutine(Live());
        environmentMask = ~LayerMask.NameToLayer("Environment");
    }

    IEnumerator Live()
    {
        alive = true;

        while (alive)
        {
            OnTick();
            yield return new WaitForSeconds(1 / tickRate);
        }

        StartCoroutine(Die());
        yield return null;
    }

    public virtual void OnTick()
    {
        hunger += hungerRate / tickRate;

        if (hunger >= 1)
        {
            alive = false;
        }
    }

    public void Eat(float hungerValue)
    {
        hunger -= hungerValue;

        hunger = hunger > 0 ? hunger : 0;
    }

    IEnumerator Die()
    {
        Destroy(this.gameObject);
        yield return null;
    }
}
