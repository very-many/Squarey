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

    void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        wallSide = onRightWall ? -1 : 1;
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
