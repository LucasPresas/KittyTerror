using UnityEngine;
using UnityEngine.Events;
using KittyTerror.Gameplay;
using KittyTerror.Events;

public class PlayerDeathMonitor : MonoBehaviour
{
    [SerializeField] private FirstPersonStateMachineController player;
    [SerializeField] private UnityEvent onPlayerDied;

    private bool _wasDead;
    private float _previousLives;

    private void Start()
    {
        _previousLives = player != null ? player.CurrentLives : 0;
        EventBus<ThoughtEvent>.Raise(new ThoughtEvent("thought.awake"));
    }

    private void Update()
    {
        if (player == null || _wasDead) return;

        if (player.CurrentLives < _previousLives)
        {
            _previousLives = player.CurrentLives;
            EventBus<ThoughtEvent>.Raise(new ThoughtEvent("thought.cat_hit"));
        }

        if (player.CurrentLives <= 0)
        {
            _wasDead = true;
            EventBus<GameOverEvent>.Raise(new GameOverEvent("Sin vidas"));
            EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("game_over"));
            onPlayerDied?.Invoke();
        }
    }
}
