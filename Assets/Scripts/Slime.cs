﻿using System.Collections;
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

        if(Random.value < moveChance / tickRate && IsGrounded)
        {
            Vector3 hopDir = GetMoveDirection();
            Hop(hopDir, horizontalForce, verticalForce);
        }
    }

    private Vector3 GetMoveDirection()
    {
        bool randomDirValid = false;
        Vector3 randomDir = Vector3.zero;

        while (!randomDirValid)
        {
            randomDir = RandomDirection();

            randomDirValid = CheckDirection(randomDir);
            Debug.DrawRay(transform.position + Vector3.up, randomDir * visionDistance, randomDirValid ? Color.green : Color.red, 1f);
        }

        Vector3 foodDir = FindClosestFoodDirection();

        Vector3 hopDir = Vector3.zero;

        if(foodDir == Vector3.zero)
        {
            hopDir = randomDir;
        }
        else if (foodDir.sqrMagnitude < 3)
        {
            hopDir = foodDir;
        }
        else
        {
            hopDir = Vector3.Lerp(randomDir, foodDir.normalized, hunger * hunger);
        }

        return hopDir.normalized;
    }

    private Vector3 RandomDirection()
    {
        float theta = Random.value * 2 * Mathf.PI;
        return new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));
    }

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
            if (!Physics.Linecast(transform.position, Food.allFood[i].transform.position, environmentMask))
            {
                float newFoodDist = Vector3.SqrMagnitude(Food.allFood[i].transform.position - transform.position);

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

        for (int i = 0; i < visionRadiusDisplayVertices; i++)
        {
            Vector3 point = visionDistance * new Vector3(Mathf.Cos(i * 2 * Mathf.PI / visionRadiusDisplayVertices), 0, Mathf.Sin(i * 2 * Mathf.PI / visionRadiusDisplayVertices));
            lr.SetPosition(i, point);
        }
    }
}