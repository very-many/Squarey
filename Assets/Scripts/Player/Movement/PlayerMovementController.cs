using Mirror;
using Mirror.Examples.BilliardsPredicted;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
    public GameObject PlayerVisuals;

    [Space]
    [Header("Stats")]
    public float speed = 7;
    public float jumpForce = 12;
    public float slideSpeed = 1;
    public float wallJumpLerp = 5;
    public float cyoteTime = 0.05f;

    private float lastCyoteTime;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallJumped;
    public bool wallSlide;
    public bool spellJumped;

    public int side = 1;

    public float floatyTimeInSec = 0;

    [Space]
    [Header("Input")]
    public Vector2 movement;

    [Space]
    [Header("Staff")]
    MultiStaffObject staffMulti;
    DirectStaff directStaff;

    [Space]
    [Header("Animation Parameters")]
    public Animator animator;
    private readonly int speedXHash = Animator.StringToHash("speedX");
    private readonly int speedYHash = Animator.StringToHash("speedY");
    private readonly int isJumpingHash = Animator.StringToHash("isJumping");
    private readonly int isFallingHash = Animator.StringToHash("isFalling");
    private readonly int isWallHoldHash = Animator.StringToHash("isWallHold");
    private readonly int isFacingLeftHash = Animator.StringToHash("isFacingLeft");
    private bool prevFacingLeft = false;
    private bool isGrounded = false;

    public Vector2 knockback = new Vector2(0, 0);
    public bool RequestTeleport { get; set; } = true;


    void Start()
    {
        PlayerVisuals = transform.GetChild(0).gameObject;
        coll = GetComponent<PlayerCollisionController>();
        rb = GetComponent<Rigidbody2D>();
        PlayerVisuals.SetActive(false);
    }

    void Update()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Game"))
            return;

        if(GameOrchestrator.Instance == null)
        {
            PlayerVisuals.SetActive(true);
        }


        if (isOwned && RequestTeleport)
        {
            if (!NetworkClient.isConnected || !NetworkClient.ready)
                return;

            Debug.Log("Requesting teleport");
            RequestTeleport = false;

            Vector2 pos = new Vector2(Random.Range(-7, 7), 10);
            CmdClientChosenTeleport(pos);
        }

        if (isOwned)
        {
            Tick();
        }
    }

    private void FixedUpdate()
    {
        knockback = knockback * 0.8f;
    }

    [Command]
    public void CmdClientChosenTeleport(Vector2 pos)
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
        if (knockback.magnitude < 1)
        {
            knockback = new Vector2(0,0);
        }

        float x = movement.x;
        Vector2 dir = new Vector2(x, movement.y);

        Walk(dir);

        if (coll.onGround)
        {
            wallJumped = false;
            spellJumped = false;

            isGrounded = true;
            lastCyoteTime = Time.time;
        }
        else
        {
            if (Time.time > lastCyoteTime + cyoteTime)
            {
                isGrounded = false;
            }
        }
                 
        if (coll.onWall && !coll.onGround && x != 0 && x != 0)
        {
            wallSlide = true;
            WallSlide();
            lastCyoteTime = Time.time;
        }
        else
        {
            if (Time.time > lastCyoteTime + cyoteTime)
            {
                wallSlide = false;
            }
        }

        if (x > 0)
            side = 1;
        else if (x < 0)
            side = -1;

        UpdateAnimationParameters();
    }

    private void UpdateAnimationParameters()
    {
        if (animator == null)
            return;

        float absXvelocity = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat(speedXHash, absXvelocity);
        animator.SetFloat(speedYHash, rb.linearVelocity.y);

        bool facingLeft = rb.linearVelocity.x < -0.01f;
        // only update if the abs velocity is large enough, to prevent feault right
        if (facingLeft != prevFacingLeft && absXvelocity > 0.01f)
        {
            animator.SetBool(isFacingLeftHash, facingLeft);
            prevFacingLeft = facingLeft;
        }

        bool isJumping = rb.linearVelocity.y > 0;
        animator.SetBool(isJumpingHash, isJumping);

        bool isFalling = rb.linearVelocity.y < 0 && !coll.onGround;
        animator.SetBool(isFallingHash, isFalling);

        bool isWallHold = coll.onWall && !coll.onGround;
        animator.SetBool(isWallHoldHash, isWallHold);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {

        if (!context.performed || !isOwned)
            return;

        if (coll.onGround || isGrounded)
            Jump(Vector2.up);
        else if (coll.onWall || wallSlide)
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
            rb.linearVelocity = new Vector2(dir.x * speed + knockback.x, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (new Vector2(dir.x * speed + knockback.x, rb.linearVelocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir)
    {
        spellJumped = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * jumpForce;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    public void SpellJump(Vector2 dir)
    {
        spellJumped = false;
        floatyTimeInSec = 1;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * 12;
    }

    public void Knockback(Vector2 dir)
    {
        rb.AddForce(dir);
        knockback = dir;
        Debug.Log("Knockback: " + dir);
    }
}
