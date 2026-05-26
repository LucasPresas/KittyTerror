using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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

    private void Update()
    {
        if (_solved) return;

        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);

        if (dist <= interactDistance)
        {
            if (!panel.activeSelf && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                panel.SetActive(true);
                inputField.text = "";
                inputField.ActivateInputField();
            }
        }
        else
        {
            if (panel.activeSelf)
                panel.SetActive(false);
        }

        if (panel.activeSelf && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            CheckCode(inputField.text);
    }

    private void CheckCode(string input)
    {
        if (input == correctCode)
        {
            _solved = true;
            panel.SetActive(false);
            if (doorToDestroy != null)
                Destroy(doorToDestroy);
            enabled = false;
        }
        else
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }
}
