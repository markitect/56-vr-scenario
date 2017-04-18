using UnityEngine;
using UnityEngine.Experimental.Input;

// A simple character controller that listens for events and translates them into state changes
// of the character.

public class SimpleCharacterControlUsingEvents : SimpleCharacterControlBase
{
    private float forwardBackwardAmount;
    private float leftRightAmount;
    private float rotateAmount;
    private bool fire;

    public new void OnEnable()
    {
        base.OnEnable();

        InputSystem.eventHandler += OnInput;
    }

    public new void OnDisable()
    {
        base.OnDisable();

        InputSystem.eventHandler -= OnInput;
    }

    protected bool OnInput(InputEvent inputEvent)
    {
        var keyEvent = inputEvent as KeyEvent;
        if (keyEvent != null)
        {
            if (keyEvent.key == KeyCode.LeftControl)
            {
                fire = keyEvent.isDown;
                ////REVIEW: we should have something like InputEvent.Use()
                return true;
            }

            if (keyEvent.key == KeyCode.A)
            {
                leftRightAmount = keyEvent.isDown ? -1.0f : 0.0f;
                return true;
            }

            if (keyEvent.key == KeyCode.D)
            {
                leftRightAmount = keyEvent.isDown ? 1.0f : 0.0f;
                return true;
            }

            if (keyEvent.key == KeyCode.W)
            {
                forwardBackwardAmount = keyEvent.isDown ? 1.0f : 0.0f;
                return true;
            }

            if (keyEvent.key == KeyCode.S)
            {
                forwardBackwardAmount = keyEvent.isDown ? -1.0f : 0.0f;
                return true;
            }

            if (keyEvent.key == KeyCode.LeftArrow)
            {
                rotateAmount = keyEvent.isDown ? -1.0f : 0.0f;
                return true;
            }

            if (keyEvent.key == KeyCode.RightArrow)
            {
                rotateAmount = keyEvent.isDown ? 1.0f : 0.0f;
                return true;
            }

            return false;
        }

        var pointerEvent = inputEvent as PointerEvent;
        if (pointerEvent != null)
        {
            return false;
        }

        var genericEvent = inputEvent as GenericControlEvent;
        if (genericEvent != null)
        {
            ////REVIEW: for working directly with events, we need the numeric control indices

            if (genericEvent.device is Gamepad)
            {
            }

            if (genericEvent.device is Pointer)
            {
            }

            return false;
        }

        return false;
    }

    protected override void ProcessInput()
    {
        if (fire)
            FireProjectileInInterval();

        m_NewPosition += transform.forward * forwardBackwardAmount + transform.right * leftRightAmount;
        m_NewRotation.y += 2.5f * rotateAmount;
    }
}
