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

    // Use rectangles (boxes) instead of circles. Each box can be scaled independently (width, height).
    public Vector2 bottomSize = new Vector2(0.5f, 0.25f);
    public Vector2 rightSize = new Vector2(0.25f, 0.5f);
    public Vector2 leftSize = new Vector2(0.25f, 0.5f);

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

        onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, bottomSize, 0f, groundLayer);
        onWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, rightSize, 0f, groundLayer)
            || Physics2D.OverlapBox((Vector2)transform.position + leftOffset, leftSize, 0f, groundLayer);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, rightSize, 0f, groundLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, leftSize, 0f, groundLayer);

        wallSide = onRightWall ? -1 : 1;

        // Landing: transition from not-on-ground -> on-ground
        /*if (!wasOnGround && onGround)
        {
            SoundManager.PlaySound(SoundType.Land);
        }*/

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
            bottomHit = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, bottomSize, 0f, groundLayer);
            rightHit = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, rightSize, 0f, groundLayer);
            leftHit = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, leftSize, 0f, groundLayer);
        }

        Gizmos.color = bottomHit ? DebugCollisionColor : DebugNoCollisionColor;
        Gizmos.DrawWireCube(new Vector3(transform.position.x + bottomOffset.x, transform.position.y + bottomOffset.y, transform.position.z), new Vector3(bottomSize.x, bottomSize.y, 1f));

        Gizmos.color = rightHit ? DebugCollisionColor : DebugNoCollisionColor;
        Gizmos.DrawWireCube(new Vector3(transform.position.x + rightOffset.x, transform.position.y + rightOffset.y, transform.position.z), new Vector3(rightSize.x, rightSize.y, 1f));

        Gizmos.color = leftHit ? DebugCollisionColor : DebugNoCollisionColor;
        Gizmos.DrawWireCube(new Vector3(transform.position.x + leftOffset.x, transform.position.y + leftOffset.y, transform.position.z), new Vector3(leftSize.x, leftSize.y, 1f));
    }
}
