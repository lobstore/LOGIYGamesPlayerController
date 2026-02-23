# LOGIY Games Player Controller

## Project Overview

This is a **Unity C# player controller framework** developed by LOGIY Games. It provides a modular, extensible architecture for character movement, AI behavior, camera control, and input handling in 3D games.

### Key Features

- **Modular Architecture**: Component-based system using `IModule` interface with priority-based execution
- **State Machine System**: Custom state machine implementation for movement states and AI behavior
- **Multiple Controller Support**: Abstract `ControllerWrapperBase` supporting both Unity's CharacterController and KinematicCharacterController
- **AI System**: Behavior state machine with Idle, Patrol, Chase, and Attack states using NavMesh pathfinding
- **Camera System**: Multiple camera perspectives (First Person, Third Person FreeLook, Third Person Look Forward, Top-Down) via Cinemachine
- **Input System**: Unity Input System integration with support for both player and AI input providers

## Directory Structure

```
Scripts/
├── AI/                     # AI behavior system
│   ├── AIBrainStateDriver.cs    # Main AI controller with state machine
│   ├── AIBaseState.cs      # Base class for AI states
│   ├── AIIdleState.cs      # Idle behavior
│   ├── AIPatrolState.cs    # Patrol behavior
│   ├── AIChaseState.cs     # Target chasing behavior
│   ├── AIAttackState.cs    # Attack behavior
│   └── AIInputReader.cs    # AI input provider (mimics player input)
├── Camera/                 # Camera control system
│   ├── CameraManager.cs    # Singleton camera manager
│   ├── CameraZoom.cs       # Camera zoom functionality
│   ├── Controllers/        # Cinemachine camera controllers
│   │   └── CinemachineCameraController.cs
│   └── CinemachineCameraUtility/
│       ├── CameraTarget.cs
│       └── CinemachineMobileInputCotroller.cs
├── Inputs/                 # Input handling
│   ├── InputReader.cs      # ScriptableObject input reader
│   ├── IInputReader.cs     # Input reader interface
│   ├── IControllable.cs    # Controllable entity interface
│   └── DragPointerHandler.cs
├── Managers/               # Manager singletons
│   ├── CameraManager.cs    # Camera management
│   └── PlayerManager.cs    # Player management
├── Modules/                # Modular component system
│   ├── Character.cs        # Main character component
│   ├── ModulesController.cs # Module manager
│   ├── CharacterGravityModule.cs
│   ├── Sensors/
│   │   └── SensorsModule.cs
│   └── Base/
│       ├── IModule.cs      # Module interface
│       ├── MonoModuleBase.cs
│       └── NetworkModuleBase.cs
├── Movement/               # Movement system
│   ├── MovementStateDriver.cs  # Movement state machine driver
│   ├── MovementStatesDataSO.cs # ScriptableObject movement state data
│   ├── States/             # Movement state machine states
│   │   ├── StateMachine.cs
│   │   ├── BaseState.cs
│   │   ├── IdleState.cs
│   │   ├── WalkState.cs
│   │   ├── RunState.cs
│   │   ├── SprintState.cs
│   │   ├── CrouchState.cs
│   │   ├── JumpState.cs
│   │   ├── FallingState.cs
│   │   ├── LandingState.cs
│   │   ├── RollState.cs
│   │   └── StopState.cs
│   └── BasicMotion/        # Motion strategies and controller wrappers
│       ├── ControllerWrapperBase.cs
│       ├── CharacterControllerWrapper.cs
│       ├── KinematicControllerWrapper.cs
│       ├── RigidbodyControllerWrapper.cs
│       ├── NavMeshControllerWrapper.cs
│       ├── MotionStrategy/
│       │   ├── IMovementStrategy.cs
│       │   ├── CameraRelativeMovement.cs
│       │   └── InputRelativeMovement.cs
│       └── RotationStrategy/
│           ├── IRotationStrategy.cs
│           ├── CameraRelativeRotation.cs
│           ├── InputRelativeRotation.cs
│           ├── CameraAlongRotation.cs
│           ├── ToMousePointRotation.cs
│           └── ToTargetRotation.cs
└── Shared/                 # Shared utilities
    ├── Singleton.cs        # Generic singleton base
    ├── StateMachine/       # Generic state machine
    │   ├── IState.cs
    │   ├── IPredicate.cs
    │   └── FuncPredicate.cs
    └── Tools/              # Utility classes (timers, debug drawing)
        ├── Timer.cs
        ├── CountdownTimer.cs
        ├── IntervalTimer.cs
        ├── StopwatchTimer.cs
        ├── TimersManager.cs
        ├── DebugDraw.cs
        ├── Vector3Extensions.cs
        └── UpdateType.cs
```

## Architecture Patterns

### Module System

The controller uses a priority-based module system:

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

Modules are discovered via `GetComponents<IModule>()` and executed in priority order by `ModulesController`.

### State Machine

Custom state machine with transition predicates:

```csharp
// Usage pattern
_stateMachine = new StateMachine();
_stateMachine.AddTransition(fromState, toState, new FuncPredicate(condition));
_stateMachine.Update();      // Logic updates
_stateMachine.FixedUpdate(); // Physics updates
```

### Strategy Pattern

Movement and rotation use strategy interfaces:

```csharp
public interface IMovementStrategy {
    Vector3 GetMovementDirection();
}

public interface IRotationStrategy {
    Quaternion GetRotation();
}
```

### Controller Wrapper Pattern

Abstract base class unifies different character controller implementations:

```csharp
public abstract class ControllerWrapperBase : MonoBehaviour
{
    public abstract bool IsGrounded { get; }
    public abstract Vector3 Velocity { get; }
    public abstract void Move(Vector3 move);
    public abstract void Rotate(Quaternion targetRotation);
    // ... additional abstract members
}
```

## Key Components

### Character.cs

Main character component that aggregates all systems:

- **InputProvider**: Abstracts input source (player or AI)
- **ControllerWrapper**: Abstracts underlying physics controller
- **Movement/Rotation Strategies**: Pluggable movement behaviors
- **Velocity Properties**: Acceleration, deceleration, speed multiplier

### AIBrainStateDriver.cs

AI controller using NavMesh for pathfinding:

- **States**: Idle → Patrol → Chase → Attack
- **Detection**: Range-based with line-of-sight raycasting
- **Integration**: Sets `Character.InputProvider.MovementInput` for seamless player/AI swap

### CameraManager.cs

Singleton managing Cinemachine virtual cameras:

- **Perspectives**: First Person, Third Person FreeLook, Third Person Look Forward, Top-Down
- **Runtime Switching**: Press `F` to cycle through camera modes

### MovementStateDriver.cs

Drives character movement state machine:

- **States**: Idle, Walk, Run, Sprint, Crouch, Jump, Fall, Land, Roll, Stop
- **Transitions**: Configured via transition table with predicates
- **Timed States**: Support for timed transitions (jump duration, landing, roll)

### SensorsModule.cs

Handles ground and obstacle detection:

- **Ground Detection**: Sphere cast for grounded check
- **Obstacle Detection**: Above and below obstacle detection
- **Slope Detection**: Ground angle calculation

## Building and Running

This is a **Unity package**. To use:

1. **Unity Version**: Requires Unity with Cinemachine and Input System packages installed
2. **Package Location**: `Assets/LOGIYGames/LOGIYGamesPlayerController/`
3. **Dependencies**:
   - Unity Input System
   - Cinemachine
   - Unity NavMesh (for AI)
   - Unity Netcode (optional, for networked modules)

### Setup Steps

1. Add `ModulesController` to your character GameObject
2. Add `Character` component to your character GameObject
3. Add a controller wrapper (e.g., `CharacterControllerWrapper` or `KinematicControllerWrapper`)
4. Configure `InputReader` ScriptableObject asset
5. Set up camera references (`CinemachineCameraFollowTransform`, `CinemachineCameraLookAtTransform`)
6. Add `MovementStateDriver` for movement state machine
7. Add `SensorsModule` for ground/obstacle detection

## Development Conventions

### Naming Conventions

- **Interfaces**: `I` prefix (e.g., `IModule`, `IState`, `IPredicate`)
- **ScriptableObjects**: `*SO` suffix (e.g., `MovementStatesDataSO`)
- **Base Classes**: `*Base` suffix (e.g., `MonoModuleBase`, `ControllerWrapperBase`)
- **Managers**: Singleton pattern with `Instance` static property
- **Private Fields**: `_` prefix for private fields (e.g., `_stateMachine`)
- **Serialized Fields**: `m_` prefix for serialized fields in some modules (e.g., `m_controllerWrapper`)

### Code Style

- **Regions**: Used for organizing properties (`#region VelocityVariables`)
- **Properties**: Prefer auto-properties with `{ get; set; }` or `{ get; private set; }`
- **Serialization**: `[SerializeField]` for private serialized fields, `[field: SerializeField]` for property serialization
- **Events**: UnityEvents for inspector-exposed events, C# events for code-only
- **Namespaces**: `LOGIYGames` root namespace with sub-namespaces:
  - `LOGIYGames.CharacterCore` - Character-related components
  - `LOGIYGames.AI` - AI behavior system
  - `LOGIYGames.Movement` - Movement system
  - `LOGIYGames.Scripts.AI` - Alternative AI namespace (legacy)

### State Machine Pattern

All states implement `IState`:

```csharp
public interface IState
{
    void Enter();
    void Exit();
    void LogicUpdate();    // Called in Update
    void PhysicsUpdate();  // Called in FixedUpdate
    void LateUpdate();
}
```

### Module Lifecycle

Modules follow Unity's update cycle:

1. `Initialize()` - Called in `Awake()`
2. `OnUpdate()` - Called in `Update()`, ordered by `ModulePriority`
3. `OnFixedUpdate()` - Called in `FixedUpdate()`, ordered by `ModulePriority`
4. `OnLateUpdate()` - Called in `LateUpdate()`, ordered by `ModulePriority`

## Common Tasks

### Adding a New Movement State

1. Create class inheriting from `BaseState`
2. Implement `Enter()`, `Exit()`, `PhysicsUpdate()` as needed
3. Add state data to `MovementStatesDataSO`
4. Configure transitions in `MovementStateDriver.ConfigureTransitions()`

### Adding a New AI State

1. Create class inheriting from `AIBaseState`
2. Implement state logic using `AIBrainStateDriver` reference
3. Add transition conditions in `AIBrainStateDriver.ConfigureTransitions()`

### Adding a New Module

1. Implement `IModule` interface or inherit from `MonoModuleBase`
2. Set `ModulePriority` for execution order
3. Add component to character GameObject

### Adding a New Movement/Rotation Strategy

1. Implement `IMovementStrategy` or `IRotationStrategy` interface
2. Assign strategy to `Character.CurrentMovementStrategy` or `Character.CurrentRotationStrategy`

## Known TODOs in Codebase

- [ ] Make `IInputReader` abstraction to change between AI/Player (partially implemented)
- [ ] Make builder pattern for movement/rotation strategies
- [ ] Make `ICBFollowable` abstraction for camera follow targets
- [ ] Make builder for AI archetypes and AI configuration
- [ ] Make `ControllerWrapperManager` for controller wrapper selection

## Additional Notes

### Input System

The project uses Unity's new Input System with generated `GameInputs` class:
- Input actions are defined in `.inputactions` file
- `InputReader` implements generated interfaces for callback handling
- Supports both keyboard/mouse and mobile touch input

### AI System

AI uses NavMesh for pathfinding while delegating actual movement to Character:
- `AIBrainStateDriver` calculates desired movement direction via NavMeshAgent
- Movement input is set via `AIInputReader.SetMovementInput()`
- Character executes actual movement using its controller wrapper

### Camera System

Camera perspectives are managed via Cinemachine virtual cameras:
- Priority-based camera switching
- Mobile touch support via `CinemachineMobileInputCotroller`
- Zoom functionality via `CameraZoom` component
