# LOGIYGames Player Controller

## Project Overview

This is a **Unity player controller framework** developed by LOGIYGames. It provides a modular, extensible architecture for character control in 3D games with support for:

- **Character movement** with acceleration/deceleration and inertia
- **Camera-relative movement** strategies
- **State machine** for character states
- **Event-driven input system** using Unity Input System
- **Module-based architecture** for extensible character behavior
- **Cinemachine integration** for camera control
- **Event channels** for decoupled communication

## Directory Structure

```
LOGIYGames/
├── LOGIYGamesPlayerController/    # Main player controller package
│   ├── Another/                   # Additional modules (IK, VRM, lip sync, etc.)
│   ├── Camera/                    # Camera controllers and utilities
│   ├── Demo/                      # Demo scenes and examples
│   ├── Inputs/                    # Input handling (InputReader, DragPointerHandler)
│   ├── Managers/                  # Manager singletons (CharacterManager, CameraManager)
│   ├── Prefabs/                   # Prefab assets
│   └── Scripts/                   # Core scripts
│       ├── Animations/            # Animator utilities
│       ├── BasicMotion/           # Movement and rotation strategies
│       ├── Modules/               # Character modules system
│       └── StateMachine/          # State machine implementation
└── Shared/                        # Shared utilities
    ├── EventChannel/              # ScriptableObject event system
    ├── StateMachine/              # Generic state machine
    ├── Tools/                     # Utility classes (Timer, DebugDraw, etc.)
    └── Singleton.cs               # Generic Singleton base class
```

## Key Architectural Patterns

### Module System

Character behavior is organized into modules implementing `IModule`:

```csharp
public interface IModule
{
    int ModulePriority { get; }
    void Initialize();
    void OnUpdate(float deltaTime);
    void OnFixedUpdate(float fixedDeltaTime);
    void OnLateUpdate(float deltaTime);
}
```

Modules are attached via `MonoModuleBase` which provides Unity lifecycle integration.

### Character Controller Wrapper

The `GenericControllerWrapper` abstracts the underlying character controller (Unity CharacterController, kinematic, etc.):

- `KinematicControllerWrapper` - Kinematic rigidbody-based
- `UnityControllerWrapper` - Unity CharacterController-based

### Movement & Rotation Strategies

Strategy pattern for customizable movement:

- `IMovementStrategy` - Movement direction calculation
  - `CameraRelativeMovement` - Movement relative to camera
  - `CameraAlongMovement` - Movement along camera axis
- `IRotationStrategy` - Rotation behavior
  - `ToMousePointRotation` - Rotate towards mouse cursor
  - `CameraAlongRotation` - Rotation aligned with camera

### State Machine

A flexible state machine in `Shared/StateMachine/`:

```csharp
var sm = new StateMachine();
sm.AddTransition(idleState, walkState, isMovingPredicate);
sm.Update();      // Logic updates
sm.FixedUpdate(); // Physics updates
sm.LateUpdate();  // Late updates
```

### Event System

Two event systems are used:

1. **Input Events** - `InputReader` uses UnityEvents for input actions
2. **Event Channels** - ScriptableObject-based pub/sub for decoupled communication:
   ```csharp
   EventChannel<T> - For typed events
   EventChannel    - For simple signals
   ```

### Input System

Uses **Unity Input System** with `InputReader` ScriptableObject:

- Character inputs (move, jump, attack, etc.)
- Camera inputs (look, zoom)
- UI inputs (navigation, submit)
- Automatic UI engagement/disengagement with cursor locking

## Key Classes

| Class | Location | Purpose |
|-------|----------|---------|
| `Character` | Scripts/Modules | Main character component with movement, rotation, jumping |
| `CharacterManager` | Managers | Manages active controllable character |
| `InputReader` | Inputs | ScriptableObject for input handling |
| `CameraManager` | Managers | Manages camera target and behavior |
| `StateMachine` | Shared/StateMachine | Generic state machine with transitions |
| `Singleton<T>` | Shared | Generic MonoBehaviour singleton |
| `EventChannel<T>` | Shared/EventChannel | ScriptableObject event bus |

## Development Conventions

### Naming
- Interfaces prefixed with `I` (e.g., `IModule`, `IState`)
- ScriptableObjects use `CreateAssetMenu` attribute
- Base classes use `Base` suffix (e.g., `MonoModuleBase`)

### Code Style
- C# properties with `{ get; private set; }` for encapsulation
- XML documentation comments on public members
- Region directives for organizing variables (e.g., `#region VelocityVariables`)

### Architecture
- Prefer composition over inheritance via modules
- Use strategy pattern for interchangeable behaviors
- Decouple systems via EventChannels
- Manager pattern for global state (CharacterManager, CameraManager)

## Building and Running

This is a Unity package within the `Assets/LOGIYGames` directory.

### Setup
1. Ensure Unity Input System package is installed
2. Ensure Cinemachine package is installed (for camera features)
3. Import the `LOGIYGames` folder into your Unity project's `Assets` folder

### Usage Example

```csharp
// Character setup
public class MyCharacter : Character {
    void Update() {
        HandleInputs(); // Process input
        // Movement handled by Character base class
    }
}
```

### Testing
- Demo scenes are located in `LOGIYGamesPlayerController/Demo/`
- Press Play in Unity to test with configured characters

## Dependencies

- **Unity Input System** - Input handling
- **Cinemachine** - Camera control
- **Unity Visual Scripting** (optional) - Referenced in InputReader

## Notes

- Some comments in source files are in Russian (Cyrillic encoding may display incorrectly)
- The project uses `IControllable` interface for switching between multiple characters
- VRM support modules exist for VRM avatar integration (look-at, lip sync, IK)
