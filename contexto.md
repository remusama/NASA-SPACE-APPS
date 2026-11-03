# Resumen: Tutorial Completo de VR en Unity 6.2 — Villim Tutorials

Este es un tutorial extenso de YouTube que enseña a crear un juego VR desde cero en Unity 6.2, dividido en 6 episodios que cubren desde la configuración inicial hasta la interacción con UI. A continuación, el resumen estructurado por partes.

---

## 1. Configuración de Unity y Proyecto VR

### Instalación
- Descargar **Unity Hub** desde `unity.com/download`.
- Instalar **Unity 6.2** (o superior). En el instalador:
  - Habilitar **Microsoft Visual Studio**.
  - Habilitar **Android Build Support** con **OpenGDK, Android SDK y NDK** (necesario para exportar a Meta Quest).
- Crear un proyecto nuevo usando la plantilla **Universal 3D** (NO la plantilla VR, porque se saltan pasos importantes de configuración que el tutorial quiere enseñar).

### Preparación de la escena
- Crear una nueva escena llamada **Main VR Scene**.
- Añadir un **Plane** (3D Object) como suelo, posición `(0,0,0)`.
- Crear un **material** llamado `Ground Material`, asignarlo al plano y ajustar color (negro) y reducir smoothness.

### Configuración XR (paso crítico)
1. **Edit → Project Settings → XR Plug-in Management** → Instalar.
2. Habilitar **OpenXR** tanto en **Desktop** como en **Android**.
3. En **OpenXR Settings**, seleccionar los **interaction profiles** de los headsets (evitar Eye Gaze).
4. En Android, habilitar **Meta Quest Support**.
5. Añadir a la Main Camera el componente **Track Pose Driver** (NO el "Input System").
6. **Ventana → Package Manager → Unity Registry** → instalar **XR Interaction Toolkit 3.2.1** (o `com.unity.xr.interaction.kit@3.2.1`).
7. Importar los **Starter Assets** desde Samples.
8. **Project Settings → XR Plug-in Management → Project Validation** → usar **Fix All** en Desktop y Android.
   - Único issue restante: post-procesado **Screen Space Ambient Occlusion**. Se puede eliminar en **PC Renderer** (right click → Remove) o ignorar.

---

## 2. Construcción del VR Player

### XR Origin
- Eliminar la Main Camera original.
- Right click → **XR → XR Origin (VR)**, renombrar a **VR Player**, posición `(0,0,0)`.
- En el componente **XR Origin**, poner **Tracking Origin Mode** en **Floor** (respeta la altura del jugador).

### Controladores
- En **Camera Offset** → Create Empty → renombrar **Left Hand** → añadir **Track Pose Driver (Input System)**.
- Duplicar (Ctrl+D) para **Right Hand**.
- Configurar referencias en cada mano:
  - **Position Input** → `XRI Left/Right Position`
  - **Rotation Input** → `XRI Left/Right Rotation`
  - **Tracking State** → `XRI Left/Right Tracking States`

### Modelos visuales
- Opción rápida: añadir cubos hijos (escala `0.05`), quitar Box Collider.
- Opción mejor: usar los prefabs **XR Controller Left/Right** de los Starter Assets → Controllers. Copiar componente Transform, arrastrar bajo cada mano, pegar valores.

### Testing sin headset
- **Build & Run**: File → Build Profile → añadir Main VR Scene → seleccionar plataforma (Windows o Android/Meta Quest) → conectar headset en modo desarrollador → Build and Run.
- **XR Interaction Simulator** (recomendado para debug): Package Manager → XR Interaction Toolkit → Samples → importar **XR Interaction Simulator**. Permite probar con teclado y ratón sin headset.

---

## 3. Input del Controlador y Hand Presents

### XR Interaction Debugger
- **Windows → Analysis → XR Interaction Debugger** → desactivar Interactors/Interactables, activar **Input Devices**.
- Muestra dispositivos detectados (headset, controladores) y valores en tiempo real de botones, triggers, joysticks.

### Input Actions
- Ubicación: `Assets/Samples/XR Interaction Toolkit/[versión]/Starter Assets/XRI Default Input Action`.
- Contiene **Action Maps** (ej. XRI Left Interaction) con acciones y sus bindings.
- Tipos: **Button** (bool), **Value** (float, Vector2, etc.).
- Recomendación: **no modificar los inputs por defecto** para principiantes.

### Script de prueba (`InputTest`)
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    public InputActionProperty testActionValue;
    public InputActionProperty testActionButton;

    void Update()
    {
        float value = testActionValue.action.ReadValue<float>();
        bool button = testActionButton.action.IsPressed();
        Debug.Log("Value: " + value + " | Button: " + button);
    }
}
```
- Asignar acciones con **Use Reference** (ej. `XRI Left Interaction Select Value` para value, `XRI Left Interaction Select` para button).

### Animar manos (Hand Presents)
1. Importar el paquete de **manos animadas** (link en descripción del video).
2. Arrastrar los prefabs de manos bajo **Left Hand** y **Right Hand** (copiar/pegar Transform como con los controladores). Desactivar los controladores XR si se prefieren manos.
3. El **Animator** usa un **Blend Tree** con dos parámetros: `grip` y `trigger`.
4. Script `AnimateHandOnInput`:
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty triggerValue;
    public InputActionProperty gripValue;
    public Animator handAnimator;

    void Update()
    {
        float trigger = triggerValue.action.ReadValue<float>();
        float grip = gripValue.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", trigger);
        handAnimator.SetFloat("Grip", grip);
    }
}
```
5. Referencias: `XRI Left/Right Interaction Activate Value` (trigger) y `XRI Left/Right Interaction Select Value` (grip).

---

## 4. Locomoción: Movimiento Continuo y Teleportación

### Movimiento continuo
1. Añadir **Character Controller** al VR Player: radius `0.1`, center Y `1`, height `2`.
2. Crear empty **Locomotion** → añadir **Locomotion Mediator** (auto-añade **XR Body Transformer** con referencia al Character Controller).
3. Crear empty **Move** bajo Locomotion → añadir **Continuous Move Provider**:
   - Mediator → Locomotion
   - Forward Source → Main Camera
   - Left Hand Move Input → `XRI Left Locomotion/Move`
   - Right Hand → **Unused**

### Rotación
- **Continuous Turn Provider**: Right Hand → `XRI Right Locomotion/Turn`. Left → Unused.
- **Snap Turn Provider** (alternativa, menos motion sickness): Right Hand → `XRI Right Locomotion/Snap Turn`. Desactivar el que no se use (o dejar que el jugador elija).

### Gravedad y salto
- **Gravity Provider** (recomendado sobre "Use Gravity" del Move Provider):
  - Transformation Priority → `10`
  - VR Player Layer → **Ignore Raycast** (No a children)
- **Jump Provider**: Right Hand → `XRI Right Locomotion/Jump`. Botón A por defecto.

### Teleportación
1. Empty **Teleportation** → añadir **Teleportation Provider** (arrastrar Mediator).
2. Arrastrar prefab **Teleport Interactor** (Starter Assets/Prefabs/Interactors) bajo **Right Hand**, reset posición.
3. En el **XR Ray Interactor** del teleport:
   - Endness → **Right**
   - Input Configuration (ambos campos) → `XRI Right Locomotion/Teleport Mode`
4. En el **Plane** del suelo:
   - Añadir **Teleportation Area**
   - Colliders → arrastrar Mesh Collider
   - Distance Calculation Mode → **Collider Volume**
   - Interaction Layer Mask → solo **Teleport** (quitar Default)
5. **Script `TeleportationActivator`** (para que el rayo solo aparezca al presionar):
```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportationActivator : MonoBehaviour
{
    public XRInteractorTeleportInteractor teleportInteractor;
    public InputActionProperty teleportActivatorAction;

    void Start()
    {
        teleportInteractor.gameObject.SetActive(false);
        teleportActivatorAction.action.performed += ActionPerformed;
    }

    void Update()
    {
        if (teleportActivatorAction.action.WasReleasedThisFrame())
            teleportInteractor.gameObject.SetActive(false);
    }

    void ActionPerformed(InputAction.CallbackContext ctx)
    {
        teleportInteractor.gameObject.SetActive(true);
    }
}
```
   - Asignar Teleport Interactor y `XRI Right Locomotion/Teleport Mode`.

### Teleportation Anchor
- Buscar **Teleport Anchor** en Project (búsqueda → All), colocar en escena.
- Fuerza destino y rotación específicos (Match Orientation → Target Up And Forward).

---

## 5. Grab Interaction (XR Grab Interactable)

### Direct Interactor
1. Empty **Left Direct Interactor** bajo Left Hand:
   - **XR Direct Interactor**
   - **Sphere Collider** con **Is Trigger** = true, radius `0.1`
   - Interaction Layer Mask → quitar Teleport
   - Endness → **Left**
   - Improve Accuracy With Sphere Collider → activar
   - Inputs: `XRI Left Interaction Select`, `Select Value`, `Activate`, `Activate Value`
2. Duplicar para **Right Direct Interactor** bajo Right Hand (cambiar a Right y referencias Right).

### Los 3 tipos de movimiento (Movement Type)
| Tipo | Comportamiento |
|------|----------------|
| **Instantaneous** | Mueve el Transform directamente, sin física. Muy responsivo, ignora colisiones. |
| **Kinematic** | Usa Rigidbody en modo kinematic. Puede empujar otros objetos, pero no ser empujado (atraviesa la mesa). |
| **Velocity Tracking** | Rigidbody no-kinematic. Física completa: no atraviesa la mesa, conserva colisiones y joints. |

### Attach Transform vs Dynamic Attach
- **Attach Transform**: punto/rotación fijo donde el objeto se "pega" al agarrarlo (ej. pistola).
- **Use Dynamic Attach**: mantiene el offset inicial del objeto al agarrarlo (más natural para objetos genéricos).

### Ejemplo: Pistola
1. Importar **Low Poly FPS Weapons Lite** (Asset Store, gratis).
2. Reemplazar Mesh Colliders por Box Collider en weapon01.
3. Añadir **XR Grab Interactable** al parent.
4. Crear empty **Attach Transform** (rotar 90° en Y), asignarlo.
5. Script `Pistol` con función `FireBullet()` (instancia bala, aplica velocidad, destruye tras tiempo).
6. En **Interactable Events → Activated** → llamar `Pistol.FireBullet`.
7. Añadir **Audio Source** y clip de disparo (¡no olvidarlo!).

### Two-Hand Interaction (dos manos)
- **Select Mode → Multiple**.
- Crear **Secondary Attach Transform** (duplicar el primero, colocarlo en otra parte del arma).
- Asignarlo en el campo correspondiente.
- El arma seguirá la mano primaria y apuntará hacia la secundaria.
- Se puede sobrescribir el **XR General Grab Transformer** para personalizar (ej. escalado a dos manos con `Two Ended Scaling`, scale multiplier `1`).

---

## 6. Interacciones Avanzadas

### Socket Interactor
1. Empty **Socket Interactor** → **XR Socket Interactor** + **Sphere Collider** (Is Trigger, radius `0.4`).
2. Añadir **Sphere** hijo (escala `0.8`), quitar su collider, material transparente (**URP Unlit**, Surface Type → Transparent, alpha bajo, Render Face → Both).
3. Ajustes clave:
   - **Attach Transform**: offset del objeto al encajar.
   - **Show Interactable Over Mesh**: preview del objeto.
   - **Scale**: útil para inventarios.
   - **Recycle Delay Time**: `0.1` (por defecto 1 es mucho).
   - **Starting Selected Interactable**: objeto ya colocado al iniciar.

### Puertas (Hinge Joint)
1. En la puerta: **Rigidbody** + **Hinge Joint**.
2. Eje correcto (usar gizmo), **Use Limits** + **Use Spring** (spring `0.3`, damper `1`).
3. En el **pomo/manilla**: **Rigidbody** + **Fixed Joint** (conectado al Rigidbody de la puerta).
4. Añadir **XR Grab Interactable** al pomo:
   - Desactivar **Retain Transform Parent** (por escala no uniforme).
   - **Movement Type → Velocity Tracking** (CRÍTICO: sin esto la física de los joints no funciona).
5. Funciona incluso con otros objetos velocity tracking (ej. un cubo para abrir la puerta).

### Cajones (Configurable Joint)
1. En el cajón: **Rigidbody** + **Configurable Joint**.
2. X Motion → **Locked**, Y Motion → **Locked**, Z Motion → **Limited**, todas las rotaciones → **Locked**.
3. **Linear Limit** → `15`.
4. **XR Grab Interactable** con **Velocity Tracking**, desactivar Track Rotation y Track Scale.

### Constraint Grab Movement (restringir ejes)
- Añadir **XR General Grab Transformer** al objeto (override).
- **Permitted Displacement Axis**: desactivar ejes no deseados (ej. quitar Y → solo X/Z).
- **Constraint Axis Displacement Mode**: Object Relative / Object Relative With Locked World Up / World Axis.
- En el XR Grab Interactable: desactivar **Track Rotation** y **Track Scale** si se desea.

---

## 7. UI Interaction (Ray y Poke)

### Crear Canvas
1. Right click → **UI → Canvas** (crea también Event System).
2. Añadir elementos: **Panel**, **Text Mesh Pro** (importar TMP Essentials), **Button**, **Scroll Bar**.
3. Canvas → **Render Mode → World Space**, escala `0.01`, posición `(0,0,0)`.
4. Nota: existe el nuevo **UI Toolkit** en Unity 6.2, pero es más complejo para principiantes; el tutorial usa el sistema clásico.

### Ray Interactor para UI
1. Arrastrar prefab **Ray Interactor** (buscar en Project) bajo **Right Hand**, posición `(0,0,0)`.
2. En **XR Ray Interactor**:
   - Endness → **Right**
   - **UI Interaction** → habilitado
   - Inputs: `XRI Right Interaction UI Press`, `UI Press Value`, `UI Scroll`
3. **Event System**:
   - Quitar **UI Input Module** por defecto.
   - Añadir **XR UI Input Module** → usar preset preconfigurado.
4. **Canvas** → añadir **Tracked Device Graphic Raycaster**.
5. Ocultar el rayo cuando no se interactúa: en **XR Interactor Line Visual** → **Invalid Color Gradient** → alpha del primer elemento a `0`.

### Capas de interacción (Interaction Layer Mask)
- Crear layer **Ray** (Project Settings → Tags and Layers).
- Ray Interactor → Interaction Layer Mask → solo **Ray**.
- Objetos con los que SÍ puede interactuar el rayo → activar layer **Ray** además de Default.
- Ray Interactor también puede agarrar objetos: añadir inputs `XRI Right Interaction Select`, `Select Value`, `Activate`, `Activate Value`.

### Simple Interactable (sin agarrar)
- Cubo + **Simple Interactable** → layer **Ray**.
- **Interactable Events → On Hover Entered / Exited** → cambiar material (ej. rojo al hover, gris default al salir).

### Interaction Group (evitar conflictos entre interactores)
- En **Right Hand** → añadir **Interaction Group**.
- Añadir: **Teleport Interactor**, **Right Direct Interactor**, **Ray Interactor**.
- Solo uno puede activarse a la vez (evita agarrar + teleportar + rayo simultáneamente).
- **Bug del teleport durante UI**: modificar `TeleportationActivator`:
```csharp
public XRRayInteractor rayInteractor;

void Start()
{
    // ...código anterior...
    rayInteractor.onOverUiEnter.AddListener(DisableTeleportRay);
}

public void DisableTeleportRay()
{
    teleportInteractor.gameObject.SetActive(false);
}
```
   - Asignar el Ray Interactor en el inspector.

### Poke Interactor
1. Buscar prefab **Poke Interactor** en Project, arrastrar bajo **Left Hand** y **Right Hand**, posición `(0,0,0)`.
2. **Right click → Unpack Completely** (necesario para mover el Poke Point).
3. Mover el **Poke Point** bajo el hueso del índice (`Index 3` de la mano), reset posición/rotación, rotar 90° en Y, ajustar offset para que salga de la punta del dedo.
4. Endness → **Right** / **Left** según corresponda.
5. **UI Interaction** debe estar habilitado (por defecto).
6. En el **Interaction Group** del Right Hand: añadir el Poke Interactor y configurarlo para que **override** al Ray Interactor (así el rayo se desactiva al usar el poke).

---

## Puntos Clave / Buenas Prácticas

- **NO usar la plantilla VR** si se quiere aprender la configuración manual.
- **Velocity Tracking** es esencial para cualquier objeto con física/joints (puertas, cajones).
- **XR General Grab Transformer** permite override de comportamiento de grab (constraints, scaling).
- **Interaction Group** evita conflictos entre múltiples interactores en la misma mano.
- **Interaction Layer Masks** separan qué interactor afecta a qué objeto (Ray vs Direct vs Teleport).
- El **XR Interaction Simulator** permite desarrollar sin headset.
- Los inputs por defecto del XRI Toolkit son buenos: no modificarlos sin razón.
- El **Track Pose Driver** (no el de Input System) es el correcto para la cámara principal.

---

## Recursos Mencionados
- **Patreon del autor**: código fuente + tutorial exclusivo VR Shooter (1 hora).
- **Low Poly FPS Weapons Lite** (Asset Store, gratis).
- **Cabinet prefab** (link en descripción del video, cortesía de Karissa).
- **Paquete de manos animadas** (link en descripción).
- Documentación de **Meta Quest Link**, **OpenXR** y **XR Interaction Simulator** (en descripción).

---

## Conclusión del Tutorial

Al finalizar los 6 episodios se tiene: VR player funcional, manos animadas con hand presents, locomoción (continua + teleportación + gravedad + salto), grab interaction con 3 modos de movimiento + two-hand + dynamic attach, sockets, puertas con Hinge Joint, cajones con Configurable Joint, constraints de movimiento, e interacción UI con Ray y Poke interactors. Todo con un sistema de capas e Interaction Groups que evita conflictos.