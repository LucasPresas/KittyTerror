# Contribuyendo a Kitty Terror

## Branches

| Branch | Quién | Propósito |
|--------|-------|-----------|
| `main` | Todos | Base estable. Solo se mergea acá tras integrar. |
| `integrationv2` | Lucas | Branch de integración activa. |
| `Luz,entorno-y-detalles.-final-Art` | Tomás | Assets 3D, materiales, iluminación. |
| `Hud` | Andrés | UI de inventario, HUD de vidas. |
| `main-menu` | Mora | Menú principal (ya mergeado en main). |

## Regla de oro

**SIEMPRE** crear la branch desde `main` actualizado:

```bash
git checkout main
git pull origin main
git checkout -b mi-branch
```

---

## Para Andrés — UI de Inventario + HUD

### Eventos disponibles en Inventory

Tu `UIInventory` debe escuchar estos eventos en vez de usar `Inventory.Instance`:

```csharp
// Encontrar el Inventory
Inventory inv = FindObjectOfType<Inventory>();

// Suscribirse
inv.OnItemAdded.AddListener(OnItemAdded);
inv.OnItemRemoved.AddListener(OnItemRemoved);
inv.OnItemSelected.AddListener(OnItemSelected);

// Callbacks
void OnItemAdded(string itemId) { /* actualizar UI */ }
void OnItemRemoved(string itemId) { /* actualizar UI */ }
void OnItemSelected(string itemId) { /* mostrar item en mano */ }
```

### ItemRegistry (crear)

Crear un ScriptableObject que mapee IDs de item → iconos:

```csharp
[CreateAssetMenu(fileName = "ItemRegistry", menuName = "KittyTerror/ItemRegistry")]
public class ItemRegistry : ScriptableObject
{
    public ItemIcon[] items;
}

[System.Serializable]
public class ItemIcon
{
    public string itemId;      // "Llave", "Hacha", "Botella"
    public Sprite icon;
    public string displayName;
}
```

Tu `UIInventory` usa este registro para buscar el icono cuando recibe `OnItemAdded("Llave")`.

### IDs de items existentes

| ID | Objeto |
|----|--------|
| `"Llave"` | Llave del cajón |
| `"Hacha"` | Hacha del mueble |
| `"Botella"` | Botella arrojadiza |

### HUD de vidas

El `PlayerDeathMonitor` expone un `UnityEvent` llamado `onPlayerDied`. Podés escucharlo desde tu HUD para mostrar pantalla de Game Over. También tiene referencia al `Player` para obtener las vidas actuales del `FirstPersonStateMachineController`.

---

## Para Mora — Game Over + Audio

### Game Over screen

El `PlayerDeathMonitor` en el MainCharacter tiene el evento `onPlayerDied`. Conectalo a tu Game Over screen desde el Inspector.

### Audio (Mora y Martina)

**Sistema de Event Bus** (a implementar):

```
Assets/Scripts/Events/
├── EventBus.cs
├── IEvent.cs
└── Events/
    ├── AudioPlayEvent.cs
    └── ...
```

Los scripts de gameplay ya están preparados para lanzar eventos de audio. Ejemplo:

```csharp
// Cuando se resuelve el candado
EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("padlock_open"));

// Cuando el gato ataca
EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("cat_attack"));
```

Vos creás un `AudioManager` que escuche estos eventos:

```csharp
void Start()
{
    EventBus<AudioPlayEvent>.OnRaised += HandleAudio;
}

void HandleAudio(AudioPlayEvent e)
{
    AudioClip clip = registry.GetClip(e.clipId);
    source.PlayOneShot(clip);
}
```

### Fases con audio pendiente

| Fase | Evento | clipId sugerido |
|------|--------|----------------|
| Candado abierto | `AudioPlayEvent("padlock_open")` | sonido de candado |
| Candado error | `AudioPlayEvent("padlock_wrong")` | sonido de error |
| Agarrar item | `AudioPlayEvent("item_pickup")` | sonido de recoger |
| Romper puerta | `AudioPlayEvent("door_break")` | sonido de puerta |
| Golpear puerta | `AudioPlayEvent("door_hit")` | sonido de golpe |
| Tirar botella | `AudioPlayEvent("bottle_throw")` | sonido de botella |
| Gato ahuyentado | `AudioPlayEvent("cat_flee")` | maullido |
| Gato ataca | `AudioPlayEvent("cat_attack")` | sonido de ataque |

---

## Para Tomás — Assets 3D

Ya tenés tus assets integrados en `main`. Para seguir trabajando:

```bash
git checkout main
git pull origin main
git checkout -b tus-cambios
```

Si querés actualizar tu branch anterior:

```bash
git checkout Luz,entorno-y-detalles.-final-Art
git rebase origin/main
git push origin Luz,entorno-y-detalles.-final-Art --force
```

### Assets pendientes

| Fase | Modelo |
|------|--------|
| Fase 2 | Candado 3D |
| Fase 4 | Botella 3D (ya existe .fbx, falta ajustar) |
| Fase 5 | Cuerpos en habitación |
| Fase 6 | Huellas de sangre (decals) |
| Fase 7 | Luces, post-processing final |

---

## Flujo de trabajo recomendado

```
main  ←── estable, solo mergea acá
  │
  ├── integrationv2  ←── Lucas prueba integraciones
  │
  ├── Hud            ←── Andrés: UI/HUD
  │
  ├── Luz,entorno... ←── Tomás: assets
  │
  └── [tu-branch]    ←── Mora/Martina: audio, UI
```

1. Cada uno trabaja en su branch
2. Cuando algo está listo, avisan a Lucas
3. Lucas lo pasa a `integrationv2`, prueba, y mergea a `main`

---

## Consejos para evitar romper cosas

1. **NUNCA** modifiques scripts que no son tuyos sin avisar
2. Si necesitás un cambio en un script de otro, hablalo antes
3. SIEMPRE hacé rebase sobre `main` antes de pushear
4. No subas archivos autogenerados (`.csproj`, `.sln`, `Library/`)
5. Usá `git status` antes de commitear para ver qué estás subiendo
