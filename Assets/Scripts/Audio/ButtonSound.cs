using KittyTerror.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
{
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ClickOnButton);
    }

    private void ClickOnButton()
    {
        Debug.Log("[PauseMenu] Botón clickeado, enviar evento click_button");
        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("click_button"));
    }

    public void HoverOnButton()
    {
        Debug.Log("[PauseMenu] Hover activado, enviar evento hover_button");
        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("hover_button"));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverOnButton();
    }

    public void OnSelect(BaseEventData eventData)
    {
        HoverOnButton();
    }

}
