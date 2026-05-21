using UnityEngine;

/// <summary>
/// Placed on each goal point in the scene.
/// Only detects the PLAYER car (which has a ReplayRecorder) — ghost cars are ignored automatically.
///
/// Unity Editor setup:
///   - Add a BoxCollider with "Is Trigger" enabled
///   - Set goalIndex to match the index of the associated SpawnPoint
/// </summary>
public class GoalTrigger : MonoBehaviour
{
    [Tooltip("Must match the SpawnPoint index of the expected car")]
    public int goalIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        // Only the player car has a ReplayRecorder → ghost cars are filtered out naturally
        ReplayRecorder recorder = other.GetComponent<ReplayRecorder>();
        CarController car       = other.GetComponent<CarController>();

        if (car != null && recorder != null)
        {
            GameManager.Instance.OnPlayerReachedGoal(car, recorder, goalIndex);
        }
    }
}
