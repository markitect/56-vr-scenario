using UnityEngine;
using UnityEngine.Experimental.Input;

// GENERATED FILE - DO NOT EDIT MANUALLY
public class FirstPersonControls : ActionMapInput
{
    public FirstPersonControls(ActionMap actionMap) : base(actionMap) {}

    public AxisControl @moveX { get { return (AxisControl)GetControl(0); } }
    public AxisControl @moveY { get { return (AxisControl)GetControl(1); } }
    public Vector2Control @move { get { return (Vector2Control)GetControl(2); } }
    public AxisControl @lookX { get { return (AxisControl)GetControl(3); } }
    public AxisControl @lookY { get { return (AxisControl)GetControl(4); } }
    public Vector2Control @look { get { return (Vector2Control)GetControl(5); } }
    public ButtonControl @fire { get { return (ButtonControl)GetControl(6); } }
    public ButtonControl @menu { get { return (ButtonControl)GetControl(7); } }
    public ButtonControl @lockCursor { get { return (ButtonControl)GetControl(8); } }
    public ButtonControl @unlockCursor { get { return (ButtonControl)GetControl(9); } }
    public ButtonControl @reconfigure { get { return (ButtonControl)GetControl(10); } }
}
