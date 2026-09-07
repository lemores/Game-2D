using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    [SerializeField] private float jumpLenght = 2f;
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private LayerMask ground;
    private Collider2D coll;

    private bool facingLeft = true;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Transition jump to fall
        if (anim.GetBool("IsJumping"))
        {
            if(rb.velocity.y < .1)
            {
                anim.SetBool("IsJumping", false);
                anim.SetBool("IsFalling", true);
            }
        }

        //Transition fall to idle
        if (anim.GetBool("IsFalling") && coll.IsTouchingLayers(ground))
        {
            anim.SetBool("IsFalling", false);
        }
    }

    private void Move()
    {
        if (facingLeft)
        {
            //Test to see if frog is beyond the leftcap
            if (transform.position.x > leftCap)
            {
                //Make sure if frog is facing right location, and if it is not, then face the right direction
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1);
                }

                //Test to see if frog is on the ground to jump then
                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLenght, jumpHeight);
                    anim.SetBool("IsJumping", true);
                }
            }
            else
            {
                facingLeft = false;
            }
        }


        else
        {
            //Test to see if frog is beyond the leftcap
            if (transform.position.x < rightCap)
            {
                //Make sure if frog is facing left location, and if it is not, then face the left direction
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }

                //Test to see if frog is on the ground to jump then
                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLenght, jumpHeight);
                    anim.SetBool("IsJumping", true);
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }
}
