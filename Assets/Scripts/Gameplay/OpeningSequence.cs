using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class OpeningSequence : MonoBehaviour
{
    [SerializeField] private float duration = 4f;
    [SerializeField] private float maxAmplitude = 3f;

    private CinemachineCamera _cmCam;
    private CinemachineBasicMultiChannelPerlin _noise;
    private Behaviour[] _playerScripts;

    private void Start()
    {
        _cmCam = FindObjectOfType<CinemachineCamera>();
        if (_cmCam == null)
        {
            Debug.LogError("[OpeningSequence] No se encontró CinemachineCamera en la escena.");
            return;
        }

        _noise = _cmCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (_noise == null)
        {
            _noise = _cmCam.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
            _noise.NoiseProfile = CreateNoiseProfile();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerScripts = player.GetComponents<Behaviour>();
            Transform pivot = player.transform.Find("CameraPivot");
            if (pivot != null)
                _cmCam.Follow = pivot;
        }

        _noise.AmplitudeGain = maxAmplitude;
        StartCoroutine(Sequence());
    }

    private NoiseSettings CreateNoiseProfile()
    {
        var profile = ScriptableObject.CreateInstance<NoiseSettings>();
        profile.OrientationNoise = new NoiseSettings.TransformNoiseParams[]
        {
            new NoiseSettings.TransformNoiseParams
            {
                X = new NoiseSettings.NoiseParams { Frequency = 1.5f, Amplitude = 1f },
                Y = new NoiseSettings.NoiseParams { Frequency = 1.2f, Amplitude = 1f },
                Z = new NoiseSettings.NoiseParams { Frequency = 0.8f, Amplitude = 0.8f }
            }
        };
        return profile;
    }

    private IEnumerator Sequence()
    {
        SetPlayerEnabled(false);

        _cmCam.Priority = 100;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            _noise.AmplitudeGain = Mathf.Lerp(maxAmplitude, 0f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _noise.AmplitudeGain = 0f;
        _cmCam.Priority = 0;
        SetPlayerEnabled(true);

        Destroy(gameObject);
    }

    private void SetPlayerEnabled(bool enabled)
    {
        if (_playerScripts == null) return;
        foreach (Behaviour script in _playerScripts)
        {
            string name = script.GetType().Name;
            if (name == "FirstPersonStateMachineController" || name == "PlayerInteraction")
                script.enabled = enabled;
        }
    }
}
