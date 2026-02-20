# Character Rotation Troubleshooting Guide

## Problem: Character doesn't rotate

### Checklist

#### 1. Verify MovementStateDriver References

Make sure all references are assigned in the Inspector:

- **Character**: Assign the Character component on the same GameObject
- **Sensors**: Assign the SensorsModule component on the same GameObject
- **InputReader**: Assign your InputReader ScriptableObject asset

#### 2. Verify StateDataSO Configuration

Check your `StatesDataSO` asset:

- Open the asset in the Inspector
- For each state (Idle, Walk, Run, etc.), ensure `Turn Smooth Time` is set to a value > 0 (recommended: 2-10)
- If `Turn Smooth Time` is 0, rotation will be instant or may not work

#### 3. Verify Camera Setup

- Ensure there is a camera tagged as "MainCamera" in your scene
- Check that `CameraManager` is properly initialized
- Verify `Camera.main` is not null (check console for errors)

#### 4. Verify Input System

- Check that InputReader is properly configured with input actions
- Verify that movement input is being received (use debugger or add debug log)
- Check that `CharacterInputsEnable` is set to true

#### 5. Check State Machine Initialization

- Verify that `MovementStateDriver` is on the same GameObject as `Character`
- Check that the state machine initializes without errors (check console)

### Debug Tools

#### CharacterRotationDebugger

Add the `CharacterRotationDebugger` component to your character to see real-time debug information:

- Movement Input value
- Turn Smooth Time
- Current Rotation Strategy
- Target vs Current Rotation
- Camera Perspective Type

#### MovementStateDriverValidator

Add the `MovementStateDriverValidator` component to automatically check references on start.

### Common Issues

#### Issue: Rotation works but is too fast/slow

**Solution**: Adjust `Turn Smooth Time` in your StateDataSO asset
- Higher value = faster rotation
- Lower value = slower rotation

#### Issue: Character only rotates when moving

**Solution**: This is expected behavior for `CameraRelativeRotation`
- Character rotates based on movement input direction
- To rotate without moving, use `CameraAlongRotation` (press Block/Focus button)

#### Issue: Character rotates opposite direction

**Solution**: Check your input axis configuration
- Verify Horizontal and Vertical axes are configured correctly
- Check that input values are in expected range (-1 to 1)

#### Issue: NullReferenceException in rotation

**Solution**: Check for null camera
- Ensure there is a camera tagged "MainCamera"
- Or add camera reference to your rotation strategy

### Code Flow

1. `MovementStateDriver.FixedUpdate()` → `StateMachine.FixedUpdate()`
2. `StateMachine.FixedUpdate()` → `BaseState.PhysicsUpdate()`
3. `BaseState.PhysicsUpdate()` → `_character.Rotate(targetRotation, turnSmoothTime)`
4. `Character.Rotate()` → `transform.rotation = Quaternion.Slerp(...)`

### Testing

1. Add `CharacterRotationDebugger` to your character
2. Enter Play mode
3. Move with WASD keys
4. Observe debug panel:
   - Movement Input should show non-zero values when pressing keys
   - Target Rotation Y should change when moving
   - Current Rotation Y should approach Target Rotation Y

### Quick Fix

If rotation still doesn't work, try this:

1. Select your character GameObject
2. Find `MovementStateDriver` component
3. Re-assign all references (Character, Sensors, InputReader)
4. Open your `StatesDataSO` asset
5. Set `Turn Smooth Time` to 5 for all states
6. Save and re-enter Play mode
