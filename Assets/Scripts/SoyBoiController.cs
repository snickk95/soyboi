using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//validates that the object has the components required to work
[RequireComponent(typeof(SpriteRenderer),typeof(Rigidbody2D),typeof(Animator))]

public class SoyBoiController : MonoBehaviour
{
    //public variables
    public float speed = 14f;
    public float accel = 6f;


    //private variables
    private SpriteRenderer sr;
    private Vector2 input;
    private Rigidbody2D rb;
    private Animator animator;

     void Awake()
    {
        //set variables to components on soy boi
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }


    void Start()
    {
        
    }

    
    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Jump");

        //flip the sprite on the x axis if the x input is greater than 0 
        if(input.x > 0f)
        {
            sr.flipX = false;
        }
        else if(input.x < 0f)
        {
            sr.flipX = true;
        }
    }

    void FixedUpdate()
    {
        var acceleration = accel;
        var xvelocity = 0f;

        if (input.x==0)
        {
            xvelocity = 0f;
        }
        else
        {
            xvelocity = rb.velocity.x;
        }
        //force is added to rigidbody calculating the horizontal controls multiplied by the speed then multiply by acceleration
        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x) * acceleration, 0));
        //stops the soy boi from moving in a nutral position
        rb.velocity = new Vector2(xvelocity, rb.velocity.y);
    }
}
