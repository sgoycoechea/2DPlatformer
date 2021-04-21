using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : Enemy
{

    // Start() variables
    private Collider2D coll;
    private bool jumpingRight = true;

    // Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private int secondsBetweenJumps;

    // FSM
    private enum State { idle, jumping, falling }
    private State state = State.idle;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.jumping:
                if (rb.velocity.y < .1f) state = State.falling;
                break;
            case State.idle:
                break;
            case State.falling:
                if (coll.IsTouchingLayers(ground))
                {
                    state = State.idle;
                    rb.velocity = Vector2.zero;
                }
                break;
        }

        animator.SetInteger("state", (int)state);
    }

    private void Move()
    {
        state = State.jumping;
        if (transform.position.x < leftCap) jumpingRight = true;
        if (transform.position.x > rightCap) jumpingRight = false;
        rb.velocity = new Vector2(jumpingRight ? horizontalSpeed : -horizontalSpeed, verticalSpeed);
        transform.localScale = new Vector2(jumpingRight ? -1 : 1, transform.localScale.y);
    }
}
