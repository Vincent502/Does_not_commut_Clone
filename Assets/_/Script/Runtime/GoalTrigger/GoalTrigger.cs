using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    #region Public

    #endregion
    //
    //
    #region Unity API

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;
        // Filtre : uniquement la voiture avec CarMovement (prefab joueur)
        CarMovement car = other.GetComponent<CarMovement>();
        if (car == null) return;
        // Optionnel : ignorer si la course n’a pas commencé
        if (!car._isRacing) return;
        _hasTriggered = true;
        Debug.Log($"[Goal] Arrivée au goal {_goalIndex} !");
        if (GameManager.Instance != null)
            GameManager.Instance.OnReachPoint();
    }

    public void ResetTrigger() => _hasTriggered = false;

    #endregion
    // space between region = 2
    //
    #region Main API

    #endregion
    //
    // 
    #region Private & Protected

    [SerializeField] private int _goalIndex = 0;
    private bool _hasTriggered;

    #endregion
}
