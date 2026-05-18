using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCollisionController))]
[DefaultExecutionOrder(1)]
public class PlayerMovementController : NetworkBehaviour
{
    private PlayerCollisionController coll;
    [HideInInspector]
    public Rigidbody2D rb;

    [Space]
    [Header("Child Player Object")]
    public GameObject PlayerObject;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallJumped;
    public bool wallSlide;

    public int side = 1;

    [Space]
    [Header("Input")]
    public Vector2 movement;

    private bool requestedTeleport = false;


    void Start()
    {
        coll = GetComponent<PlayerCollisionController>();
        rb = GetComponent<Rigidbody2D>();
        PlayerObject.SetActive(false);
    }

    void Update()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Game"))
            return;

        PlayerObject.SetActive(true);

        if (isOwned && !requestedTeleport)
        {
            if (!NetworkClient.isConnected || !NetworkClient.ready)
                return;

            Debug.Log("Requesting teleport");
            requestedTeleport = true;

            Vector2 pos = new Vector2(Random.Range(-7, 7), 10);
            CmdClientChosenTeleport(pos);
        }

        if (isOwned)
        {
            Tick();
        }
    }

    [Command]
    private void CmdClientChosenTeleport(Vector2 pos)
    {
        transform.position = pos;
        rb.linearVelocity = Vector2.zero;
        rb.position = pos;

        RpcApplyTeleport(pos);
    }

    [ClientRpc]
    private void RpcApplyTeleport(Vector2 pos)
    {
        transform.position = pos;
        rb.linearVelocity = Vector2.zero;
        rb.position = pos;
    }

    private void Tick()
    {
        float x = movement.x;
        Vector2 dir = new Vector2(x, movement.y);

        Walk(dir);

        if (coll.onGround)
        {
            wallJumped = false;
        }

        if (coll.onWall && !coll.onGround && x != 0)
        {
            wallSlide = true;
            WallSlide();
        }
        else
        {
            wallSlide = false;
        }

        if (x > 0)
            side = 1;
        else if (x < 0)
            side = -1;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (coll.onGround)
            Jump(Vector2.up);
        else if (coll.onWall)
            WallJump();
    }

    private void WallJump()
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        side = coll.onRightWall ? -1 : 1;
        Jump((Vector2.up + wallDir) / 1.5f);

        wallJumped = true;
    }

    private void WallSlide()
    {
        if (!canMove)
            return;

        bool pushingWall = false;
        if ((rb.linearVelocity.x > 0 && coll.onRightWall) || (rb.linearVelocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.linearVelocity.x;

        rb.linearVelocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (!wallJumped)
        {
            rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (new Vector2(dir.x * speed, rb.linearVelocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * jumpForce;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}
