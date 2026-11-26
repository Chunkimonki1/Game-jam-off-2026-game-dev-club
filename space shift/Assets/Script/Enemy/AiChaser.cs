using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;

public class AiChaser : MonoBehaviour
{
    public GameObject player;
    public float speed;
    private float distance; 
    [SerializeField]
    public float reach; // Distance at which the enemy starts chasing 
    [SerializeField]
    private LayerMask obstacleLayer; //Assign your wall layer here in the inspector 
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Awake()
    {
        //Get the RigidBody2D component automatically
        rb = GetComponent<Rigidbody2D>();
        
        
    }

    // Update is called once per frame
    void Update()//Makes enemy rotate towards player
    {
        if(player == null)
        {
            return;
        }
        //it checks the enemy's distance from the player this is always running and is essential to create a directional arrow (vector)
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;
        direction.Normalize();
        //Check if player is within reach AND there is a clear line of sight (no walls in between)
        if(distance < reach && HasLineOfSight(direction))
        {
            //The angle is calculated in radians and we're converting it into degrees
            //its easier to just memorize it tbh
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(Vector3.forward * angle); 
        }
        
    }
    void FixedUpdate() //Makes enemy move towards player uses engine physics aka rigidbody2D that why we use FixedUpdate
    {
        if (player == null)
        {
            return;
        }
        if (distance < reach && HasLineOfSight(((Vector2)player.transform.position - (Vector2)transform.position).normalized))
        {
            Vector2 targetPosition = Vector2.MoveTowards(rb.position, player.transform.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(targetPosition);
        }
    }


    bool HasLineOfSight(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer);
        // VISUALIZATION: Choose color based on whether a wall blocks the ray
        // Red = Wall is blocking view | Green = Clear line of sight to player
        Color rayColor = (hit.collider != null) ? Color.red : Color.green;
        Debug.DrawRay(transform.position, direction * distance, rayColor);

        return hit.collider == null;

    }
}
