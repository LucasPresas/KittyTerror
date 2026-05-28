# PLAN.md — Kitty Terror, Segundo Parcial

## Leyenda
- [ ] Pendiente
- [x] Completado
- (L) Lucas — programación
- (M) Mora — UI escenas + audio
- (Ma) Martina — audio + UI escenas
- (T) Tomas — assets 3D
- (A) Andres — UI pantalla (vidas, inventario, HUD)

---

## Fase 0: Infraestructura

- [ ] **Event Bus** (L) — `EventBus<T>`, `IEvent`, eventos base
- [ ] **Game Over screen** (M) — UI de derrota, escucha `PlayerDiedEvent`
- [ ] **HUD vidas** (A) — mostrar vidas restantes, escucha `PlayerDamagedEvent`
- [ ] **AudioManager** (M/Ma) — escucha `AudioPlayEvent`, reproduce sonidos
- [ ] **AudioClipRegistry** (M/Ma) — ScriptableObject con mapeo clipId → AudioClip

---

## Fase 1: Primer Puzzle — El Reloj  ✅

- [x] Reloj interactuable (E = minutos, F = horas)
- [x] Gato patrulla y ataca al acercarse
- [x] Gato resta 1 vida por ataque
- [x] Al perder 3 vidas → se dispara `onPlayerDied`
- [x] Al poner el reloj en 3 en punto → gato se destruye
- [x] PlayerDeathMonitor funcional

---

## Fase 2: Candado de 3 Números  ✅

- [x] `NumberPadlock.cs` — interacción con E, input field, Enter confirma, destruye puerta
- [x] Puerta placeholder + script asignado + panel/input conectados
- [x] Pistas visuales: nro de depto + mancha de 4 dedos
- [ ] Sonidos: candado abierto / incorrecto (pendiente)
- [ ] Modelo 3D del candado (pendiente)

---

## ▶️ Fase 3: Living — Puerta con Hacha  ✅

- [x] `Inventory.cs` — sistema básico con eventos OnItemAdded/Removed/Selected
- [x] `ItemPickup.cs` — recolectar items
- [x] `Drawer.cs` — cajón con llave
- [x] `Toolbox.cs` — mueble con hacha (requiere llave)
- [x] `LockedDoor.cs` — puerta de 3 golpes con cambio de sprite
- [x] `PlayerInteraction.cs` — soporte IInteractable genérico
- [x] `ItemEquip.cs` — mostrar/ocultar items en ArmsHolder
- [x] Assets de Tomás integrados (materiales, modelos, luces, entorno)
- [ ] Animación de brazo al golpear (posterior)
- [ ] UI de inventario (Andres)
- [ ] Modelos 3D finales (Tomas)
- [ ] Sonidos y SFX (M/Ma)

---

## ▶️ Fase 4: Pasillo — El Gato  <-- SIGUIENTE

- [ ] `Bottle.cs` — item arrojadizo (recogible en la escena)
- [ ] Gato se ahuyenta al recibir botella (nuevo estado "Flee" o destroy)
- [ ] Sonido: botella romperse, gato ahuyentado (M/Ma)
- [ ] Modelo botella (T)

---

## Fase 5: Habitación — Escena de Shock

- [ ] `ShockTrigger.cs` — al entrar: lock movimiento + cámara shake + sonido
- [ ] `CameraShake.cs` — shake procedural leve
- [ ] Evento `PlayerShockEvent` para Mora/Martina
- [ ] Efecto visual de pantalla (vignette, color tint, etc.)
- [ ] Sonido de shock + silencio posterior
- [ ] Modelos: cuerpos en la habitación (T)

---

## Fase 6: Huellas de Sangre

- [ ] `BloodTrail.cs` — waypoints invertidos
- [ ] Decals/Billboards que siguen al jugador
- [ ] Textura/decals de huellas de sangre (T)

---

## Fase 7: Pulido y Cierre

- [ ] Radio inicial con locución (trigger al empezar)
- [ ] Sistema de pistas falsas vs correctas
- [x] Menú principal (Mora)
- [ ] Música/ambiente general (M/Ma)
- [ ] UI final pulida (vidas, inventario, interacciones) (A)
- [ ] Assets finales, luces, post-processing (T)
- [ ] Testing general y ajustes
