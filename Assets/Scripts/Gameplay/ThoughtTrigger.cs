using UnityEngine;
using KittyTerror.Events;

public class ThoughtTrigger : MonoBehaviour
{
    [SerializeField] private string thoughtId;
    [SerializeField] private bool oneShot = true;

    private bool _used;

    private void OnTriggerEnter(Collider other)
    {
        if (_used && oneShot) return;
        if (!other.CompareTag("Player")) return;

        _used = true;
        EventBus<ThoughtEvent>.Raise(new ThoughtEvent(thoughtId));
    }
}
