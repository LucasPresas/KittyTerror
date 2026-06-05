using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;
using KittyTerror.Events;

public class NumberPadlock : MonoBehaviour
{
    [Header("Código")]
    [SerializeField] private string correctCode = "314";

    [Header("Puerta")]
    [SerializeField] private GameObject doorToDestroy;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField inputField;

    [Header("Detección")]
    [SerializeField] private float interactDistance = 4f;

    private bool _solved;
    private Camera _cam;

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _cam = player.GetComponentInChildren<Camera>();
        inputField.onSubmit.AddListener(OnSubmit);
    }

    private void Update()
    {
        if (_solved || _cam == null) return;

        float dist = Vector3.Distance(transform.position, _cam.transform.position);

        if (dist <= interactDistance)
        {
            if (!panel.activeSelf && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                panel.SetActive(true);
                inputField.text = "";
                inputField.ActivateInputField();
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                EventBus<ThoughtEvent>.Raise(new ThoughtEvent("thought.padlock_interact"));
            }
        }
        else
        {
            if (panel.activeSelf)
                panel.SetActive(false);
        }
    }

    private void OnSubmit(string text)
    {
        if (panel.activeSelf) CheckCode(text);
    }

    private void CheckCode(string input)
    {
        if (input == correctCode)
        {
            _solved = true;
            panel.SetActive(false);
            EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("padlock_open"));
            if (doorToDestroy != null)
                Destroy(doorToDestroy);
            enabled = false;
        }
        else
        {
            EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("padlock_wrong"));
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }
}
