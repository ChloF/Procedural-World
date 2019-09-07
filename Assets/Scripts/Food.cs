using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float hungerValue;
    public float eatAnimationDuration;
    public float size;

    private void Start()
    {
        LayerMask environmentMask = ~LayerMask.NameToLayer("Environment");
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue, environmentMask))
        {
            transform.position = hit.point + Vector3.up * size / 2;
        }
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

    private IEnumerator Eaten(Transform to)
    {
        float t = 0;

        Vector3 from = transform.position;

        while (t < .5)
        {
            transform.position = Vector3.Lerp(from, to.position, 2*t);
            transform.localScale = (1 - t) * Vector3.one;
            t += Time.deltaTime / eatAnimationDuration;

            yield return null;
        }

        while (t < 1)
        {
            transform.position = to.position;
            transform.localScale = (1 - t) * Vector3.one;
            t += Time.deltaTime / eatAnimationDuration;

            yield return null;
        }

        Destroy(this.gameObject);
    }
}
