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

    // Player configuration
    [SerializeField] private float runSpeed = 7f;                               // Speed of the player.
    [SerializeField] private float jumpForce = 14f;                             // Amount of force added when the player jumps.
    [SerializeField] private bool airControl = true;							// Whether or not a player can steer while jumping.
    [SerializeField] private float hurtForce = 5f;                              // Amount of force added when the player is hurt.
    [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;            // Amount of maxSpeed applied to crouching movement.
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = 0f;      // How much to smooth out the movement
    [SerializeField] private Collider2D crouchDisableCollider;	     			// A collider that will be disabled when crouching

    // These need to be serialized to be set
    [SerializeField] private LayerMask ground;                                  // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheckBox;                          // A position marking where to check if the player is grounded.
    [SerializeField] private Transform ceilingCheckBox;                         // A position marking where to check for ceilings.
    [SerializeField] private Text pointsText;                                   // Text with the current point count.

    // Audio
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource cherry;
    [SerializeField] private AudioSource gem;

    // Other
    int points = 0;
    bool facingRight = true;
    bool isCrouching = false;
    bool isHurting = false;
    bool isGrounded = false;
    Vector3 velocity = Vector3.zero;
    const float ceilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (!isHurting) ManageMovement();
        CalculateState();
    }

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
            if (IsFalling())
            {
                enemy.HandleJumpedOn();
                Jump();
            }
            else
            {
                isHurting = true; 
                bool isEnemyToTheRight = other.gameObject.transform.position.x > transform.position.x;
                rb.velocity = new Vector2(isEnemyToTheRight ? -hurtForce : hurtForce, rb.velocity.y);
            }
        }
    }


    private void ManageMovement()
    {
        isCrouching = Input.GetButton("Crouch") && isGrounded && !isHurting;
        if (!isCrouching && Physics2D.OverlapCircle(ceilingCheckBox.position, ceilingRadius, ground)) // If the character has a ceiling preventing them from standing up, keep them crouching
            isCrouching = true;

        float horizontalMove = Input.GetAxis("Horizontal");

        if (canMovePlayer() && horizontalMove != 0)
        {
            float move = horizontalMove * runSpeed;

            if (isCrouching)
            {
                move *= crouchSpeed;
                if (crouchDisableCollider != null) crouchDisableCollider.enabled = false; // Disable one of the colliders when crouching
            }
            else
            {
                if (crouchDisableCollider != null) crouchDisableCollider.enabled = true; // Enable the collider when not crouching
            }


            //rb.velocity = new Vector2(move, rb.velocity.y);
            Vector3 targetVelocity = new Vector2(move, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

            if (ShouldFlipSprite(move))
                FlipSprite();
        }

        if (Input.GetButtonDown("Jump") && canJump())
            Jump();
    }

    private bool ShouldFlipSprite(float move)
    {
        return (move > 0 && !facingRight) || (move < 0 && facingRight);
    }

    private void FlipSprite()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    private bool canMovePlayer()
    {
        return !isHurting && (isGrounded || airControl);
    }

    private bool canJump()
    {
        return isPlayerOnTheGround() && !isHurting;
    }

    private bool isPlayerOnTheGround()
    {
        return coll.IsTouchingLayers(ground);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private bool IsFalling()
    {
        return !isGrounded && !isHurting && rb.velocity.y < 1f;
    }

    private void CalculateState()
    {
        if (isHurting && Mathf.Abs(rb.velocity.x) < .1f) isHurting = false;
        isGrounded = isPlayerOnTheGround();

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsHurt", isHurting);
        animator.SetFloat("VerticalSpeed", rb.velocity.y);
        animator.SetFloat("VerticalSpeedAbs", Mathf.Abs(rb.velocity.y));
        animator.SetFloat("HorizontalSpeedAbs", Mathf.Abs(rb.velocity.x));
    }

    private void Footstep()
    {
        footstep.Play();
    }
}
