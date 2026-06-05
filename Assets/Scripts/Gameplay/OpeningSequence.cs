using System.Collections;
using UnityEngine;

public class OpeningSequence : MonoBehaviour
{
    [SerializeField] private float duration = 4f;
    [SerializeField] private float swayAngle = 15f;

    private Behaviour[] _playerScripts;
    private Transform _camPivot;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerScripts = player.GetComponents<Behaviour>();
            _camPivot = player.transform.Find("CameraPivot");
        }

        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        SetPlayerEnabled(false);

        if (_camPivot == null) yield break;

        Quaternion startRot = _camPivot.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float sway = Mathf.Sin(t * Mathf.PI * 2f) * swayAngle;
            _camPivot.localRotation = startRot * Quaternion.Euler(10f, sway, 5f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _camPivot.localRotation = startRot;
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
