using UnityEngine;
using UnityEngine.Experimental.Input;

public class OutputTest : MonoBehaviour
{
    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return;

        gamepad.leftVibration.value = gamepad.leftTrigger.value;
        gamepad.rightVibration.value = gamepad.rightTrigger.value;
    }
}
