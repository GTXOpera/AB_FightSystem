using System;
using NUnit.Framework;
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

    public STATES PlayerState;

    private Rigidbody2D rb;
    private SpriteRenderer cl;
    private PlayerInput playerinput;
    private Vector2 walkInput;

    private float JUMP = 680f;
    private float SPEED = 8f;
    public bool ground;
    public bool otherPlayer;
    private bool crouched;
    public bool invert = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cl = GetComponent<SpriteRenderer>();
        playerinput = GetComponent<PlayerInput>();


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

        // Debug.Log(rb.linearVelocity);
    }

    private void FixedUpdate()
    {
        switch (PlayerState)
        {
            case STATES.Idle:
                walk();
                jump();
                NormalAttack();
                cl.color = Color.gray2; //gris
                break;
            case STATES.Walk:
                walk();
                jump();
                NormalAttack();
                cl.color = Color.green; //verde
                break;
            case STATES.NormalAttack:
                NormalAttack();
                walk();
                break;
            case STATES.Crouch:
                crouch();
                NormalAttack();
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                cl.material.color = Color.violet; // morado
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
                // if (playerinput.actions["Punch"].WasPressedThisFrame())
                // {
                //     PlayerState = STATES.NormalAttack;
                // }

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
            rb.transform.localScale = new Vector2(1f, 0.5f);
            crouched = true;
        }

        else
        {
            playerinput.actions["Jump"].Enable();
            rb.transform.localScale = new Vector2(1f, 1f);
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
            print("Puñetazo");
            cl.color = Color.darkRed;
        }
        else if (punchInput.activeControl == Keyboard.current.xKey)
        {
            print("Patada");
            cl.color = Color.softRed;
        }

    }
    private void GroundCheck()
    {
        float raylength = 1.1f;

        Ray ray = new Ray(transform.position, Vector2.down);
        Debug.DrawRay(ray.origin, ray.direction * raylength, Color.green);

        ground = Physics2D.Raycast(ray.origin, ray.direction, raylength);
    }


}
