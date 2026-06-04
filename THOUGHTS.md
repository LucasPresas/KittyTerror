# Sistema de Pensamientos (Thought System)

## Arquitectura

```
ThoughtEvent ──→ EventBus ──→ ThoughtDisplay (muestra texto en UI)
      ↑
      │
ThoughtRegistry.asset (mapea thoughtId → texto)
      ↑
      │
ThoughtTrigger (collider) / InteractableThought (E key)
Scripts de gameplay (ItemPickup, LockedDoor, etc.)
```

## IDs y textos

| ID | Texto | Gatillo |
|----|-------|---------|
| `thought.awake` | "Qué dolor de cabeza, qué pasó acá?" | Al arrancar el juego (PlayerDeathMonitor.Start) |
| `thought.first_blood` | "Eso es sangre? Kitty estás bien?" | Al salir de la 1er habitación (ThoughtTrigger) |
| `thought.cat_hit` | "Kitty!! Qué haces? Gato de m****" | Golpe del gato (PlayerDeathMonitor) |
| `thought.family_photo` | "Mi familia...dónde estarán?" | E en el cuadro (InteractableThought) |
| `thought.clock` | "Este reloj...otra vez se salió de hora" | E en el reloj (PlayerInteraction) |
| `thought.cabinet_locked` | "Está cerrado..." | E en armario sin llave (Toolbox) |
| `thought.four_fingers` | "Una mano...de 4 dedos?" | Zona mancha (InteractableThought) |
| `thought.unbelieveable` | "No puedo creer esto, qué habrá pasado?" | Zona cuerpos (InteractableThought) |
| `thought.key_get` | "Mmm qué abrirá esta llave?" | Agarrar llave (Drawer/ItemPickup) |
| `thought.door_break` | "Kitty...otra vez?" | Romper puerta (LockedDoor) |
| `thought.bottle_throw` | "Jiuura gato" | Tirar botella (BottleThrow) |
| `thought.padlock_interact` | "El candado está cerrado...cuál será la combinación correcta? Deben haber pistas en esta habitación" | E en candado |
| `thought.final` | "NOOO QUÉ PASÓ ACÁ? MI FAMILIA" | Zona final (ThoughtTrigger) |

## Setup en escena (para Mora/Andrés)

### 1. ThoughtDisplay (texto en pantalla)
Crear un GameObject "ThoughtDisplay" en la escena con:
- Componente `TextMeshProUGUI` (como hijo de un Canvas)
- Componente `ThoughtDisplay.cs`
- Arrastrar `ThoughtRegistry.asset` al campo `registry`

### 2. ThoughtTriggers (colliders)
Para pensamientos por zona (first_blood, four_fingers, unbelieveable, final):
- Crear un GameObject vacío con un `BoxCollider` en `isTrigger`
- Agregar `ThoughtTrigger.cs`
- Escribir el `thoughtId` correspondiente

### 3. InteractableThought (objetos con E)
Para objetos sin lógica propia (family_photo, four_fingers, unbelieveable):
- Agregar `InteractableThought.cs` al GameObject
- Escribir `thoughtId` y texto de interacción ("Observar")

## Scripts modificados

Los siguientes scripts ahora emiten `ThoughtEvent` automáticamente:

| Script | thoughtId | Cuándo |
|--------|-----------|--------|
| `PlayerDeathMonitor.cs` | `thought.awake` | Al cargar la escena |
| `PlayerDeathMonitor.cs` | `thought.cat_hit` | Al recibir daño del gato |
| `PlayerInteraction.cs` | `thought.clock` | Al interactuar con el reloj (E) |
| `Drawer.cs` | `thought.key_get` | Al abrir el cajón (obtener llave) |
| `ItemPickup.cs` | `thought.key_get` | Al agarrar la llave del suelo |
| `Toolbox.cs` | `thought.cabinet_locked` | Al intentar abrir sin llave |
| `LockedDoor.cs` | `thought.door_break` | Al romper la puerta |
| `BottleThrow.cs` | `thought.bottle_throw` | Al lanzar una botella |

## Crear el ThoughtRegistry.asset (si falta)

En Unity: Assets → Create → KittyTerror → ThoughtRegistry.
Luego asignarle los mismos textos que están en la lista de arriba.
