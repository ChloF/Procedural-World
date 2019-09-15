using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Slime : Entity
{
    public float horizontalForce;
    public float verticalForce;
    public float moveRate;
    public bool displayVisionRadius;
    public int visionRadiusDisplayVertices;

    private LineRenderer lr; //LineRenderer to draw circle visualising vision distance.
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
        
        //Move if on the ground, averaging moveRate movements per second.
        if(Random.value < moveRate / tickRate && IsGrounded)
        {
            Vector3 hopDir = GetMoveDirection();
            Hop(hopDir, horizontalForce, verticalForce);
        }
    }

    private Vector3 GetMoveDirection()
    {
        //Get a random direction, checking to make sure it isn't obstructed.
        bool randomDirValid = false;
        Vector3 randomDir = Vector3.zero;

        while (!randomDirValid)
        {
            randomDir = RandomDirection();
            randomDirValid = CheckDirection(randomDir);
        }
        
        //Get the direction of the closest piece of food.
        Vector3 foodDir = FindClosestFoodDirection();

        //Set hopDir based on the other directions.
        Vector3 hopDir = Vector3.zero;
        if(foodDir == Vector3.zero)
        {
            //If no food is found, move in the random direction.
            hopDir = randomDir;
        }
        else if (foodDir.sqrMagnitude < 3)
        {
            //If close to food, hop towards the food.
            hopDir = foodDir;
        }
        else
        {
            //If not close enough to food, interpolate between random direction and food direction, based on hunger.
            hopDir = Vector3.Lerp(randomDir, foodDir.normalized, hunger * hunger);
        }

        return hopDir.normalized;
    }

    private Vector3 RandomDirection()
    {
        //Pick a random angle.
        float theta = Random.value * 2 * Mathf.PI;
        //Find the point on the unit circle correspoding to that angle.
        return new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));
    }

    //Checks if the slime can move in a certain direction.
    private bool CheckDirection(Vector3 direction)
    {
        Ray visionRay = new Ray(transform.position + Vector3.up, direction);
        return !Physics.Raycast(visionRay, visionDistance, environmentMask);
    }

    private Vector3 FindClosestFoodDirection()
    {
        Vector3 foodDir = Vector3.zero;
        float foodDist = float.MaxValue;

        for (int i = 0; i < Food.allFood.Count; i++)
        {
            //Linecast to the food, to check if there is a path to the food.
            if (!Physics.Linecast(transform.position, Food.allFood[i].transform.position, environmentMask))
            {
                float newFoodDist = Vector3.SqrMagnitude(Food.allFood[i].transform.position - transform.position);
                //Compare the distance of this food to the distance of the current closest food.
                if (newFoodDist < foodDist)
                {
                    foodDir = Food.allFood[i].transform.position - transform.position;
                    foodDist = newFoodDist;
                }
            }
        }

        return foodDir;
    }

    void Hop(Vector3 direction, float h, float v)
    {
        rb.AddForce(direction.normalized * h + Vector3.up * v, ForceMode.Impulse);
    }

    void DrawVisionRadius()
    {
        lr.positionCount = visionRadiusDisplayVertices;
        
        //Add visionRadiusDisplayVertices points, evenly spaced around a circle.
        for (int i = 0; i < visionRadiusDisplayVertices; i++)
        {
            float x = Mathf.Cos(i * 2 * Mathf.PI / visionRadiusDisplayVertices);
            float y = Mathf.Sin(i * 2 * Mathf.PI / visionRadiusDisplayVertices);
            Vector3 point = visionDistance * new Vector3(x, 0, y);
            lr.SetPosition(i, point);
        }
    }
}
