# Checklist — Criterios de Evaluación

## 1. Construcción de Horror, Ambientación e Inmersión
- [x] Escena oscura con niebla (Fog density 0.03)
- [x] URP con post-processing (Bloom, Vignette)
- [x] Marcas de sangre, huellas, foto familiar
- [x] ThoughtSystem narrativo (13 pensamientos)
- [x] Sonidos de terror (gritos, gato, puertas)
- [ ] Sin sonidos ambientales reproduciéndose (archivos sin usar)
- [ ] Sin luces parpadeantes
- [ ] Sin CameraShake ni ShockTrigger
- [ ] Post-processing en valores por defecto (vignette baja, sin color grading)

## 2. Organización del Proyecto (GitHub + Carpetas)
- [x] Repositorio Git funcional con commits convencionales
- [x] Documentación: PLAN.md, THOUGHTS.md, AGENTS.md, INSTRUCCIONES_AUDIO.md
- [x] Scripts organizados por dominio (Audio, Events, Gameplay, UI, etc.)
- [x] Settings de URP bien configurados
- [ ] Espacios en nombres de archivos (30+)
- [ ] Assets duplicados (Cat.prefab, texturas)
- [ ] Namespaces inconsistentes (solo 11/30 scripts con namespace)
- [ ] `Assets/Assets/` — carpeta confusa

## 3. Diseño del Escenario y Nivel
- [x] Escenario completo con geometría ProBuilder (paredes, pisos, techos)
- [x] Colliders dedicados (~20 BoxColliders)
- [x] 21 modelos 3D (muebles, items, decoración)
- [x] Iluminación: Directional + 14 luces + fog + lightmaps horneados
- [x] Prefabs para la mayoría de props
- [ ] NavMesh sin hornear (configurado pero vacío)
- [ ] Varios prefabs sin colliders (mesa, silla, armario, etc.)
- [ ] Sin documentación de diseño de nivel (layout, plano)

## 4. Path Route y Jugabilidad
- [x] Secuencia completa: Despertar → Llave → Candado → Hacha → Puerta → Reloj → Gato
- [x] Puzzles funcionales (candado 314, reloj 3:00)
- [x] Cadena de items (Llave → Hacha, Botella opcional)
- [x] Inventario con hotbar (slots 1/2/3)
- [x] Sistema de muerte y GameOver funcional
- [ ] ❌ **CRÍTICO: No hay condición de victoria** — el juego no tiene final
- [ ] ❌ **CRÍTICO: No hay pantalla de victoria, ni créditos, ni escena de cierre**

## 5. Sistema de Animaciones
- [ ] Sin Animation Clips (.anim) — 0 archivos
- [ ] Sin Animator Controllers (.controller) — 0 archivos
- [ ] Sin componentes Animator en ningún GameObject
- [ ] Sin animaciones de UI (fades, transiciones)
- [ ] Sin animaciones de personaje (gato, brazos del jugador)
- [ ] Todo el movimiento es procedural vía Transform (Lerp, Rotate)

## 6. Sistema de Audio, Iluminación y Partículas
### Audio
- [x] EventBus + AudioManager funcional
- [x] AudioMixer con grupos (Master > SFX > UI, Music)
- [x] AudioClipRegistry con 21 entries
- [x] 28 archivos SFX + 5 ambientes + 2 botones
- [ ] 2 clips sin asignar: `item_equip`, `player_death_impact`
- [ ] Sin sistema de ambientes reproduciéndose
- [ ] Un solo AudioSource (sin separación SFX/Música/Ambiente)

### Iluminación
- [x] URP configurado (PC + Mobile)
- [x] Post-processing profiles existentes
- [x] Luces reactivas (ReactiveLight.cs)
- [x] Fog + color grading básico
- [ ] Solo 3 luces en escena — muy pocas
- [ ] DefaultVolumeProfile con componentes de test/debug
- [ ] Sin baked lighting ni light probes

### Partículas
- [x] Efecto de botella al romperse (BottleProjectile)
- [x] Efecto de puzzle resuelto (ClockPuzzles)
- [ ] Sin más efectos: impacto, sangre, polvo (pendiente)

## 7. Sistema de UI + Eventos
### Eventos
- [x] EventBus genérico funcional (AudioPlayEvent, ThoughtEvent, GameOverEvent)
- [x] Eventos bien cableados entre scripts
- [x] ThoughtSystem completo (trigger, display, registry)

### UI
- [x] HealthUI (slider de vida)
- [x] UIHotbar (slots de inventario)
- [x] PauseMenu completo
- [x] GameOverMenu (Retry / BackToMenu)
- [x] MenuManager (Start / Exit)
- [x] AudioSettings (sliders SFX/Music)
- [x] ThoughtDisplay (texto narrativo)
- [x] Interaction Prompt con filtros (cat, botella, LockedDoor)
- [ ] PadlockUI.cs está vacío (stub)

## 8. Sistema de Cámaras (Cinemachine) + Corrutinas
### Cámaras
- [ ] ❌ **Cinemachine no está instalado** — paquete ausente
- [ ] ❌ **Cinemachine no se usa** — 0 referencias
- [x] Cámara FPS manual (FirstPersonStateMachineController)
- [x] 3 cámaras en escena (player, environment, weapon overlay)
- [x] Cámara Overlay para armas configurada correctamente
- [x] CameraDeathDrop con curva de animación y fade
- [ ] Potencial conflicto: 2 cámaras con tag "MainCamera"

### Corrutinas
- [x] 8 StartCoroutine en 4 scripts
- [x] Corrutina de ataque del gato (multi-fase con try/finally)
- [x] CameraDeathDrop usa Time.unscaledDeltaTime (funciona en pausa)
- [x] ReactiveLight y ThoughtDisplay con tracking de corrutina

## 9. Inteligencia Artificial (IA)
- [x] FSM del gato con 3 estados (Idle/Guard/Attack)
- [x] Detección por Raycast + Line of Sight
- [x] Huida al ser golpeado por botella
- [x] AttackTrigger + AttackInvoker
- [ ] Sin NavMeshAgent — el gato no navega el nivel
- [ ] Sin pathfinding — el gato es estático (solo rota en su lugar)
- [ ] Sin patrullaje ni waypoints
- [ ] Monolito de 752 líneas (viola SRP)

## 10. Desafío Final Integrador
- [x] Puzzle del reloj elimina al gato (Destroy)
- [x] GameOver funcional con menú
- [ ] ❌ **No hay condición de victoria** — el juego no termina
- [ ] ❌ **No hay pantalla de créditos ni escena de cierre**
- [ ] ❌ **ShockTrigger no implementado** (Fase 5 pendiente)
- [ ] ❌ **Sin secuencia final** — después del reloj el juego sigue sin rumbo
- [ ] Escena de shock (cuerpos) parcial: trigger existe, efectos no

---

## Resumen Visual

```
1.  Horror/Ambientación/Inmersión    ████████░░░░   ~50%
2.  Organización del Proyecto        ██████████░░   ~75%
3.  Diseño del Escenario y Nivel      ██████████░░   ~75%
4.  Path Route y Jugabilidad          ███████░░░░░   ~65%  (sin win condition)
5.  Sistema de Animaciones            ░░░░░░░░░░░░   ~0%
6.  Audio, Iluminación y Partículas   ███████░░░░░   ~55%
7.  UI + Eventos                      ██████████░░   ~85%
8.  Cámaras (Cinemachine) + Corrut.  ██████░░░░░░   ~50%  (sin Cinemachine)
9.  Inteligencia Artificial (IA)      █████░░░░░░░   ~35%
10. Desafío Final Integrador          ███░░░░░░░░░   ~20%
```
