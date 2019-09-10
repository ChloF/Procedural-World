using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float hungerValue;
    public float eatAnimationDuration;
    public float size;
    public float maxHeight;
    public float minHeight;
	
	//Static list of all food objects to be used by entities.
    public static List<Food> allFood = new List<Food>();

    private void Start()
    {
        allFood.Add(this);
        StartCoroutine(Spawned());
    }
	
	//Play a short animation when spawning in.
    private IEnumerator Spawned()
    {
        float t = 0;

        while(t < 1)
        {
            transform.localScale = t * Vector3.one;

            t += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponentInParent<Entity>();
        if (entity)
        {
            entity.Eat(hungerValue);

            StartCoroutine(Eaten(other.transform));
        }
    }
	
	//Play a short animation when being Eaten.
    private IEnumerator Eaten(Transform to)
    {
        float t = 0;

        Vector3 from = transform.position;
		
		//Go to the entity, while getting smaller.
        while (t < .5)
        {
            transform.position = Vector3.Lerp(from, to.position, 2*t);
            transform.localScale = (1 - t) * Vector3.one;
            t += Time.deltaTime / eatAnimationDuration;

            yield return null;
        }
		
		//Continue getting smaller while inside the entity.
        while (t < 1)
        {
            transform.position = to.position;
            transform.localScale = (1 - t) * Vector3.one;
            t += Time.deltaTime / eatAnimationDuration;

            yield return null;
        }
		
        allFood.Remove(this);
        Destroy(this.gameObject);

        yield return null;
    }
}
