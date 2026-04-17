using UnityEngine;

namespace KittyTerror.Gameplay
{
    public class CatAttackInvoker : MonoBehaviour
    {
        [SerializeField] private CatStateMachineController cat;

        public void TriggerCatAttack()
        {
            if (cat == null)
            {
                return;
            }

            cat.TriggerExternalAttack();
        }
    }
}
