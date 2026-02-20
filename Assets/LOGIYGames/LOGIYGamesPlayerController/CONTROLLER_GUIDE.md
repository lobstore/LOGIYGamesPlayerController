# Controller Wrapper Architecture Guide

## Overview

This architecture allows seamless swapping between Unity's built-in `CharacterController` and `KinematicCharacterController` without changing any character behavior code.

## Architecture

```
GenericControllerWrapper (Abstract Base Class)
├── UnityControllerWrapper (Unity CharacterController)
└── KinematicControllerWrapper (KinematicCharacterController)
```

## Key Components

### 1. GenericControllerWrapper

The abstract base class that provides a unified interface for both controller types:

- **Ground Detection**: `IsGrounded`
- **Velocity**: `Velocity`
- **Movement**: `Move()`, `MoveAndRotate()`, `Rotate()`
- **Capsule Properties**: `Height`, `Radius`, `Center`, `MaxStepHeight`, `SlopeLimit`
- **Transform**: `SetPosition()`, `SetRotation()`, `SetPositionAndRotation()`
- **Collision Toggle**: `CollisionEnabled`
- **Jumping**: `Jump(float force)`
- **Collider Access**: `GetCollider()`

### 2. UnityControllerWrapper

Wraps Unity's built-in `CharacterController`:
- Requires `CharacterController` component
- Uses CharacterController's native movement methods
- Direct velocity and ground detection from CharacterController

### 3. KinematicControllerWrapper

Wraps KinematicCharacterController's `KinematicCharacterMotor`:
- Requires `CapsuleCollider` and `KinematicCharacterMotor` components
- Implements `ICharacterController` interface
- Handles movement through motor's update cycle
- Provides consistent behavior with UnityControllerWrapper

## How to Swap Controllers

### Using Unity CharacterController

1. Add these components to your character GameObject:
   - `CharacterController`
   - `UnityControllerWrapper`
   - `Character`
   - `CharacterGravityModule`
   - `SensorsModule`

2. Configure the `CharacterController` in the Inspector

### Using KinematicCharacterController

1. Add these components to your character GameObject:
   - `CapsuleCollider`
   - `KinematicCharacterMotor`
   - `KinematicControllerWrapper`
   - `Character`
   - `CharacterGravityModule`
   - `SensorsModule`

2. Configure the `KinematicCharacterMotor` in the Inspector:
   - Set capsule dimensions
   - Configure grounding settings
   - Set step handling method
   - Configure ledge handling

### No Code Changes Required

The `Character.cs` script and all other character behavior scripts work with **both** controllers without modification because they interact through the `GenericControllerWrapper` interface.

## Component Dependencies

### All Characters Need:
```
GenericControllerWrapper (UnityControllerWrapper OR KinematicControllerWrapper)
Character
CharacterGravityModule
SensorsModule
```

### For KinematicCharacterController:
```
CapsuleCollider (required by KinematicControllerWrapper)
KinematicCharacterMotor (required by KinematicControllerWrapper)
```

### For Unity CharacterController:
```
CharacterController (required by UnityControllerWrapper)
```

## Implementation Details

### Movement

Both controllers handle movement differently but expose the same interface:

**UnityControllerWrapper:**
```csharp
public override void Move(Vector3 a_move)
{
    m_characterController.Move(a_move * Time.deltaTime);
}
```

**KinematicControllerWrapper:**
```csharp
public override void Move(Vector3 a_move)
{
    m_cachedMoveDelta = a_move * Time.deltaTime;
    m_kinematicMotor.SetPosition(m_kinematicMotor.TransientPosition + m_cachedMoveDelta);
}
```

### Ground Detection

Both controllers provide ground detection through their respective systems:

**UnityControllerWrapper:**
```csharp
public override bool IsGrounded => m_characterController.isGrounded;
```

**KinematicControllerWrapper:**
```csharp
public override bool IsGrounded => m_kinematicMotor.GroundingStatus.IsStableOnGround;
```

### Capsule Properties

Both controllers use capsule collision, so properties map directly:

| Property | Unity CharacterController | KinematicCharacterMotor |
|----------|--------------------------|------------------------|
| Height | `characterController.height` | `capsuleCollider.height` |
| Radius | `characterController.radius` | `capsuleCollider.radius` |
| Center | `characterController.center` | `capsuleCollider.center` |
| Step Height | `characterController.stepOffset` | `motor.MaxStepHeight` |
| Slope Limit | `characterController.slopeLimit` | Custom field (used in validation) |

## Sensor Module Compatibility

The `SensorsModule` works with both controllers by using the `GetCollider()` method to access the underlying collider bounds:

```csharp
public Vector3 DetectionOrigin
{
    get
    {
        Collider col = m_controllerWrapper.GetCollider();
        return new Vector3(
            col.bounds.center.x,
            col.bounds.center.y + m_detectionOriginYOffset,
            col.bounds.center.z
        );
    }
}
```

## Gravity Module Compatibility

The `CharacterGravityModule` uses the controller wrapper's `Move()` method for consistent gravity application:

```csharp
private void ApplyGravity(float fixedDeltaTime)
{
    m_controllerWrapper.Move(Velocity * fixedDeltaTime);
}
```

## Benefits

1. **Flexibility**: Switch controller types without rewriting character logic
2. **Testing**: Easily compare controller performance and behavior
3. **Modularity**: Add new controller types by extending `GenericControllerWrapper`
4. **Consistency**: All character modules work with any controller type
5. **Maintainability**: Centralized controller-specific logic

## Migration Example

### Before (Direct CharacterController usage):
```csharp
// This couples your code to CharacterController
private CharacterController controller;
controller.Move(direction);
```

### After (GenericControllerWrapper usage):
```csharp
// This works with any controller type
private GenericControllerWrapper controller;
controller.Move(direction);
```

## Notes

- **KinematicCharacterController** requires the `KinematicCharacterController` package
- Both controllers use capsule collision, ensuring consistent collision behavior
- The `SensorsModule` uses Physics.SphereCast for ground detection, working independently of the controller type
- For advanced features (like rigidbody interaction), configure the specific controller's settings in the Inspector
