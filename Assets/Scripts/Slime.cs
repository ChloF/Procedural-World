using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Slime : MonoBehaviour
{
    [Range(0, 1)]
    public float hunger = 0;
    [Range(0, 1)]
    public float thirst = 0;
    public float hungerRate;
    public float thirstRate;
    public float horizontalForce;
    public float verticalForce;
    public bool alive = true;
    public float moveChance;
    public float tickRate = 10;
    public float visionDistance;
    public float groundCheckDist;
    public int visionRadiusDisplayVertices;
    public bool IsGrounded
    {
        get
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            return Physics.Raycast(ray, groundCheckDist, environmentMask);
        }
    }

    private GameObject[] food;
    private LayerMask environmentMask;
    private LineRenderer lr;
    private Rigidbody rb;

    private void Start()
    {
        alive = true;
        environmentMask = ~LayerMask.NameToLayer("Environment");
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();

        StartCoroutine(Live());
    }

    private void Update()
    {
        DrawVisionRadius();
    }

    IEnumerator Live()
    {
        while (alive)
        {
            OnTick();
            yield return new WaitForSeconds(1 / tickRate);
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
        food = GameObject.FindGameObjectsWithTag("Food");

        hunger += hungerRate / tickRate;
        thirst += thirstRate / tickRate;

        if (hunger >= 1 || thirst >= 1)
        {
            alive = false;
        }

        if(Random.value < moveChance && IsGrounded)
        {
            bool randomDirValid = false;
            Vector3 randomDir = Vector3.zero;

            while (!randomDirValid)
            {
                float theta = Random.value * 2 * Mathf.PI;
                randomDir = new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));

                Ray visionRay = new Ray(transform.position + Vector3.up, randomDir);
                randomDirValid = !Physics.Raycast(visionRay, visionDistance, environmentMask);
                Debug.DrawRay(transform.position + Vector3.up, randomDir * visionDistance, randomDirValid ? Color.green : Color.red, 1f);
            }

            Vector3 foodDir = Vector3.zero;
            float foodDist = float.MaxValue;

            for (int i = 0; i < food.Length; i++)
            {
                if(!Physics.Linecast(transform.position, food[i].transform.position, environmentMask))
                {
                    float newFoodDist = Vector3.SqrMagnitude(food[i].transform.position - transform.position);

                    if (newFoodDist < foodDist)
                    {
                        foodDir = food[i].transform.position - transform.position;
                        foodDist = newFoodDist;
                    }
                }
            }

            Vector3 hopDir = Vector3.zero;

            if(foodDist < 2)
            {
                hopDir = foodDir;
            }
            else
            {
                hopDir = Vector3.Lerp(randomDir, foodDir, hunger * hunger);
            }

            hopDir = hopDir.normalized;
            Hop(hopDir, horizontalForce, verticalForce);
        }
    }

    void Hop(Vector3 direction, float h, float v)
    {
        rb.AddForce(direction.normalized * h + Vector3.up * v, ForceMode.Impulse);
    }

    public void Eat(float hungerValue)
    {
        hunger -= hungerValue;

        hunger = hunger > 0 ? hunger : 0;
    }

    void DrawVisionRadius()
    {
        lr.positionCount = visionRadiusDisplayVertices;

        for (int i = 0; i < visionRadiusDisplayVertices; i++)
        {
            Vector3 point = visionDistance * new Vector3(Mathf.Cos(i * 2 * Mathf.PI / visionRadiusDisplayVertices), 0, Mathf.Sin(i * 2 * Mathf.PI / visionRadiusDisplayVertices));
            lr.SetPosition(i, point);
        }
    }
}