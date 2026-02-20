# LOGIYGames Player Controller

## Project Overview

This is a **Unity-based player controller system** developed by LOGIYGames. It provides a modular, extensible framework for character control in 3D games with support for:

- **Multiple camera perspectives**: First-person, Third-person (Free Look & Look Forward), Top-Down
- **Modular character system**: Plug-and-play modules for character behavior
- **State machine-driven movement**: Clean state-based movement logic (Idle, Run, Sprint, Jump, Fall, Crouch, Roll, etc.)
- **VRM character support**: Integration with UniVRM10 for VRM avatars including look-at and lip-sync modules
- **Cinemachine integration**: Professional camera control using Unity's Cinemachine package
- **Input System**: Unity's new Input System with support for both keyboard/mouse and mobile touch controls

## Directory Structure

```
LOGIYGames/
├── LOGIYGamesPlayerController/    # Main player controller package
│   ├── Another/                   # Additional modules (IK, VRM, interaction)
│   ├── Camera/                    # Camera controllers and utilities
│   ├── Demo/                      # Demo scene and setup
│   ├── Inputs/                    # Input handling (InputReader, drag handlers)
│   ├── Managers/                  # Singleton managers (Character, Camera)
│   ├── Prefabs/                   # Pre-built character and camera prefabs
│   └── Scripts/                   # Core controller scripts
│       ├── BasicMotion/           # Movement and rotation strategies
│       ├── Modules/               # Character modules base and implementations
│       ├── StateMachine/          # State machine data and states
│       └── Animations/            # Animator integration utilities
└── Shared/                        # Shared utilities across projects
    ├── EventChannel/              # ScriptableObject event system
    ├── StateMachine/              # Generic state machine implementation
    ├── Tools/                     # Utilities (Timers, Debug, Extensions)
    └── Singleton.cs               # Generic singleton base class
```

## Key Components

### Core Architecture

| Component | Description |
|-----------|-------------|
| `Character` | Main character component implementing `IControllable`, handles movement input, velocity, and motion |
| `CharacterManager` | Singleton managing active character control, supports multiple character switching |
| `CameraManager` | Singleton managing camera perspectives and Cinemachine virtual cameras |
| `InputReader` | ScriptableObject-based input handler using Unity Input System |
| `ModulesController` | Manages character modules with priority-based execution |

### Design Patterns

- **Strategy Pattern**: Movement and rotation strategies (`IMovementStrategy`, `IRotationStrategy`)
- **State Machine**: Generic `StateMachine` class with transition support for movement states
- **Observer Pattern**: `EventChannel<T>` ScriptableObject event system for decoupled communication
- **Module Pattern**: `IModule` interface for composable character behaviors
- **Singleton**: `Singleton<T>` base class for global managers

### Module System

Character behavior is extended through modules inheriting from `MonoModuleBase`:

```csharp
public abstract class MonoModuleBase : MonoBehaviour, IModule
{
    public int ModulePriority { get; protected set; }
    public virtual void Initialize() { }
    public virtual void OnUpdate(float deltaTime) { }
    public virtual void OnFixedUpdate(float fixedDeltaTime) { }
    public virtual void OnLateUpdate(float deltaTime) { }
}
```

**Available Modules:**
- `CharacterGravityModule` - Gravity handling
- `SensorsModule` - Ground/obstacle detection
- `EyeBlinkerModule` - Automatic eye blinking
- `EyesTracking` - Eye tracking behavior
- `IKBodyModule` - Inverse kinematics for body
- `IKGrabItem` - IK-based item grabbing
- `PushModule` - Object pushing behavior
- `Vrm10LookAtModule` - VRM look-at with nystagmus simulation
- `VrmLipSyncModule` - VRM lip synchronization

### Camera System

| Controller | Type | Description |
|------------|------|-------------|
| `CinemachineCameraController` | Base | Wraps Cinemachine virtual cameras |
| `FirstPersonCameraController` | 1st Person | Inside-character view |
| `ThirdPersonCameraController` | 3rd Person | Free-look camera following character |
| `TopDownCameraController` | Top-Down | Overhead view |

Switch camera perspectives with **F** key (cycles through all modes).

### Input System

The `InputReader` ScriptableObject provides:
- Character inputs (Move, Look, Jump, Crouch, Attack, Sprint, Interact, Evade, Block, Focus)
- Camera inputs (Zoom)
- UI inputs (Submit, Navigate)
- Game control inputs (Escape, Map, Voice)

**Toggle UI mode** to engage/disengage cursor and UI interaction.

## Building and Running

### Prerequisites

- **Unity Version**: 2021.3 LTS or later (URP/HDRP compatible)
- **Required Packages**:
  - Cinemachine
  - Input System
  - UniVRM10 (for VRM support)

### Setup Steps

1. **Import the package** into your Unity project's `Assets/LOGIYGames` folder
2. **Install dependencies** via Package Manager:
   - Cinemachine
   - Input System
3. **Create InputReader**: Right-click → Create → Input → InputReader
4. **Configure character**:
   - Add `Character` component to your character GameObject
   - Assign required references (InputReader, transforms)
   - Add desired modules
5. **Setup camera**:
   - Add `CameraManager` prefab to scene
   - Configure virtual camera preferences
6. **Run the demo scene**: Open `LOGIYGamesPlayerController/Demo/Demo.unity`

### Default Controls

| Action | Key/Button |
|--------|------------|
| Move | WASD / Left Stick |
| Look | Mouse / Right Stick |
| Jump | Space |
| Sprint | Left Shift |
| Crouch | Left Ctrl |
| Attack | Left Mouse Button |
| Interact | E |
| Evade/Roll | Space (while moving) |
| Block | Right Mouse Button |
| Focus | F (toggle) |
| Switch Camera | F (in game) |
| Switch Character | 1 / 2 |
| Toggle UI | (Configurable) |

## Development Conventions

### Naming Conventions

- **Classes**: PascalCase (e.g., `CharacterManager`, `MonoModuleBase`)
- **Methods**: PascalCase (e.g., `HandleInputs()`, `SetCharacterControl()`)
- **Properties**: PascalCase with `{ get; private set; }` pattern
- **Private fields**: camelCase with underscore prefix for serialized fields (e.g., `_instance`, `_stateMachine`)
- **Interfaces**: IPrefix (e.g., `IModule`, `IControllable`, `IRotationStrategy`)

### Code Style

- **Region blocks**: Used for organizing related fields (`#region VelocityVariables`)
- **XML documentation**: Used on public APIs and important properties
- **SerializeField**: Explicit attribute for Unity-serialized private fields
- **Header attributes**: Group inspector fields with `[Header("Category")]`
- **Tooltip attributes**: Add context with `[Tooltip("Description")]`

### Architecture Principles

1. **Separation of Concerns**: Movement logic separated into strategy classes
2. **Composition over Inheritance**: Modules compose character behavior
3. **ScriptableObject Events**: Decouple systems with `EventChannel<T>`
4. **Priority-based Execution**: Modules execute based on `ModulePriority` value (lower = earlier)

### Testing Practices

- Demo scene provided for manual testing
- Character switching (1/2 keys) for multi-character scenarios
- Camera perspective cycling (F key) for camera testing

## Key ScriptableObjects

| Type | Purpose |
|------|---------|
| `InputReader` | Centralized input configuration |
| `EventChannel` | Event broadcasting without direct references |
| `StatesDataSO` | State machine configuration data |

## Extensions & Customization

### Adding a New Module

```csharp
public class MyCustomModule : MonoModuleBase
{
    [SerializeField] private float myValue = 1.0f;
    
    public override void Initialize()
    {
        // Setup code
    }
    
    public override void OnFixedUpdate(float fixedDeltaTime)
    {
        // Physics update code
    }
}
```

### Adding a New Movement State

1. Create state class implementing `IState`
2. Add to `MovementStateDriver` state list
3. Configure transitions in state machine

### Adding a New Camera Perspective

1. Create `CinemachineCameraController` prefab
2. Add to `CameraManager.cinemachineCameraControllers` list
3. Implement switch method in `CameraManager`

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Character not moving | Check `InputReader` assignment and input action bindings |
| Camera not following | Verify `CinemachineCameraFollowTransform` and `CinemachineCameraLookAtTransform` assigned |
| Modules not executing | Ensure `ModulesController` is present and modules are enabled |
| VRM look-at not working | Confirm `UniVRM10` package is installed and `Vrm10Instance` assigned |

## License

Proprietary - LOGIYGames
