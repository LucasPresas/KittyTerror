using System.Collections;
using UnityEngine;
using Cinemachine;

public class OpeningSequence : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera openingVcam;
    [SerializeField] private float duration = 4f;

    private GameObject _player;
    private MonoBehaviour[] _playerScripts;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
            _playerScripts = _player.GetComponents<MonoBehaviour>();

        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        if (openingVcam != null)
            openingVcam.Priority = 20;

        SetPlayerEnabled(false);

        yield return new WaitForSeconds(duration);

        if (openingVcam != null)
            openingVcam.Priority = 0;

        SetPlayerEnabled(true);

        Destroy(gameObject);
    }

    private void SetPlayerEnabled(bool enabled)
    {
        if (_playerScripts == null) return;
        foreach (MonoBehaviour script in _playerScripts)
        {
            if (script is FirstPersonStateMachineController || script is PlayerInteraction)
                script.enabled = enabled;
        }
    }
}
