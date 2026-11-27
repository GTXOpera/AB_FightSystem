using System;
using System.Data.Common;
using NUnit.Framework;
using UnityEditor.Animations;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{


    public enum STATES
    {
        Idle,
        Walk,
        NormalAttack,
        Block,
        Crouch,
        HitStun,
        CommandAttack,
        Air
    }

    public enum ANIM
    {
        Idle,
        Walk,
        Crouch,
        Jump,
        Block,
        Jab,
        Kick
    }

    public STATES PlayerState;
    public ANIM AnimState;

    private Rigidbody2D rb;
    private BoxCollider2D pyr;
    private SpriteRenderer cl;
    private PlayerInput playerinput;
    private Vector2 walkInput;
    private Animator animator;

    public float JUMP = 680f;
    private float SPEED = 8f;
    public bool ground;
    public bool rival;
    public bool otherPlayer;
    private bool crouched;
    public bool invert = false;
    string stateName;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cl = GetComponent<SpriteRenderer>();
        playerinput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        if (invert)
        {
            SPEED = -8;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        GroundCheck();
        walkInput = playerinput.actions["Move"].ReadValue<Vector2>();
        UpdateState();

        stateName = AnimState.ToString();

        // Debug.Log(rb.linearVelocity);
        // Debug.Log(PlayerState);
    }

    private void FixedUpdate()
    {

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jab") || animator.GetCurrentAnimatorStateInfo(0).IsName("Kick"))
        {
            playerinput.actions.Disable();
        }
        else
        {
            playerinput.actions.Enable();
        }


        switch (PlayerState)
        {
            case STATES.Idle:
                walk();
                jump();
                NormalAttack();
                cl.color = Color.white;
                animator.SetBool("Moving", false);
                animator.SetBool("zkey", false);
                animator.SetBool("xkey", false);

                break;
            case STATES.Walk:
                walk();
                jump();
                NormalAttack();
                cl.color = Color.green; //verde
                break;
            case STATES.NormalAttack:
                NormalAttack();
                // playerinput.actions.Disable();
                // animator.SetBool("zkey", true);
                break;
            case STATES.Crouch:
                crouch();
                NormalAttack();
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                break;
            case STATES.HitStun:
                break;
            case STATES.CommandAttack:
                break;
            case STATES.Block:
                blocking();
                crouch();
                cl.color = Color.gold; //amarillo
                break;
            case STATES.Air:
                NormalAttack();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
                cl.color = Color.blue; //azul
                break;
        } 
    }

    private void UpdateState()
    {

        switch (PlayerState)
        {
            case STATES.Idle:


                if (Mathf.Abs(walkInput.x) > 0.1f)
                {
                    PlayerState = STATES.Walk;
                }

                if (!ground)
                {
                    PlayerState = STATES.Air;
                }

                if (playerinput.actions["Crouch"].IsPressed() && ground)
                {
                    PlayerState = STATES.Crouch;
                }
                if (playerinput.actions["Block"].IsPressed() && ground)
                {
                    PlayerState = STATES.Block;
                }

                // Para pasar   1                                                                                                                                                                                                                                                                            al estado de movimientos como comandos. Apena funciona, pero lo hace, no se como
                if (playerinput.actions["Punch"].WasPerformedThisFrame())
                {
                    PlayerState = STATES.NormalAttack;

                    if (Keyboard.current.xKey.IsPressed() && playerinput.actions["Command"].WasPressedThisFrame())
                    {
                        PlayerState = STATES.CommandAttack;
                    }
                }

                break;
            case STATES.Walk:
                if (!ground)
                {
                    PlayerState = STATES.Air;
                }

                if (Mathf.Abs(walkInput.x) < 0.1f)
                {
                    PlayerState = STATES.Idle;
                }

                if (playerinput.actions["Crouch"].IsPressed() && ground)
                {
                    PlayerState = STATES.Crouch;
                }
                if (playerinput.actions["Block"].IsPressed() && ground)
                {
                    PlayerState = STATES.Block;
                }
                // if (playerinput.actions["Punch"].WasPressedThisFrame())
                // {
                //     PlayerState = STATES.NormalAttack;
                // }

                break;
            case STATES.NormalAttack:

                if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Jab") || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Kick"))
                {
                    PlayerState = STATES.Idle;
                }


                break;
            case STATES.Crouch:
                if (playerinput.actions["Block"].WasReleasedThisFrame() || !crouched)
                {
                    PlayerState = STATES.Idle;
                }
                if (playerinput.actions["Block"].IsPressed() && crouched)
                {
                    PlayerState = STATES.Block;
                }
                // if (playerinput.actions["Punch"].WasPressedThisFrame())
                // {
                //     PlayerState = STATES.NormalAttack;
                // }

                break;
            case STATES.HitStun:
                break;
            case STATES.CommandAttack:
                break;
            case STATES.Block:
                if (playerinput.actions["Block"].WasReleasedThisFrame())
                {
                    PlayerState = STATES.Idle;
                }
                else if (playerinput.actions["Block"].WasReleasedThisFrame() && crouched)
                {
                    PlayerState = STATES.Crouch;
                }
                break;
            case STATES.Air:
                if (ground)
                {
                    PlayerState = STATES.Idle;
                }
                // if (playerinput.actions["Punch"].WasPressedThisFrame())
                // {
                //     PlayerState = STATES.NormalAttack;
                // }

                break;
        }
    }
    public void jump()
    {
        if (playerinput.actions["Jump"].IsPressed())
        {
            rb.AddForce(Vector2.up * JUMP);
        }
    }

    public void walk()
    {
        rb.linearVelocity = new Vector2(walkInput.x * SPEED, rb.linearVelocity.y);
    }

    public void crouch()
    {
        if (playerinput.actions["Crouch"].IsPressed() && ground)
        {
            playerinput.actions["Jump"].Disable();
            rb.transform.localScale = new Vector2(0.8f, 0.4f);

            crouched = true;
        }

        else
        {
            playerinput.actions["Jump"].Enable();
            rb.transform.localScale = new Vector2(0.8f, 0.8f);
            crouched = false;
        }
    }

    public void blocking()
    {
        if (playerinput.actions["Block"].IsPressed() && ground)
        {
            playerinput.actions["Jump"].Disable();
        }
        else
        {
            playerinput.actions["Jump"].Enable();
        }
    }

    public void NormalAttack()
    {
        var punchInput = playerinput.actions["Punch"];
        if (punchInput.activeControl == Keyboard.current.zKey)
        {
            animator.SetBool("zkey", true);
        }
        else if (punchInput.activeControl == Keyboard.current.xKey)
        {
            animator.SetBool("xkey", true);
        }

    }

    public void SpecialAttack()
    {
        var command = playerinput.actions["Command"];
        var punchInput = playerinput.actions["Punch"];

        if (command.IsPressed())
        {
            Console.Write("Comando");
        }
    }

    private void GroundCheck()
    {
        float raylength = 3.2f;

        Ray ray = new Ray(transform.position, Vector2.down);
        Debug.DrawRay(ray.origin, ray.direction * raylength, Color.green);

        ground = Physics2D.Raycast(ray.origin, ray.direction, raylength);

    }

    private void RivalCheck()
    {

    }

}
