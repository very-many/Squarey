using UnityEngine;

[RequireComponent(typeof(PlayerCollisionController))]
public class PlayerAnimationController : MonoBehaviour
{
    [Space]
    [Header("Animation Parameters")]
    public Animator capeAnimator;

    private readonly int animSpeedXHas = Animator.StringToHash("speedX");
    private readonly int animSpeedYHash = Animator.StringToHash("speedY");
    private readonly int animIsWallHoldHash = Animator.StringToHash("isWallHold");
    private readonly int animIsFacingLeftHash = Animator.StringToHash("isFacingLeft");
    private readonly int animOnGroundHash = Animator.StringToHash("onGround");
    private bool prevFacingLeft = false;
    private Vector3 previousPosition;

    private PlayerCollisionController collisionController;

    void Start()
    {
        collisionController = GetComponent<PlayerCollisionController>();
    }

    void Update()
    {
        if (capeAnimator == null)
            return;

        Vector3 estimatedVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        float absXVelocity = Mathf.Abs(estimatedVelocity.x);
        capeAnimator.SetFloat(animSpeedXHas, absXVelocity);
        capeAnimator.SetFloat(animSpeedYHash, estimatedVelocity.y);

        bool isFacingLeft = estimatedVelocity.x < -0.01f || collisionController.onLeftWall;
        // only update if the abs velocity is large enough, to prevent feault right
        if (isFacingLeft != prevFacingLeft && absXVelocity > 0.01f)
        {
            capeAnimator.SetBool(animIsFacingLeftHash, isFacingLeft);
            prevFacingLeft = isFacingLeft;
        }

        bool isWallHold = collisionController.onWall && !collisionController.onGround;
        capeAnimator.SetBool(animIsWallHoldHash, isWallHold);

        capeAnimator.SetBool(animOnGroundHash, collisionController.onGround);
    }
}
