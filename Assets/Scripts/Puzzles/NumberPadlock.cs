using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class NumberPadlock : MonoBehaviour
{
    [SerializeField] private string correctCode = "314";
    [SerializeField] private GameObject doorToDestroy;

    public UnityEvent onCorrect;
    public UnityEvent onIncorrect;
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        Debug.Log($"[NumberPadlock] Awake - trigger set, code={correctCode}, door={doorToDestroy?.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[NumberPadlock] OnTriggerEnter: {other.name}, tag={other.tag}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("[NumberPadlock] Player detected!");
            onPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[NumberPadlock] OnTriggerExit: {other.name}");
        if (other.CompareTag("Player"))
            onPlayerExit?.Invoke();
    }

    public void CheckCode(string input)
    {
        Debug.Log($"[NumberPadlock] CheckCode: input='{input}', correct='{correctCode}', match={input == correctCode}");
        if (input == correctCode)
        {
            Debug.Log("[NumberPadlock] Correct! Destroying door.");
            onCorrect?.Invoke();
            if (doorToDestroy != null)
                Destroy(doorToDestroy);
            enabled = false;
        }
        else
        {
            Debug.Log("[NumberPadlock] Incorrect!");
            onIncorrect?.Invoke();
        }
    }

    public void CheckCode(string input)
    {
        if (input == correctCode)
        {
            onCorrect?.Invoke();
            if (doorToDestroy != null)
                Destroy(doorToDestroy);
            enabled = false;
        }
        else
        {
            onIncorrect?.Invoke();
        }
    }
}
