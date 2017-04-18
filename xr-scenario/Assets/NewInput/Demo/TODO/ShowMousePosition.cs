using UnityEngine;
using UnityEngine.Experimental.Input;

public class ShowMousePosition : MonoBehaviour
{
    public void OnGUI()
    {
        var pointer = Pointer.current;
        if (pointer == null || pointer.cursor.isLocked)
            return;

        var positionFromOldSystem = Input.mousePosition;

        GUI.Label(new Rect(25, 25, 220, 30), string.Format("MouseX: {0} (old: {1})", pointer.positionX.value, positionFromOldSystem.x));
        GUI.Label(new Rect(25, 50, 220, 30), string.Format("MouseY: {0} (old: {1})", pointer.positionY.value, positionFromOldSystem.y));
    }
}
