using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public Transform player;
    public Transform respawnPoint;
    private Rigidbody2D rb;


    public TextMesh lifeText;
    public TextMesh cherryText;
    public Text timeText;
    public GameObject character;
    public GameObject gameOverMenu;
    public GameObject blurBlackground;
    [SerializeField] AudioSource footstep;
    [SerializeField] AudioSource cherry;


    public float runSpeed = 40f;
    private float hurtForce = 20f;
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool hurted = false;


    private int cherryCount = 0;
    private int lifeCount = 3;
    private float gameplayElapsed;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lifeText.text = lifeCount.ToString();
       
    }

    // Update is called once per frame
    void Update()
    {
   
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        GameOver();
        CountingTime();
    }


    void FixedUpdate()
    {
            //Move character
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;
            animator.SetBool("IsHurted", false);
     
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }


    //In contact with something
    private void OnCollisionEnter2D(Collision2D other)
    {

        //In contact with enemy
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            //Kill enemy if falling
            if(rb.velocity.y < .1)
            {
                enemy.JumpedOn();
            }
            else
            {
                hurted = true;
                lifeCount -= 1;
                lifeText.text = lifeCount.ToString();

                if (other.gameObject.transform.position.x > player.transform.position.x)
                {
                    //Enemy is to my right therefore i should be damaged and move left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                    animator.SetBool("IsHurted", true);
                    hurted = false;
                }
                else
                {
                    //Enemy is to my left therefore i should be damaged and move right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                    animator.SetBool("IsHurted", true);
                    hurted = false;
                }
            }
        }   
    }

    //In contact with Triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Cherry
        if (collision.tag == "Collectable")
        {
            Destroy(collision.gameObject); //Or collision.gameObject.SetActive(false);
            cherryCount += 1;
            cherry.Play();

            //Counting Cherries
            cherryText.text = cherryCount.ToString();
        }

        //KillFloor
        if (collision.tag == "Kill")
        { 
            //Respawning
            player.transform.position = respawnPoint.transform.position;

            //LostingLives
            lifeCount -= 1;
            lifeText.text = lifeCount.ToString();
            
        }
    }


    //Counting TIME
    void CountingTime()
    {
        gameplayElapsed += Time.deltaTime;
        timeText.text = "Time: " + Math.Round(gameplayElapsed).ToString()+"s";
    }

    //Setting GAMEOVER
    private void GameOver()
    {
        if (lifeCount < 0)
        {
            lifeText.text = "";
            gameOverMenu.SetActive(true);
            blurBlackground.SetActive(true);
            gameplayElapsed -= Time.deltaTime;
            character.SetActive(false);
        }
    }

    public void Footstep()
    {
        footstep.Play();
    }



    //TODO make character jump when hit enemy
    //TODO make falling animation on animator
    //TODO change to state system
    //TODO Find a way to know that character is jumping when hitting enemy
    //TODO Lock controll when hurted 
    //TODO Lock camera when GameOver
}
