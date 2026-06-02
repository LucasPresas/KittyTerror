# Instrucciones para Mora — Sistema de Audio

## ¿Qué está listo?

Ya implementé el **Event Bus** (`EventBus<T>`) y el evento de audio (`AudioPlayEvent`). 
Los scripts de gameplay ya lanzan eventos cuando pasa algo. 
Solo falta que vos crees el **AudioManager** que los escuche y reproduzca los sonidos.

## Archivos creados

```
Assets/Scripts/Events/
├── IEvent.cs              → Interface que marca un evento
├── EventBus.cs            → Bus genérico: Raise() / OnRaised
├── INSTRUCCIONES_AUDIO.md → Este archivo
└── Events/
    └── AudioPlayEvent.cs  → Evento con un string ClipId
```

## Cómo crear el AudioManager

Hacé un script `AudioManager.cs` en `Assets/Scripts/` (o donde quieras) que:

```csharp
using UnityEngine;
using KittyTerror.Events;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;

    private void OnEnable()
    {
        EventBus<AudioPlayEvent>.OnRaised += PlaySound;
    }

    private void OnDisable()
    {
        EventBus<AudioPlayEvent>.OnRaised -= PlaySound;
    }

    private void PlaySound(AudioPlayEvent e)
    {
        // Buscar el clip según e.ClipId y reproducirlo
        Debug.Log($"[AudioManager] Reproducir: {e.ClipId}");
        // ej: sfxSource.PlayOneShot(clip);
    }
}
```

Luego arrastrá el `AudioManager` a cualquier GameObject en la escena (o creá uno nuevo llamado "AudioManager").

## ClipIds que ya se están emitiendo

| Script | clipId | Cuándo |
|--------|--------|--------|
| `NumberPadlock.cs` | `padlock_open` | Código correcto |
| `NumberPadlock.cs` | `padlock_wrong` | Código incorrecto |
| `ItemPickup.cs` | `item_pickup` | Agarrar item suelto |
| `Drawer.cs` | `item_pickup` | Abrir cajón |
| `Toolbox.cs` | `item_pickup` | Abrir mueble |
| `LockedDoor.cs` | `door_hit` | Golpear puerta |
| `LockedDoor.cs` | `door_break` | Puerta destruida |
| `BottleThrow.cs` | `bottle_throw` | Lanzar botella |
| `CatStateMachineController` | `cat_attack` | Gato ataca |
| `CatStateMachineController` | `cat_flee` | Gato huye |
| `FirstPersonStateMachineController` | `player_hit` | Jugador recibe daño |
| `ItemEquip.cs` | `item_equip` | Seleccionar item con 1/2/3 |
| `ClockPuzzles.cs` | `clock_tick_hour` | Girar manecilla de horas |
| `ClockPuzzles.cs` | `clock_tick_minute` | Girar manecilla de minutos |
| `ClockPuzzles.cs` | `puzzle_solved` | Reloj resuelto correctamente |
| `MenuManager.cs` | `click_button` | Click en botón del menú |
| `UIButtonSound.cs` | `click_button` | Click en botón del menú |
| `MenuManager.cs` | `hover_button` | Hover sobre botón del menú |
| `UIButtonSound.cs` | `hover_button` | Hover sobre botón del menú |

## (Opcional) AudioClipRegistry

Si querés hacerlo más prolijo, podés crear un ScriptableObject que mapee clipId → AudioClip:

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipRegistry", menuName = "KittyTerror/AudioClipRegistry")]
public class AudioClipRegistry : ScriptableObject
{
    public AudioEntry[] entries;

    public AudioClip GetClip(string clipId)
    {
        foreach (var e in entries)
            if (e.clipId == clipId) return e.clip;
        return null;
    }
}

[System.Serializable]
public class AudioEntry
{
    public string clipId;
    public AudioClip clip;
}
```

Después creás el Asset desde el menú Assets → Create → KittyTerror → AudioClipRegistry, cargás los clips, y referenciás el registry desde tu `AudioManager`.

## Resumen para implementar

1. Crear `AudioManager.cs` con el listener de `EventBus<AudioPlayEvent>`
2. Arrastrarlo a la escena
3. Asignar un `AudioSource` en el Inspector
4. (Opcional) Crear `AudioClipRegistry` para organizar los clips

Cualquier duda preguntame!
