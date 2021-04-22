using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Could also be declared as "public Rigidbody2D rb", but that is a bad practice unless it is needed in other classes
    // If we do that, we can set its value through Unity's Inspector
    // We can add the attribute [SerializeField] to a private variable for it to be seen in Unity's Inspector

    // Start() variables
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D coll;

    // Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 6.5f;
    [SerializeField] private int points = 0;
    [SerializeField] private float hurtForce = 5f;
    [SerializeField] private Text pointsText;

    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource cherry;
    [SerializeField] private AudioSource gem;


    // FSM
    private enum State { idle, running, jumping, falling, hurt }
    private State state = State.idle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.hurt) ManageMovement();
        SetState();
    }

    // This is an override
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Gem")
        {
            Destroy(collision.gameObject);
            gem.Play();
            points += 5;
        }

        if (collision.tag == "Cherry")
        {
            Destroy(collision.gameObject);
            cherry.Play();
            points += 1;
        }

        pointsText.text = points.ToString();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (state == State.falling)
            {
                enemy.HandleJumpedOn();
                Jump();
            }
            else
            {
                state = State.hurt;
                bool isEnemyToTheRight = other.gameObject.transform.position.x > transform.position.x;
                rb.velocity = new Vector2(isEnemyToTheRight ? -hurtForce : hurtForce, rb.velocity.y);
            }
        }
    }


    private void ManageMovement()
    {
        // To check for a specific key we can use Input.GetKey(KeyCode.A) or Input.GetKeyDown(KeyCode.A), but it's a bad practice
        float hDirection = Input.GetAxis("Horizontal");
        if (hDirection != 0)
        {
            rb.velocity = new Vector2(hDirection > 0 ? speed : -speed, rb.velocity.y);
            transform.localScale = new Vector2(hDirection > 0 ? 1 : -1, transform.localScale.y);
        }

        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground) && state != State.hurt)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void SetState()
    {
        switch (state)
        {
            case State.jumping:
                if (rb.velocity.y < .1f) state = State.falling;
                break;
            case State.running:
                if (rb.velocity.y < -.1f) state = State.falling;
                else if (Mathf.Abs(rb.velocity.x) < 2f) state = State.idle;
                break;
            case State.idle:
                if (rb.velocity.y < -.1f) state = State.falling;
                else if (Mathf.Abs(rb.velocity.x) > 2f) state = State.running;
                break;
            case State.falling:
                if (coll.IsTouchingLayers(ground)) state = State.idle;
                break;
            case State.hurt:
                if (Mathf.Abs(rb.velocity.x) < .1f) state = State.idle;
                break;
        }
        
        animator.SetInteger("state", (int)state);
    }

    private void Footstep()
    {
        footstep.Play();
    }
}
