using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float hungerValue;
    public float eatAnimationDuration;

    private void OnTriggerEnter(Collider other)
    {
        Slime slime = other.GetComponentInParent<Slime>();
        if (slime)
        {
            slime.Eat(hungerValue);

            StartCoroutine(Eaten(other.transform));
        }
    }

    private IEnumerator Eaten(Transform to)
    {
        float t = 0;

        Vector3 from = transform.position;

        while (t < 1)
        {
            transform.position = Vector3.Lerp(from, to.position, t);
            transform.localScale = (1 - t) * Vector3.one;
            t += Time.deltaTime / eatAnimationDuration;

            yield return null;
        }

        Destroy(this.gameObject);
    }
}
