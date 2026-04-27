using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirectStaff : MonoBehaviour
{
    public Camera camera;
    public GameObject player;
    public bool onlinePlayer = false;
    public float intensity = 1f;
    public float speed = 0.5f;


    void Start()
    {
        //TODO set onlinePlayer true if the staff belongs to an online Player

        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!onlinePlayer)
        {
            FollowMouse();
        }
        else
        {
            //sync it online
        }
    }

    private void FollowMouse()
    {
        /* Get the mouse position in world spaaace rather than screen space. */
        var mouseWorldCoord = camera.ScreenPointToRay(Mouse.current.position.ReadValue()).origin;

        Vector2 offset = new Vector2(mouseWorldCoord.x - this.transform.position.x, mouseWorldCoord.y - this.transform.position.y);

        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        angle = (angle -  120) % 360;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        

        /* Get a vector pointing from initialPosition to the target. Vector shouldn't be longer than maxDistance. */
        var originToMouse = mouseWorldCoord - player.transform.position;
        originToMouse = Vector3.ClampMagnitude(originToMouse, intensity);

        /* Linearly interpolate from current position to mouse's position. */
        this.transform.position = Vector3.Lerp(player.transform.position, player.transform.position + originToMouse, speed);
    }
}
