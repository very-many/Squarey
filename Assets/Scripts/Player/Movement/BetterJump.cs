using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class BetterJumping : NetworkBehaviour
{
    private Rigidbody2D rb;

    [Header("Input Action - Jump")]
    [SerializeField]
    private InputActionReference JumpAction;

    [Space]
    [Header("Stats")]
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;

    private bool offlineTest;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        offlineTest = GetComponent<PlayerMovementController>().offlineTest;
    }

    void Update()
    {
        if (!isOwned && !offlineTest)
            return;
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += (FallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
        else if (rb.linearVelocity.y > 0 && !JumpAction.action.IsPressed())
        {
            rb.linearVelocity += (LowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
    }
}
