using UnityEngine;
using UnityEngine.Experimental.Input;

// GENERATED FILE - DO NOT EDIT MANUALLY
public class MenuActions : ActionMapInput
{
    public MenuActions(ActionMap actionMap) : base(actionMap) {}

    public ButtonControl @select { get { return (ButtonControl)GetControl(0); } }
    public ButtonControl @cancel { get { return (ButtonControl)GetControl(1); } }
    public ButtonControl @moveLeft { get { return (ButtonControl)GetControl(2); } }
    public ButtonControl @moveRight { get { return (ButtonControl)GetControl(3); } }
    public ButtonControl @moveDown { get { return (ButtonControl)GetControl(4); } }
    public ButtonControl @moveUp { get { return (ButtonControl)GetControl(5); } }
}
