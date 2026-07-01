using Mirror;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!NetworkServer.active) 
            return;
        if (GameOrchestrator.Instance.CurrentGameState != GameOrchestrator.GameState.Game) 
            return;

        // Only react to player: Physics Collision Matrix ignores Bullet <> KillZone collisions, so we don't need to check for bullets here

        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            health.Kill();
        }
    }
}