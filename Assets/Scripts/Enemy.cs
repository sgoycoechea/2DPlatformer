using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start() variables
    protected Rigidbody2D rb;
    protected Animator animator;
    protected AudioSource death;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        death = GetComponent<AudioSource>();
    }


    public virtual void HandleJumpedOn()
    {
        rb.velocity = new Vector2(0, 0);
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        death.Play();
        animator.SetTrigger("death");
    }

    protected virtual void Death()
    {
        Destroy(this.gameObject);
    }
}
