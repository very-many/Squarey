using Mirror;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DirectStaff : NetworkBehaviour
{
    public GameObject player;
    public GameObject staff;
    public float maxDistance = 0.7f; //how far can the tip of the staff get from the player?
    public float minDistance = 0.1f; // how close can the tip of the staff get to the player?
    public float speed = 0.5f; // how fast the staff moves to the target position
    public float staffWeight = 5f; // how much further the mouse has to be for the staff to be at max distance from the player
    private Quaternion quaternionAngle;
    public Quaternion castAngle;

    public Vector2 playerPosition;
    private Vector2 staffPosition;
    public Vector2 castPosition;
    public Vector2 castDirection;
    public Vector2 castTarget;

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        this.staff = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerPosition = player.transform.position;
        staffPosition = staff.transform.position;
        castPosition = staffPosition;
        castTarget = mouseWorldPos;

        Vector2 offsetToStaff = mouseWorldPos - staffPosition;

        Vector2 offsetToPlayer = mouseWorldPos - playerPosition;

        //Rotation StaffObject
        float objectAngle = Mathf.Atan2(offsetToPlayer.y, offsetToPlayer.x) * Mathf.Rad2Deg;
        objectAngle = (objectAngle - 120) % 360;
        quaternionAngle = Quaternion.Euler(0, 0, objectAngle);
        transform.rotation = quaternionAngle;

        //Bewegung StaffObject
        Vector2 targetStaffPosition = Vector2.ClampMagnitude(offsetToPlayer/staffWeight, maxDistance) + playerPosition;

        Vector2 staffMovementToNextFrame = Vector2.Lerp(staffPosition, targetStaffPosition, speed) - staffPosition;

        Vector2 staffPositionNextFrame = Vector2.ClampMagnitude(staffMovementToNextFrame + staffPosition - playerPosition, maxDistance) + playerPosition;
        
        staffMovementToNextFrame = staffPositionNextFrame - staffPosition;

        staffPositionNextFrame = ClampMinMagnitude(staffMovementToNextFrame + staffPosition - playerPosition, minDistance) + playerPosition;
        
        transform.position = staffPositionNextFrame;

        //Rotation CastAngle
        float aimAngle = Mathf.Atan2(offsetToPlayer.y, offsetToPlayer.x) * Mathf.Rad2Deg;
        castAngle = Quaternion.Euler(0, 0, aimAngle);

        //CastDirection
        castDirection = offsetToStaff.normalized;
    }

    public Vector2 ClampMinMagnitude(Vector2 vector, float min)
    {
        if (vector.magnitude < min)
        {
            return vector.normalized * min;
        }
        return vector;
    }
}
