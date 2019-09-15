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
            //Raycast to check for the ground.
            Ray ray = new Ray(transform.position, Vector3.down);
            return Physics.Raycast(ray, groundCheckDistance, environmentMask);
        }
    }

    [HideInInspector]
    public LayerMask environmentMask;

    public virtual void Start()
    {
        environmentMask = ~LayerMask.NameToLayer("Environment");

        StartCoroutine(Live());
    }

    public virtual IEnumerator Live()
    {
        alive = true;
        
        while (alive)
        {
            //Call OnTick tickRate times per second.
            OnTick();
            yield return new WaitForSeconds(1 / tickRate);
        }

        StartCoroutine(Die());
        yield return null;
    }

    public virtual void OnTick()
    {
        //Increase hunger over time.
        hunger += hungerRate / tickRate;
        
        //Die in the event of starvation.
        if (hunger >= 1)
        {
            alive = false;
        }
    }

    public virtual void Eat(float hungerValue)
    {
        hunger -= hungerValue;
        
        //Hunger can not go below 0.
        hunger = hunger > 0 ? hunger : 0;
    }

    public virtual IEnumerator Die()
    {
        Destroy(this.gameObject);
        yield return null;
    }
}
