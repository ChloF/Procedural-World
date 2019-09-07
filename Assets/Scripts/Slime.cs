using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Slime : Entity
{
    public float horizontalForce;
    public float verticalForce;
    public float moveChance;
    public bool displayVisionRadius;
    public int visionRadiusDisplayVertices;

    private GameObject[] food;
    private LineRenderer lr;
    private Rigidbody rb;

    public override void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();

        base.Start();
    }

    public void Update()
    {
        if (displayVisionRadius)
        {
            lr.enabled = true;
            DrawVisionRadius();
        }
        else
        {
            lr.enabled = false;
        }
    }

    public override void OnTick()
    {
        base.OnTick();

        food = GameObject.FindGameObjectsWithTag("Food");

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