using UnityEngine;

public class InteractionPromptHandler : MonoBehaviour
{
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private float rayDistance = 4f;
    [SerializeField] private LayerMask rayLayer = ~0;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null) _camera = Camera.main;
    }

    void Update()
    {
        if (_camera == null) return;

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        RaycastHit hit;
        bool found = Physics.Raycast(ray, out hit, rayDistance, rayLayer);

        IInteractable interactable = null;
        if (found && hit.collider.GetComponentInParent<BottleProjectile>() == null)
        {
            interactable = hit.collider.GetComponentInParent<IInteractable>();
        }

        if (interactable is LockedDoor)
            interactable = null;

        if (promptRoot != null)
            promptRoot.SetActive(interactable != null);
    }
}
