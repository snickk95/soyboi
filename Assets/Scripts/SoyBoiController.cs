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
    public bool isJumping;
    public float jumpSpeed = 8f;
    public float jumpDurationThreshold = 0.25f;
    public float airAccel = 3f;


    //private variables
    private SpriteRenderer sr;
    private Vector2 input;
    private Rigidbody2D rb;
    private Animator animator;
    private float rayCastLengthCheck = 0.005f;
    private float width;
    private float height;
    private float jumpDuration;

    void Awake()
    {
        //set variables to components on soy boi
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        height = GetComponent<Collider2D>().bounds.extents.x + 0.2f;
        width = GetComponent<Collider2D>().bounds.extents.x + 0.1f;
    }


    void Start()
    {
        
    }

    public bool PlayerIsGrounded()
    {
        bool groundCheck1 = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y - height), -Vector2.up, rayCastLengthCheck);

        bool groundCheck2 = Physics2D.Raycast(new Vector2(transform.position.x + (width - 0.2f), transform.position.y - height), Vector2.up, rayCastLengthCheck);

        bool groundCheck3 = Physics2D.Raycast(new Vector2(transform.position.x - (width - 0.2f), transform.position.y - height), Vector2.up, rayCastLengthCheck);

        if (groundCheck1||groundCheck2||groundCheck3)
        {
            return true;
        }
        else
        {
            return false;
        }
       
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

        if (input.y >= 1f)
        {
            jumpDuration += Time.deltaTime;

        }
        else
        {
            isJumping = false;
            jumpDuration = 0f;
        }

        if (PlayerIsGrounded()&& isJumping==false)
        {
            if (input.y>0f)
            {
                isJumping = true;
            }
        }
    }


    void FixedUpdate()
    {
        var acceleration = 0f;
        var xvelocity = 0f;

        if (PlayerIsGrounded())
        {
            acceleration = accel;
        }
        else
        {
            acceleration = airAccel;
        }

        if (PlayerIsGrounded() && input.x==0)
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

        if (isJumping && jumpDuration < jumpDurationThreshold)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }
}
