using UnityEngine;

namespace KittyTerror.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class CatAttackTrigger : MonoBehaviour
    {
        [SerializeField] private CatStateMachineController cat;
        [SerializeField] private bool oneShot = true;
        [SerializeField] private bool requirePlayerTag = true;

        private bool _alreadyTriggered;

        private void Reset()
        {
            Collider triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_alreadyTriggered && oneShot) return;
            if (cat == null) return;

            if (requirePlayerTag && !other.CompareTag("Player"))
            {
                return;
            }

            cat.TriggerExternalAttack();

            if (oneShot)
            {
                _alreadyTriggered = true;
            }
        }
    }
}
