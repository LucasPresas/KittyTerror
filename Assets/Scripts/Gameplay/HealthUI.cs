using UnityEngine;
using UnityEngine.UI;
using KittyTerror.Gameplay;

public class HealthUI : MonoBehaviour
{
    public Slider healthSlider;

    private FirstPersonStateMachineController player;

    void Start()
    {
        player = FindObjectOfType<FirstPersonStateMachineController>();

        if (player != null)
        {
            healthSlider.maxValue = player.MaxLives;
            healthSlider.value = player.CurrentLives;
        }
    }

    void Update()
    {
        if (player == null) return;

        healthSlider.value = player.CurrentLives;
    }
}