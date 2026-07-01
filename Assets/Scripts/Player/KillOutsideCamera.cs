using Mirror;
using UnityEngine;

public class KillOutsideCamera : NetworkBehaviour
{
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
    }

    void Update()
    {

        bool outOfWorld = transform.position.y < -100f;
       
        if (outOfWorld)
        {
            health.TakeDamage(health.maxHealth); // Instantly kill the player
        }
    }
}