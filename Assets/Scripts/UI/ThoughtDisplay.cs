using UnityEngine;
using TMPro;
using KittyTerror.Events;
using System.Collections;

public class ThoughtDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI thoughtText;
    [SerializeField] private float displayDuration = 3.5f;
    [SerializeField] private ThoughtRegistry registry;

    private Coroutine _hideCoroutine;

    private void OnEnable()
    {
        EventBus<ThoughtEvent>.OnRaised += ShowThought;
    }

    private void OnDisable()
    {
        EventBus<ThoughtEvent>.OnRaised -= ShowThought;
    }

    private void ShowThought(ThoughtEvent e)
    {
        string text = registry != null ? registry.GetThought(e.ThoughtId) : e.ThoughtId;
        if (string.IsNullOrEmpty(text)) return;

        thoughtText.text = text;
        thoughtText.gameObject.SetActive(true);

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        thoughtText.gameObject.SetActive(false);
    }
}
