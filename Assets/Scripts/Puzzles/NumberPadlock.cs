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
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            onPlayerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            onPlayerExit?.Invoke();
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
