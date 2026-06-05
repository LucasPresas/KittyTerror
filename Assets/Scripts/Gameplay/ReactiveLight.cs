using System.Collections;
using UnityEngine;
using KittyTerror.Events;

namespace KittyTerror.Gameplay
{
    public class ReactiveLight : MonoBehaviour
    {
        [SerializeField] private Light[] targetLights;
        [SerializeField] private string listenThoughtId = "thought.padlock_interact";
        [SerializeField] private float duration = 4f;
        [SerializeField] private float activeIntensity = 2f;

        private float[] _originalIntensities;
        private bool[] _originallyEnabled;
        private Coroutine _timer;

        private void OnEnable()
        {
            EventBus<ThoughtEvent>.OnRaised += OnThought;
        }

        private void OnDisable()
        {
            EventBus<ThoughtEvent>.OnRaised -= OnThought;
        }

        private void OnThought(ThoughtEvent e)
        {
            if (e.ThoughtId != listenThoughtId) return;
            TriggerLights();
        }

        public void TriggerLights()
        {
            if (targetLights == null || targetLights.Length == 0) return;

            if (_timer != null) StopCoroutine(_timer);

            _originalIntensities = new float[targetLights.Length];
            _originallyEnabled = new bool[targetLights.Length];

            for (int i = 0; i < targetLights.Length; i++)
            {
                Light light = targetLights[i];
                if (light == null) continue;

                _originalIntensities[i] = light.intensity;
                _originallyEnabled[i] = light.enabled;

                light.enabled = true;
                light.intensity = activeIntensity;
            }

            _timer = StartCoroutine(ResetLights());
        }

        private IEnumerator ResetLights()
        {
            yield return new WaitForSeconds(duration);

            for (int i = 0; i < targetLights.Length; i++)
            {
                if (targetLights[i] == null) continue;
                targetLights[i].intensity = _originalIntensities[i];
                targetLights[i].enabled = _originallyEnabled[i];
            }
        }
    }
}
