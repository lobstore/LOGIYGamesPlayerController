# LOGIYGames Player Controller

## Project Overview

This is a **Unity character controller framework** designed for 3D games. It provides a modular, extensible architecture for player character control with support for:

- **Multiple character management** - Switch control between different characters
- **Cinemachine integration** - Camera system using Unity Cinachine
- **Input System** - Built on Unity's new Input System package
- **State Machine** - Flexible state machine for character states
- **Module System** - Composable modules for character behaviors
- **Event Channels** - ScriptableObject-based event system for decoupled communication
- **VRM Support** - Modules for VRM character models (look-at, lip sync, IK)

## Directory Structure

```
Assets/LOGIYGames/
├── LOGIYGamesPlayerController/    # Main controller package
│   ├── Another/                   # Additional modules (IK, VRM, interactions)
│   ├── Camera/                    # Cinemachine camera utilities
│   ├── Demo/                      # Demo scene and setup
│   ├── Inputs/                    # Input System assets and handlers
│   ├── Managers/                  # Singleton managers (Character, Camera)
│   ├── Prefabs/                   # Pre-configured prefabs
│   └── Scripts/                   # Core controller scripts
│       ├── Animations/            # Animator utilities
│       ├── BasicMotion/           # Movement and rotation strategies
│       ├── Modules/               # Character module system
│       └── StateMachine/          # State machine implementation
└── Shared/                        # Shared utilities
    ├── EventChannel/              # ScriptableObject event system
    ├── StateMachine/              # Generic state machine
    └── Tools/                     # Utilities (timers, debug, extensions)
```

## Key Components

### Character System

- **`Character`** - Main character component implementing `IControllable`
  - Handles movement input, velocity, acceleration/deceleration
  - Integrates with `GenericControllerWrapper` for physics
  - Supports jumping, rolling, and directional movement

- **`CharacterManager`** - Singleton managing active character control
  - Switches between multiple characters
  - Coordinates camera target assignment

### Input System

- **`InputReader`** - ScriptableObject-based input handler
  - Implements generated `GameInputs` interfaces
  - Provides events for all input actions
  - Manages UI engagement state (cursor lock, input mode switching)

### Camera System

- **`CinemachineCameraController`** - Wrapper for Cinemachine cameras
- **`CameraManager`** - Singleton managing camera targets

### Module Architecture

- **`IModule`** - Base interface for character modules
- **`MonoModuleBase`** - MonoBehaviour base with lifecycle methods:
  - `Initialize()`, `OnUpdate()`, `OnFixedUpdate()`, `OnLateUpdate()`
- **`ModulesController`** - Manages module execution order via priorities

### State Machine

- **`StateMachine`** - Generic state machine with transitions
  - Supports `IState` interface with `Enter()`, `Exit()`, `LogicUpdate()`, `PhysicsUpdate()`
  - Conditional transitions via `IPredicate`
  - Any-state transitions supported

### Movement State Machine

The `MovementStateDriver` manages character movement states with the following transition table:

#### State Transition Table

| From State | To State   | Condition                                      |
|------------|------------|------------------------------------------------|
| Idle       | Walk       | Movement input > 0.1 & grounded                |
| Idle       | Jump       | Jump pressed & grounded                        |
| Idle       | Crouch     | Crouch pressed                                 |
| Idle       | Roll       | Evade pressed                                  |
| Idle       | Falling    | Not grounded                                   |
| Walk       | Idle       | No movement input & grounded                   |
| Walk       | Run        | Strong input (>0.6) & not sprinting            |
| Walk       | Jump       | Jump pressed & grounded                        |
| Walk       | Crouch     | Crouch pressed                                 |
| Walk       | Roll       | Evade pressed                                  |
| Walk       | Falling    | Not grounded                                   |
| Run        | Idle       | No movement input & grounded                   |
| Run        | Walk       | Weak input (<0.6)                              |
| Run        | Sprint     | Sprint pressed & strong input                  |
| Run        | Jump       | Jump pressed & grounded                        |
| Run        | Crouch     | Crouch pressed                                 |
| Run        | Roll       | Evade pressed                                  |
| Run        | Falling    | Not grounded                                   |
| Sprint     | Idle       | No movement input & grounded                   |
| Sprint     | Run        | Sprint released                                |
| Sprint     | Jump       | Jump pressed & grounded                        |
| Sprint     | Crouch     | Crouch pressed                                 |
| Sprint     | Roll       | Evade pressed                                  |
| Sprint     | Falling    | Not grounded                                   |
| Crouch     | Idle       | Crouch released & grounded & no input          |
| Crouch     | Walk       | Crouch released & has movement input           |
| Crouch     | Roll       | Evade pressed                                  |
| Crouch     | Falling    | Not grounded                                   |
| Jump       | Falling    | Jump duration elapsed & not grounded           |
| Jump       | Landing    | Jump duration elapsed & grounded               |
| Falling    | Landing    | Grounded                                       |
| Landing    | Idle       | Landing duration elapsed & no input            |
| Landing    | Walk       | Landing duration elapsed & has movement input  |
| Landing    | Roll       | Evade pressed                                  |
| Roll       | Idle       | Roll duration elapsed & no input & grounded    |
| Roll       | Walk       | Roll duration elapsed & has movement input     |
| Roll       | Falling    | Roll duration elapsed & not grounded           |

#### State Types

- **Base States**: `IdleState`, `WalkState`, `RunState`, `SprintState`, `StopState`, `FallingState`, `LandingState`, `CrouchState`
- **Timed States**: `JumpState`, `RollState` (have duration and cooldown timers)

#### Transition Configuration

Transitions are configured in `MovementStateDriver.ConfigureTransitions()` using the `AddTransition()` helper method:

```csharp
private void AddTransition(IState from, IState to, Func<bool> condition)
{
    _stateMachine.AddTransition(from, to, new FuncPredicate(condition));
}
```

Condition helpers:
- `HasMovementInput()` - Input magnitude > 0.1
- `HasStrongMovementInput()` - Input magnitude > 0.6
- `IsGrounded()` - Sensor module ground detection
- `IsSprinting()` - Sprint input pressed

### Movement Strategies

- **Motion Strategies** (`IMovementStrategy`):
  - `CameraRelativeMovement` - Movement relative to camera direction
  - `CameraAlongMovement` - Camera aligns with movement direction

- **Rotation Strategies** (`IRotationStrategy`):
  - `CameraRelativeRotation` - Rotation relative to camera
  - `CameraAlongRotation` - Rotation along movement
  - `ToMousePointRotation` - Rotate towards mouse cursor

### Shared Utilities

- **`Singleton<T>`** - Generic singleton pattern for MonoBehaviours
- **`EventChannel<T>`** - ScriptableObject event system for decoupled communication
- **Timers** - `CountdownTimer`, `StopwatchTimer`, `IntervalTimer`, `TimersManager`

## Building and Running

### Prerequisites

- Unity (version compatible with Cinemachine and Input System packages)
- Unity Input System package
- Unity Cinemachine package
- VRM packages (for VRM character support)

### Setup

1. Open the Unity project containing this asset
2. Navigate to `Assets/LOGIYGames/LOGIYGamesPlayerController/Demo/` for the demo scene
3. The demo scene showcases the controller with a VRM character model

### Configuration

1. **Input Setup**: Configure input bindings via the `InputReader` asset
2. **Character Setup**: Add `Character` component with required references
3. **Camera Setup**: Use provided camera prefabs or configure Cinemachine cameras

## Development Conventions

### Naming Conventions

- **Interfaces**: Prefix with `I` (e.g., `IControllable`, `IModule`)
- **ScriptableObjects**: Use descriptive names, often with "SO" suffix for data containers
- **Managers**: Singleton classes suffixed with "Manager"

### Architecture Patterns

- **Strategy Pattern**: Used for movement and rotation behaviors
- **Observer Pattern**: Event channels for decoupled communication
- **State Pattern**: State machine for character states
- **Module Pattern**: Composable character behaviors with priority-based execution

### Code Style

- Properties with `{ get; private set; }` for encapsulation
- `[SerializeField]` for Unity Inspector exposure
- XML documentation comments on public APIs
- Region directives for organizing related fields

## Dependencies

- **Unity Input System** - Input handling
- **Unity Cinemachine** - Camera system
- **VRM** (optional) - VRM character model support

## Notes

- The project uses `.meta` files indicating Unity asset database tracking
- Some source files contain Cyrillic comments (Russian language)
- The controller supports both kinematic and dynamic character movement via wrapper classes
