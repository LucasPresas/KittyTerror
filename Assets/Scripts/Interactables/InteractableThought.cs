using UnityEngine;
using KittyTerror.Events;

public class InteractableThought : MonoBehaviour, IInteractable
{
    [SerializeField] private string thoughtId;
    [SerializeField] private string interactText = "Observar";

    public string GetInteractText() => interactText;

    public void Interact()
    {
        EventBus<ThoughtEvent>.Raise(new ThoughtEvent(thoughtId));
    }
}
