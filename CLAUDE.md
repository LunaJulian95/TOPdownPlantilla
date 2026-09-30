# Proyecto Unity — Contexto para Claude Code

## 1. Resumen del proyecto

Juego 2D (vista top-down) hecho en Unity. El jugador se mueve por un mapa dividido en distintas zonas/áreas conectadas entre sí, con una cámara Cinemachine que sigue al jugador y se "confina" a los límites de cada área. El juego cuenta además con un menú que se abre y cierra con una tecla.

> Nota: este archivo fue redactado a partir de los scripts y assets existentes. Si hay partes del proyecto que no están reflejadas acá (sistema de diálogos, inventario, guardado, etc.), agregalas en las secciones correspondientes.

## 2. Estructura de carpetas (Assets)

Estructura base identificada / sugerida — ajustar si en el proyecto real están organizadas distinto:

- `Assets/Scripts` → Scripts de C# (lógica de jugador, UI, transiciones de mapa, etc.)
- `Assets/Animations` → Clips de animación (.anim) y Animator Controllers (.controller)
- `Assets/Prefabs` → Prefabs reutilizables (Player, objetos interactuables, etc.)
- `Assets/Scenes` → Escenas del juego
- `Assets/Art` / `Sprites` → Sprites y hojas de sprites
- `Assets/Audio` → Música y efectos de sonido

## 3. Scripts principales

### 3.1 `PlayerMovement.cs`
Controla el movimiento del jugador.
- Requiere un componente `Rigidbody2D` en el mismo GameObject (usa `[RequireComponent]`).
- Usa el nuevo Input System de Unity (`UnityEngine.InputSystem`), con el callback `OnMove(InputValue value)` para recibir el input de movimiento (probablemente conectado a un `PlayerInput` con acción "Move").
- Mueve al jugador aplicando velocidad al `Rigidbody2D` en `FixedUpdate` (`rb.linearVelocity`), en vez de mover el `Transform` directamente.
- Actualiza parámetros del Animator en cada frame: `isMoving` (bool), `moveX` y `moveY` (float), que alimentan el Blend Tree de animación.
- Variable expuesta en el Inspector: `moveSpeed` (velocidad de movimiento).

### 3.2 `MapTransitions.cs`
Maneja el pasaje del jugador entre distintas áreas/zonas del mapa.
- Se coloca en triggers (`Collider2D` en modo Trigger) ubicados en los bordes de cada zona del mapa.
- Detecta al jugador mediante `OnTriggerEnter2D`, comparando el tag `"Player"`.
- Al activarse, actualiza el `CinemachineConfiner2D` de la cámara con un nuevo `PolygonCollider2D` (`mapBoundary`), para que la cámara quede confinada a los límites de la nueva zona.
- Tiene un enum `Direction` (Up, Down, Left, Right) y una variable `amountToMove` para desplazar al jugador hacia la nueva zona.
- ⚠️ **Función incompleta**: `UpdatePlayerPosition` calcula la nueva posición pero todavía no la aplica al transform del jugador. Tenerlo en cuenta si se edita este script.
- Depende del paquete `Unity.Cinemachine`.

### 3.3 `MenuController.cs`
Controla la apertura y cierre de un menú simple.
- Referencia un GameObject `menuCanvas` (el Canvas/panel de UI del menú), asignado desde el Inspector.
- El menú arranca desactivado (`Start()` lo pone en `SetActive(false)`).
- Se abre/cierra con la tecla `Tab` (`Input.GetKeyDown(KeyCode.Tab)`), alternando el estado activo del Canvas.
- Usa el Input Manager clásico (`UnityEngine.Input`), a diferencia de `PlayerMovement.cs` que usa el nuevo Input System.

| Script | Responsabilidad | Depende de |
|---|---|---|
| `PlayerMovement.cs` | Movimiento del jugador y parámetros de animación | Rigidbody2D, Animator, Input System |
| `MapTransitions.cs` | Transición entre zonas del mapa y confinamiento de cámara | Cinemachine, PolygonCollider2D, tag "Player" |
| `MenuController.cs` | Mostrar/ocultar el menú con Tab | GameObject de UI (menuCanvas) |

## 4. Sistema de animación del jugador

Animator Controller `Player` (`Player.controller`):
- Parámetros: `moveX` (float), `moveY` (float), `isMoving` (bool), `Speed` (float).
- Contiene un Blend Tree ("Movenent") que mezcla las animaciones direccionales según `moveX` y `moveY`.
- Clips de animación: `playerIdle`, `playerup`, `playerDown`, `playerLeft`, `playeRigth` (⚠️ typo en el nombre del archivo — "Rigth" en vez de "Right" — tenerlo en cuenta al buscar el archivo por nombre).
- La transición entre el estado Idle y el Blend Tree de movimiento se controla con el booleano `isMoving`, que `PlayerMovement.cs` actualiza en cada `Update()`.

## 5. Convenciones del proyecto

- Tag `"Player"` asignado al GameObject del jugador (usado por `MapTransitions.cs` para detectarlo).
- Los scripts no usan namespaces propios; las clases son públicas y heredan de `MonoBehaviour`.
- Mezcla de Input System nuevo (`PlayerMovement`) e Input Manager clásico (`MenuController`) — no asumir un único sistema de input en todo el proyecto.
- Cámara manejada con Cinemachine (`CinemachineConfiner2D`), no con `Camera.main` directamente.

## 6. Notas para el agente de IA

- Antes de crear un script nuevo, revisar si ya existe una funcionalidad similar en `Scripts/` (por ejemplo, no crear un segundo controlador de movimiento).
- Respetar el sistema de Input ya usado en cada script (no mezclar Input System nuevo y clásico dentro del mismo script).
- Al tocar animaciones, actualizar el Animator Controller (`Player.controller`) y no solo los `.anim` sueltos.
- `MapTransitions.cs` tiene una función incompleta (`UpdatePlayerPosition` no aplica el resultado al transform) — si se toca, avisar o corregir según corresponda.
- Completar/editar esta sección con reglas propias del equipo: convenciones de nombres, dónde van los prefabs nuevos, cómo se prueban los cambios, etc.
