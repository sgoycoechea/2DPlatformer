using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleController : Enemy
{

    // Start() variables
    private Collider2D coll;

    // Inspector variables
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    [SerializeField] private float speed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
        rb.velocity = new Vector2(-speed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < leftCap)
        {
            rb.velocity = new Vector2(speed, 0);
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if (transform.position.x > rightCap)
        {
            rb.velocity = new Vector2(-speed, 0);
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }
}
