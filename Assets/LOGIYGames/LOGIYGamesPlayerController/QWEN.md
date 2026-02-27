# LOGIYGames Player Controller

A modular, extensible player controller framework for Unity, designed for action games with complex character movement, AI behavior, and camera systems. Built with Cinemachine for camera management and Unity's New Input System.

## Project Overview

This is a **Unity C# project** providing a complete character controller solution with:

- **State-based movement system** - Finite State Machine (FSM) architecture for movement states (Idle, Walk, Run, Sprint, Jump, Fall, Crouch, Roll, etc.)
- **Modular character architecture** - Component-based modules (gravity, sensors, animation) that can be mixed and matched
- **AI behavior framework** - State machine-driven AI with patrol, chase, attack, and idle states using NavMesh for pathfinding
- **Camera system** - Multi-perspective camera support (First Person, Third Person Free Look, Top Down) via Cinemachine
- **Input abstraction** - Unified input interface supporting both player and AI control via Unity Input System
- **Strategy pattern** - Pluggable movement and rotation strategies (Camera-relative, Input-relative, Target-relative)

## Directory Structure

```
LOGIYGamesPlayerController/
├── Scripts/
│   ├── AI/                    # AI behavior states and brain driver
│   │   ├── AIBrainStateDriver.cs
│   │   ├── AIBaseState.cs
│   │   ├── AIIdleState.cs
│   │   ├── AIPatrolState.cs
│   │   ├── AIChaseState.cs
│   │   ├── AIAttackState.cs
│   │   └── Data/              # AI configuration presets
│   ├── Animation/             # Animation module integration
│   ├── Audio/                 # Audio module for SFX
│   ├── Camera/                # Cinemachine camera controllers
│   ├── Inputs/                # Input System readers and handlers
│   ├── Managers/              # Singleton managers (Player, Camera)
│   ├── Modules/               # Character modules (gravity, sensors)
│   ├── Movement/              # Core movement system
│   │   ├── States/            # Movement state implementations
│   │   ├── BasicMotion/       # Motion and rotation strategies
│   │   ├── Driver/            # Movement state driver
│   │   └── Data/              # Movement configuration data
│   └── Shared/                # Common utilities
│       ├── StateMachine/      # FSM core interfaces
│       └── Tools/             # Timers, debug utilities
├── Prefabs/                   # Ready-to-use prefabs
│   ├── Character.prefab
│   ├── Camera.prefab
│   ├── CameraManager.prefab
│   └── CharacterManager.prefab
└── Data/                      # ScriptableObject configurations
    ├── AI/
    ├── Camera/
    └── Movement/
```

## Core Architecture

### Character Module System

The `Character` class (`Scripts/Modules/Character.cs`) is the central component:

```csharp
public class Character : MonoModuleBase, IControllable
{
    // Core properties
    public IMovementInputReader Input { get; set; }
    public ControllerWrapperBase CController { get; set; }
    public IMovementStrategy CurrentMovementStrategy { get; set; }
    public IRotationStrategy CurrentRotationStrategy { get; set; }
    
    // Movement properties
    public float SpeedMultiplier { get; set; }
    public float Acceleration { get; set; }
    public float Deceleration { get; set; }
    public Vector3 Velocity { get; set; }
}
```

### State Machine

Custom FSM implementation (`Scripts/Shared/StateMachine/`) with:

- **IState** interface: `Enter()`, `Exit()`, `LogicUpdate()`, `PhysicsUpdate()`, `LateUpdate()`
- **State transitions** with predicates for conditional switching
- **Any-state transitions** for global state changes

### Movement States

All movement states inherit from `BaseMovementState`:

| State | Description |
|-------|-------------|
| `IdleState` | Stationary character |
| `WalkState` | Basic walking movement |
| `RunState` | Standard running |
| `SprintState` | High-speed movement |
| `CrouchState` | Lowered stance |
| `JumpState` | Vertical leap |
| `FallingState` | Airborne (no ground) |
| `LandingState` | Jump recovery |
| `RollState` | Evasive maneuver |
| `StopState` | Deceleration to halt |

### Strategy Pattern

**Movement Strategies** (`IMovementStrategy`):
- `CameraRelativeMovement` - Move relative to camera view
- `InputRelativeMovement` - Move relative to input direction
- `NoneMovement` - No movement

**Rotation Strategies** (`IRotationStrategy`):
- `CameraRelativeRotation` - Face camera direction
- `InputRelativeRotation` - Face input direction
- `ToTargetRotation` - Face target transform
- `ToMousePointRotation` - Face mouse cursor position

### AI System

AI uses the same state machine architecture:

```csharp
public class AIBrainStateDriver : MonoModuleBase
{
    public NavMeshAgent NavMeshAgent { get; }
    public StateMachine StateMachine { get; }
    public Transform Target { get; set; }
    
    // Detection methods
    public bool IsTargetDetected()
    public bool HasLineOfSight()
    public bool HasLostTarget()
}
```

**AI States**: `AIIdleState`, `AIPatrolState`, `AIChaseState`, `AIAttackState`

### Input System

Unity's New Input System with abstraction layer:

```csharp
public interface IMovementInputReader
{
    Vector2 MovementInput { get; }
    Vector2 LookInput { get; }
    bool JumpPressed { get; }
    bool SprintPressed { get; }
    // ...
}
```

`InputReader` implements player input; `AIInputReader` implements AI input for seamless swapping.

### Camera System

`CameraManager` singleton handles camera perspective switching:

- **First Person** - Direct view through character eyes
- **Third Person Free Look** - Orbital camera with mouse look
- **Third Person Look Forward** - Fixed forward orientation
- **Top Down** - Overhead view

## Key Components

| Component | Path | Purpose |
|-----------|------|---------|
| `Character` | `Scripts/Modules/Character.cs` | Core character controller |
| `StateMachine` | `Scripts/Shared/StateMachine/` | FSM implementation |
| `PlayerManager` | `Scripts/Managers/PlayerManager.cs` | Player character management |
| `CameraManager` | `Scripts/Managers/CameraManager.cs` | Camera perspective switching |
| `InputReader` | `Scripts/Inputs/InputReader.cs` | Input System wrapper |
| `AIBrainStateDriver` | `Scripts/AI/AIBrainStateDriver.cs` | AI behavior controller |
| `ModulesController` | `Scripts/Modules/ModulesController.cs` | Module lifecycle management |

## Usage

### Basic Character Setup

1. Add `Character` component to GameObject
2. Assign `ControllerWrapperBase` (CharacterController, Rigidbody, or Kinematic)
3. Configure movement strategies in inspector
4. Link `InputReader` via `PlayerManager`

### Adding Custom Movement States

```csharp
[Serializable]
public class CustomState : BaseMovementState
{
    public CustomState(MovementStateDriver ctx, MovementStateData data) 
        : base(ctx, data) { }
    
    public override void Enter()
    {
        base.Enter();
        // Setup
    }
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // Movement logic
    }
}
```

### AI Configuration

1. Add `AIBrainStateDriver` to AI GameObject
2. Assign NavMeshAgent (auto-configured)
3. Create `AIStatesPresetBase` ScriptableObject
4. Set patrol points and detection ranges

## Development Conventions

### Naming Patterns

- **Interfaces**: `I` prefix (`IState`, `IModule`, `IMovementStrategy`)
- **Base classes**: Descriptive names with `Base` suffix (`MonoModuleBase`, `BaseMovementState`)
- **Managers**: Singleton pattern (`PlayerManager`, `CameraManager`)
- **Data classes**: `*Data` suffix (`MovementStateData`, `JumpStateData`)

### Module Pattern

Modules follow a standard lifecycle:

```csharp
public interface IModule
{
    int ModulePriority { get; }
    bool Enabled { get; }
    void Initialize();
    void OnUpdate(float deltaTime);
    void OnFixedUpdate(float fixedDeltaTime);
    void OnLateUpdate(float deltaTime);
}
```

### State Machine Usage

```csharp
// Initialize
_stateMachine = new StateMachine();
_stateMachine.AddTransition(idleState, runState, () => input.MovementInput.magnitude > 0.1f);
_stateMachine.SetState(idleState);

// Update loop
_stateMachine.Update();      // Logic
_stateMachine.FixedUpdate(); // Physics
```

## Dependencies

- **Unity Engine** (version depends on project)
- **Cinemachine** - Camera system
- **Unity Input System** - Input handling
- **Unity NavMesh** - AI pathfinding
- **Unity Visual Scripting** (optional) - Used in some components

## Prefabs

| Prefab | Description |
|--------|-------------|
| `Character.prefab` | Base character with controller |
| `Camera.prefab` | Camera setup |
| `CameraManager.prefab` | Camera management system |
| `CharacterManager.prefab` | Player/AI management |
| `1stPersonVirtualCamera.prefab` | First-person camera |
| `3rdPersonVirtualCamera.prefab` | Third-person camera |
| `TopDownPersonVirtualCamera.prefab` | Top-down camera |

## Notes

- The project uses **Unity's New Input System** (not legacy Input Manager)
- Character controller supports multiple wrapper types: `CharacterController`, `Rigidbody`, `Kinematic`, `NavMesh`
- AI uses NavMesh for pathfinding but handles rotation/movement separately for better control
- State machine supports both specific transitions and "any state" transitions
