# Rotation Architecture Guide

## Problem Solved

**KinematicCharacterController** manages rotation internally through `ICharacterController.UpdateRotation()`, which conflicted with the external rotation system in `Character.Rotate()`.

## Solution

The rotation logic uses `CController.Rotate(deltaRotation)` to apply rotation deltas, which are then applied:
- **KinematicControllerWrapper**: In `UpdateRotation()` callback
- **UnityControllerWrapper**: Directly to `transform.rotation`

## Architecture Flow

```
BaseState.PhysicsUpdate()
    ↓
Character.Rotate(targetRotation, smoothTime)
    ↓
Calculate delta: deltaRotation = targetRotation * inverse(currentRotation)
    ↓
CController.Rotate(deltaRotation)
    ↓
┌─────────────────────────────────────────────────────────────┐
│ KinematicControllerWrapper          │ UnityControllerWrapper│
│ ───────────────────────────────────  │ ─────────────────────│
│ Stores pending rotation             │ Applies immediately   │
│ Applies in UpdateRotation()         │ to transform.rotation │
└─────────────────────────────────────────────────────────────┘
```

## Key Changes

### 1. Character.Rotate()

Uses original smoothing logic but delegates to wrapper:

```csharp
public void Rotate(Quaternion targetRotation, float turnSmoothTime = 0)
{
    float smoothTime = turnSmoothTime > 0 ? turnSmoothTime : TurnSmoothTime;
    
    if (smoothTime > 0f)
    {
        // Smooth rotation using Slerp
        Quaternion smoothedRotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            smoothTime * Time.fixedDeltaTime
        );
        Quaternion deltaRotation = smoothedRotation * Quaternion.Inverse(transform.rotation);
        CController.Rotate(deltaRotation);
    }
    else
    {
        // Instant rotation
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(transform.rotation);
        CController.Rotate(deltaRotation);
    }
}
```

### 2. KinematicControllerWrapper.Rotate()

Stores pending rotation for application in `UpdateRotation()`:

```csharp
public override void Rotate(Quaternion a_rotDelta)
{
    // Store pending rotation to be applied in UpdateRotation
    m_pendingRotation = a_rotDelta;
    m_hasPendingRotation = true;
}

public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
{
    if (m_hasPendingRotation)
    {
        // Apply pending rotation delta
        currentRotation = currentRotation * m_pendingRotation;
        m_hasPendingRotation = false;
        m_pendingRotation = Quaternion.identity;
    }
}
```

### 3. UnityControllerWrapper.Rotate()

Applies rotation directly:

```csharp
public override void Rotate(Quaternion a_rotDelta)
{
    m_characterController.transform.rotation *= a_rotDelta;
    m_cachedRotDelta = a_rotDelta;
}
```

## Usage

### Setting Rotation

```csharp
// Through Character (recommended)
character.RotateToDirection(moveDirection, turnSmoothTime: 5f);
character.Rotate(targetRotation, turnSmoothTime: 5f);

// Directly through wrapper (delta rotation only)
controller.Rotate(Quaternion.Euler(0, 90, 0)); // Rotate 90 degrees around Y
```

## Benefits

1. **Unified Interface**: Same rotation API for both controller types
2. **Proper KCC Integration**: Rotation works through KinematicCharacterController's update cycle
3. **Smooth Transitions**: Built-in smoothing with configurable smooth time
4. **Controller Agnostic**: Character and State code doesn't need to know which controller is used
5. **Simple Delta-Based**: Uses rotation deltas which work naturally with both systems

## Technical Details

### Rotation Smoothing

Smoothing is done in `Character.Rotate()` using `Quaternion.Slerp`:

```csharp
Quaternion smoothedRotation = Quaternion.Slerp(
    transform.rotation,      // Current rotation
    targetRotation,          // Target rotation
    smoothTime * Time.fixedDeltaTime  // Interpolation factor
);
```

### Delta Rotation

The delta rotation is calculated as:

```csharp
Quaternion deltaRotation = targetRotation * Quaternion.Inverse(currentRotation);
```

This delta is then applied by the wrapper:
- **Kinematic**: `currentRotation = currentRotation * pendingRotation`
- **Unity**: `transform.rotation *= deltaRotation`

### Update Order

**KinematicCharacterController:**
1. `KinematicCharacterSystem` calls `UpdateRotation()`
2. Wrapper applies pending rotation delta
3. Motor applies rotation to transform

**Unity CharacterController:**
1. `UnityControllerWrapper.Rotate()` is called
2. Rotation is applied immediately to `transform.rotation`
