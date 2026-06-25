using SmallHedge.SoundManager;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Space]

    [Header("Collision")]

    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private static readonly Color DebugNoCollisionColor = Color.green;
    private static readonly Color DebugCollisionColor = Color.red;

    // cached components / state
    private Rigidbody2D rb;
    private bool lastOnGround;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastOnGround = false;
    }

    void Update()
    {
        // preserve previous ground state
        bool wasOnGround = onGround;

        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        wallSide = onRightWall ? -1 : 1;

        // Landing: transition from not-on-ground -> on-ground
        if (!wasOnGround && onGround)
        {
            SoundManager.PlaySound(SoundType.Land);
        }

        // Jumping: transition from on-ground -> not-on-ground
        // Only play if the player is moving upward (to avoid playing when walking off edges).
        if (wasOnGround && !onGround)
        {
            if (rb != null && rb.linearVelocity.y > 0.01f)
            {
                SoundManager.PlaySound(SoundType.Jump);
            }
        }

        lastOnGround = onGround;
    }

    void OnDrawGizmos()
    {
        bool bottomHit;
        bool rightHit;
        bool leftHit;

        if (Application.isPlaying)
        {
            bottomHit = onGround;
            rightHit = onRightWall;
            leftHit = onLeftWall;
        }
        else
        {
            bottomHit = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
            rightHit = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
            leftHit = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        }

        Gizmos.color = bottomHit ? DebugCollisionColor : DebugNoCollisionColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);

        Gizmos.color = rightHit ? DebugCollisionColor : DebugNoCollisionColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);

        Gizmos.color = leftHit ? DebugCollisionColor : DebugNoCollisionColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }
}
