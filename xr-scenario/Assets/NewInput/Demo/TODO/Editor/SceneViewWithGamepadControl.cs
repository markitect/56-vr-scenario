using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Input;

public class SceneViewWithGamepadControl : SceneView
{
    [MenuItem("Window/Scene View with Gamepad Control")]
    public static void Open()
    {
        EditorWindow.GetWindow<SceneViewWithGamepadControl>();
    }

    const float kHorizontalRotateSpeed = 90.0f;
    const float kVerticalRotateSpeed = 90.0f;
    TimeHelper timeHelper;

    public void Update()
    {
        // Don't handle input when not focused.
        if (focusedWindow != this)
            return;

        var gamepad = Gamepad.current;
        if (gamepad == null)
            return;

        float deltaTime = timeHelper.Update();

        var leftStickX = gamepad.leftStickX.value;
        var leftStickY = gamepad.leftStickY.value;
        var vertical = -gamepad.leftBumper.value + gamepad.rightBumper.value;

        var rightStickX = gamepad.rightStickX.value;
        var rightStickY = gamepad.rightStickY.value;

        var zoom = -gamepad.leftTrigger.value + gamepad.rightTrigger.value;

        if (leftStickX == 0 && leftStickY == 0 && vertical == 0 && rightStickX == 0 && rightStickY == 0 && zoom == 0)
            return;

        // Make speed of motion be based on size of current frame selection.
        Vector3 motion = new Vector3(leftStickX, vertical, leftStickY) * size * deltaTime;
        pivot = pivot + rotation * motion;

        Vector3 camPos = pivot - rotation * Vector3.forward * cameraDistance;

        // Allow scaling size.
        // We do this after getting camPos so that the camera won't be moved by changing the size.
        size = size * Mathf.Pow(2, deltaTime * -zoom);

        // Normal FPS camera behavior
        Quaternion newRotation = rotation;
        newRotation = Quaternion.AngleAxis(rightStickY * kVerticalRotateSpeed * deltaTime, rotation * -Vector3.right) * newRotation;
        newRotation = Quaternion.AngleAxis(rightStickX * kHorizontalRotateSpeed * deltaTime, Vector3.up) * newRotation;
        rotation = newRotation;

        pivot = camPos + rotation * Vector3.forward * cameraDistance;

        Repaint();
    }
}

internal struct TimeHelper
{
    public float deltaTime;
    long lastTime;

    public void Begin()
    {
        lastTime = System.DateTime.Now.Ticks;
    }

    public float Update()
    {
        deltaTime = (System.DateTime.Now.Ticks - lastTime) / 10000000.0f;
        lastTime = System.DateTime.Now.Ticks;
        return deltaTime;
    }
}
