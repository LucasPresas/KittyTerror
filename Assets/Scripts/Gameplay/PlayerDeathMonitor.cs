using UnityEngine;
using UnityEngine.Events;
using KittyTerror.Gameplay;

public class PlayerDeathMonitor : MonoBehaviour
{
    [SerializeField] private FirstPersonStateMachineController player;
    [SerializeField] private UnityEvent onPlayerDied;

    private bool _wasDead;

    private void Update()
    {
        if (player == null || _wasDead) return;

        if (player.CurrentLives <= 0)
        {
            _wasDead = true;
            onPlayerDied?.Invoke();
        }
    }
}
