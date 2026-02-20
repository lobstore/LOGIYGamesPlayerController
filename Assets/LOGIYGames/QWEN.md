# LOGIYGames Player Controller

## Project Overview

This is a **Unity C# player controller framework** designed for 3D character movement and camera control. The project provides a modular, state-machine-driven architecture for handling character movement, camera perspectives, and input management.

### Key Features

- **State Machine System**: A robust state machine (`StateMachine.cs`) drives character movement states (Idle, Walk, Run, Sprint, Jump, Fall, Landing, Crouch, Roll)
- **Multiple Controller Wrappers**: Support for different character controller implementations:
  - `UnityControllerWrapper` - Unity's built-in `CharacterController`
  - `KinematicControllerWrapper` - For kinematic character controller integration
  - `RigidbodyControllerWrapper` - For physics-based movement
  - `GenericControllerWrapper` - Base interface for controller abstraction
- **Camera System**: Cinemachine-based camera management with multiple perspectives:
  - First Person
  - Third Person Free Look
  - Third Person Look Forward
  - Top Down
- **Input System**: Uses Unity's new Input System package with `InputReader` ScriptableObject for centralized input handling
- **Module Architecture**: Extensible module system (`MonoModuleBase`, `IModule`) for adding character behaviors
- **Event Channels**: Decoupled event communication system for game events

## Directory Structure

```
Assets/LOGIYGames/
├── LOGIYGamesPlayerController/
│   ├── Another/              # Additional modules (IK, VRM, eye tracking, etc.)
│   ├── Camera/               # Camera controllers and Cinemachine presets
│   ├── Demo/                 # Demo scene
│   ├── Inputs/               # Input System assets and handlers
│   ├── Managers/             # Singleton managers (CameraManager, CharacterManager)
│   ├── Prefabs/              # Prefabs including VRM character
│   └── Scripts/
│       ├── Animations/       # Animation-related scripts
│       ├── BasicMotion/      # Controller wrappers and movement strategies
│       ├── Debug/            # Debug utilities
│       ├── Modules/          # Character modules (gravity, sensors)
│       └── StateMachine/     # Movement state definitions
└── Shared/
    ├── EventChannel/         # Event channel system for decoupled communication
    ├── StateMachine/         # Reusable state machine components
    ├── Tools/                # Utility classes (timers, etc.)
    └── Singleton.cs          # Generic Singleton base class
```

## Core Components

### Character Controller (`Character.cs`)

The main character component that handles:
- Movement input processing
- Velocity-based movement with acceleration/deceleration
- Rotation smoothing
- Jump and roll mechanics
- Height changing (crouching)
- Integration with controller wrappers

### Movement State Driver (`MovementStateDriver.cs`)

Drives the character state machine with timed transitions between states:
- **Idle** → Walk, Jump, Crouch, Roll, Falling
- **Walk** → Idle, Run, Jump, Crouch, Roll, Falling
- **Run** → Idle, Walk, Sprint, Jump, Crouch, Roll, Falling
- **Sprint** → Idle, Run, Jump, Crouch, Roll, Falling
- **Crouch** → Idle, Walk, Roll, Falling
- **Jump** → Landing, Falling
- **Falling** → Landing
- **Landing** → Idle, Walk, Roll
- **Roll** → Idle, Walk, Falling

### Input System (`InputReader.cs`)

A ScriptableObject-based input handler that provides:
- Movement input (2D vector)
- Look input (2D vector)
- Action buttons (Jump, Crouch, Sprint, Attack, Block, Evade, Interact)
- UI engagement/disengagement
- Event-based input notifications

### Camera System (`CameraManager.cs`)

Manages camera perspectives using Cinemachine:
- Priority-based virtual camera switching
- Mobile input support via `DragPointerHandler`
- Perspective type enumeration for state tracking

## Building and Running

### Prerequisites

- **Unity**: Version compatible with the project (check `ProjectSettings/ProjectVersion.txt`)
- **Unity Input System**: Package required for input handling
- **Cinemachine**: Package required for camera system
- **VRM Support**: If using VRM characters, ensure VRM packages are installed

### Setup Steps

1. Open the Unity project containing this asset
2. Ensure required packages are installed via Package Manager:
   - Input System
   - Cinemachine
3. Open the demo scene at `LOGIYGamesPlayerController/Demo/Demo.unity`
4. Press Play to test the controller

### Key Configuration

- **Input Actions**: Configure in `Inputs/InputReader.asset`
- **State Data**: Configure movement parameters in `StateMachine/StatesDataSO.asset`
- **Camera Presets**: Adjust in `Camera/CinemachinePresets/`

## Development Conventions

### Architecture Patterns

1. **Strategy Pattern**: Used for movement and rotation strategies (`IMovementStrategy`, `IRotationStrategy`)
2. **State Pattern**: All movement states implement `IState` interface
3. **Singleton Pattern**: Used for managers (`CameraManager`, `TimersManager`)
4. **ScriptableObject Pattern**: Used for input configuration and state data

### Coding Style

- **Naming**: PascalCase for public members, camelCase for private fields with `m_` prefix in some legacy code
- **Regions**: Used to organize code sections (e.g., `#region Movement Methods`)
- **XML Documentation**: Extensive use of XML comments for public APIs
- **Serialization**: `[SerializeField]` for private serialized fields, `[field: SerializeField]` for auto-properties

### Timer System

The project includes a timer system in `Shared/Tools/`:
- `Timer` - Base timer class
- `CountdownTimer` - Countdown timer implementation
- `IntervalTimer` - Repeating interval timer
- `StopwatchTimer` - Stopwatch functionality
- `TimersManager` - Singleton that updates all registered timers

### Event System

Decoupled communication via `Shared/EventChannel/`:
- `EventChannel<T>` - Generic event channel
- `EventListener<T>` - Event listener component
- Specialized channels for `float` and `int` types

## Key Interfaces

```csharp
// IState - Base interface for all states
public interface IState
{
    void Enter();
    void Exit();
    void LogicUpdate();
    void LateUpdate();
    void PhysicsUpdate();
}

// IModule - Base interface for modules
public interface IModule
{
    int ModulePriority { get; }
    void Initialize();
}

// IControllable - Interface for controllable entities
public interface IControllable
{
    void OnControlGained();
    void OnControlLost();
    void HandleInputs();
}
```

## Notes

- The project supports VRM characters (see `Prefabs/CatgirlBattle.vrm`)
- Mobile input support is available via `DragPointerHandler` and `CinemachineMobileInputController`
- The state machine supports timed transitions with duration and cooldown timers
- Some code comments are in Russian (e.g., `Singleton.cs`)
