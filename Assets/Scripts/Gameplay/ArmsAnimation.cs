using UnityEngine;

public class ArmsAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        InteractionAnimationEvent.OnInteract += PlayAnimation;
    }

    private void OnDisable()
    {
        InteractionAnimationEvent.OnInteract -= PlayAnimation;
    }

    private void PlayAnimation()
    {
        animator.SetTrigger("Interact");
    }
}