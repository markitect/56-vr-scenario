using UnityEngine;
using UnityEngine.Experimental.Input;

// A simple character controller that looks at the state of its respective input devices every fixed update
// and applies their state to the character.

public class SimpleCharacterControlUsingState : SimpleCharacterControlBase
{
    public Transform leftControllerCube;
    public Transform rightControllerCube;

    protected override void ProcessInput()
    {
        // Process VR.
        var vrControlled = VRControl();

        // Process keyboard&mouse.
        var keyboardControlled = !vrControlled && KeyboardControl();
        keyboardControlled |= !vrControlled && PointerControl();

        // Process gamepad.
        if (!vrControlled && !keyboardControlled)
            GamepadControl();
    }

    private bool VRControl()
    {
        var leftHand = TrackedController.leftHand;
        var rightHand = TrackedController.rightHand;

        if (leftHand == null || rightHand == null)
            return false;

        var controllersUsed = false;

        if (leftHand.trigger.value > 0.15f || rightHand.trigger.value > 0.15f)
        {
            FireProjectileInInterval();
            controllersUsed = true;
        }

        if (leftControllerCube != null)
        {
            leftControllerCube.localPosition = leftHand.position.value;
            leftControllerCube.localRotation = leftHand.rotation.value;
        }
        if (rightControllerCube != null)
        {
            rightControllerCube.localPosition = rightHand.position.value;
            rightControllerCube.localRotation = rightHand.rotation.value;
        }

        return controllersUsed;
    }

    private bool KeyboardControl()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return false;

        var keyboardUsed = false;
        if (keyboard.GetControl(KeyCode.W).isHeld)
        {
            m_NewPosition += transform.forward * 0.5f;
            keyboardUsed = true;
        }
        if (keyboard.GetControl(KeyCode.S).isHeld)
        {
            m_NewPosition += transform.forward * -0.5f;
            keyboardUsed = true;
        }
        if (keyboard.GetControl(KeyCode.A).isHeld)
        {
            m_NewPosition += transform.right * -0.5f;
            keyboardUsed = true;
        }
        if (keyboard.GetControl(KeyCode.D).isHeld)
        {
            m_NewPosition += transform.right * 0.5f;
            keyboardUsed = true;
        }

        if (keyboard.GetControl(KeyCode.LeftArrow).isHeld)
        {
            m_NewRotation.y -= 2.5f;
            keyboardUsed = true;
        }
        if (keyboard.GetControl(KeyCode.RightArrow).isHeld)
        {
            m_NewRotation.y += 2.5f;
            keyboardUsed = true;
        }

        if (keyboard.GetControl(KeyCode.LeftControl).isHeld)
        {
            FireProjectileInInterval();
            keyboardUsed = true;
        }

        return keyboardUsed;
    }

    private bool GamepadControl()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return false;

        var leftStickX = gamepad.leftStickX.value;
        var leftStickY = gamepad.leftStickY.value;
        var rightStickX = gamepad.rightStickX.value;
        var rightTrigger = gamepad.rightTrigger.value;

        ////TODO: get rid of ugly manual deadzone handling

        const float kDeadZone = 0.15f;
        const float kRotateSpeed = 2.5f;

        var gamepadUsed = false;
        if (Mathf.Abs(leftStickX) > kDeadZone)
        {
            m_NewPosition += transform.right * leftStickX;
            gamepadUsed = true;
        }
        if (Mathf.Abs(leftStickY) > kDeadZone)
        {
            m_NewPosition += transform.forward * leftStickY;
            gamepadUsed = true;
        }
        if (Mathf.Abs(rightStickX) > kDeadZone)
        {
            m_NewRotation.y += kRotateSpeed * rightStickX;
            gamepadUsed = true;
        }

        if (rightTrigger > kDeadZone)
        {
            FireProjectileInInterval();
            gamepadUsed = true;
        }

        return gamepadUsed;
    }

    private bool PointerControl()
    {
        var pointer = Pointer.current;
        if (pointer == null)
            return false;

        var pointerUsed = false;
        const float kRotateSpeed = 1.0f;

        if (pointer.cursor.isLocked)
        {
            var horizontal = pointer.horizontalDelta.value;
            if (!Mathf.Approximately(0.0f, horizontal))
            {
                m_NewRotation.y += horizontal * kRotateSpeed;
                pointerUsed = true;
            }
        }

        if (pointer.leftButton.isHeld)
        {
            FireProjectileInInterval();
            pointerUsed = true;
        }

        return pointerUsed;
    }
}
