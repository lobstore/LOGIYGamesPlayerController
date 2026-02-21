# LOGIYGames Player Controller

A comprehensive Unity player controller framework built with modular architecture, supporting both standard Unity CharacterController and KinematicCharacterController, with VRM character support.

## Project Overview

This is a **Unity C# project** containing a sophisticated player controller system designed for 3D games. The framework provides:

- **Modular character controller** with support for multiple movement states (Idle, Walk, Run, Sprint, Jump, Fall, Crouch, Roll)
- **State machine architecture** for clean movement logic separation
- **Multiple controller backends**: Unity's CharacterController and KinematicCharacterController
- **Cinemachine integration** for camera control (1st/3rd person)
- **VRM character support** with look-at, lip-sync, and IK modules
- **Event-driven input system** using Unity's Input System package
- **Network-ready** architecture with Unity Netcode support

## Directory Structure

```
Assets/LOGIYGames/
├── LOGIYGamesPlayerController/    # Main controller package
│   ├── Another/                   # Additional modules (IK, VRM, interactions)
│   ├── Camera/                    # Cinemachine camera controllers
│   ├── Demo/                      # Demo scene
│   ├── Inputs/                    # Input System configuration
│   ├── Managers/                  # Manager singletons (Camera, Character)
│   ├── Prefabs/                   # Prefabs (cameras, characters)
│   └── Scripts/                   # Core controller scripts
│       ├── Animations/            # Animator integration
│       ├── BasicMotion/           # Movement/rotation strategies
│       ├── Debug/                 # Debugging utilities
│       ├── Modules/               # Character modules system
│       └── StateMachine/          # State machine implementation
└── Shared/                        # Shared utilities
    ├── EventChannel/              # ScriptableObject event system
    ├── StateMachine/              # Reusable state machine
    └── Tools/                     # Utilities (timers, extensions)
```

## Key Components

### Core Architecture

| Component | Description |
|-----------|-------------|
| `Character` | Main character component implementing `IControllable` interface |
| `MovementStateDriver` | Drives the state machine based on input and sensor data |
| `StateMachine` | Generic state machine with timed transitions |
| `GenericControllerWrapper` | Abstract wrapper for different controller backends |
| `ModulesController` | Manages character modules with priority-based execution |
| `CharacterManager` | **LEGO-style character switching** - all characters stay active, only control is switched |

### Movement States

- **IdleState** - Stationary character
- **WalkState** - Walking movement
- **RunState** - Running movement
- **SprintState** - Sprinting movement
- **CrouchState** - Crouching
- **JumpState** - Jumping
- **FallingState** - Airborne
- **LandingState** - Landing recovery
- **RollState** - Rolling/evasion
- **StopState** - Stopping animation

### Input System

Uses **Unity Input System** with `InputReader` ScriptableObject:
- Movement input (analog stick/WASD)
- Look input (mouse/right stick)
- Actions: Jump, Crouch, Sprint, Attack, Block, Interact, Evade, Focus
- UI engagement/disengagement with cursor control

### Camera System

- **CinemachineCameraController** - Manages virtual cameras
- **CameraManager** - Singleton for camera target switching
- Supports 1st person and 3rd person virtual cameras
- Camera zoom and FOV control

### VRM Integration

- **Vrm10LookAtModule** - Eye and head tracking with nystagmus simulation
- **VrmLipSyncModule** - Voice synchronization
- **IKBodyModule** - Inverse kinematics for body
- **IKGrabItem** - Hand IK for item interaction
- **EyeBlinkerModule** - Automatic eye blinking

### Shared Utilities

| Utility | Purpose |
|---------|---------|
| `Singleton<T>` | Generic MonoBehaviour singleton base class |
| `EventChannel<T>` | ScriptableObject-based event system |
| `StateMachine` | Reusable state machine pattern |
| `Timer` classes | Stopwatch, Countdown, Interval timers |
| `DebugDraw` | Runtime debugging visualization |

## Development Conventions

### Code Style

- **XML documentation comments** for public APIs
- **Region directives** for organizing related code sections
- **Property-based access** for encapsulation (e.g., `{ get; private set; }`)
- **Interface segregation** with focused interfaces (`IModule`, `IControllable`, `IState`)
- **Strategy pattern** for movement/rotation behaviors

### Module Pattern

Modules implement `IModule` interface with priority-based execution:

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

### State Machine Pattern

States implement `IState` interface:

```csharp
public interface IState
{
    void Enter();
    void Exit();
    void LogicUpdate();
    void PhysicsUpdate();
    void LateUpdate();
}
```

### Naming Conventions

- **PascalCase** for classes, methods, properties
- **_underscore** prefix for private fields
- **SerializedField** attribute for inspector-exposed private fields
- **ScriptableObject** assets use descriptive names with type suffix

## Building and Running

### Prerequisites

- **Unity** (version compatible with project settings)
- **Unity Input System** package
- **Cinemachine** package
- **UniVRM10** package for VRM support
- **KinematicCharacterController** (optional, for KCC backend)

### Setup

1. Open the Unity project containing this asset
2. Ensure all package dependencies are installed via Package Manager
3. Import the `LOGIYGames` folder into your project's `Assets` folder
4. Configure the `InputReader` asset in the project
5. Set up the `StatesDataSO` with desired state parameters

### Demo Scene

Load `LOGIYGamesPlayerController/Demo/Demo.unity` to see the controller in action with the included VRM character.

## Key Configuration Files

| File | Purpose |
|------|---------|
| `InputReader.asset` | Input System configuration |
| `StatesDataSO.asset` | State machine parameters (speeds, durations, thresholds) |
| `Cinemachine*.prefab` | Camera preset configurations |

## Common Patterns

### LEGO-Style Character Switching

All characters remain active in the scene - only control is transferred (like LEGO games):

```csharp
// Switch to specific character by index
CharacterManager.Instance.SetCharacterControl(0); // First character
CharacterManager.Instance.SetCharacterControl(1); // Second character

// Check current character
int currentIndex = CharacterManager.Instance.CurrentCharacterIndex;
Character current = CharacterManager.Instance.CurrentControllable as Character;

// Check if switching is available
bool canSwitch = CharacterManager.Instance.CanSwitchCharacters();
```

**Default Controls:**
- `1`, `2`, `3` - Direct character selection
- `Tab` - Cycle to next character
- `Q` - Cycle to previous character

**How it works:**
1. All characters stay active and rendered in the scene
2. Only one character receives input (the controlled one)
3. Camera follows the current character's transform
4. Switch effects play on both characters during transition
5. Non-controlled characters can still have AI or idle animations

### Event-Driven Communication

```csharp
// Subscribe
eventChannel.Register(listener);

// Publish
eventChannel.Invoke(value);
```

### Module Registration

Add `IModule` components to character GameObject; `ModulesController` auto-discovers and orders by priority.

## Notes

- Some source files contain **Russian comments** (legacy code)
- The project uses **Unity Netcode** annotations for potential multiplayer support
- Debug features included for state machine validation and movement testing
