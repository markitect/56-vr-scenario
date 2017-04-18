using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Utilities;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
    public class VirtualJoystick : InputDevice
    {
        public static VirtualJoystick current { get { return InputSystem.GetCurrentDeviceOfType<VirtualJoystick>(); } }

        public override void AddStandardControls(ControlSetup setup)
        {
            leftStickX = (AxisControl)setup.AddControl(CommonControls.leftStickX);
            leftStickY = (AxisControl)setup.AddControl(CommonControls.leftStickY);
            leftStick = (Vector2Control)setup.AddControl(CommonControls.leftStick);

            rightStickX = (AxisControl)setup.AddControl(CommonControls.rightStickX);
            rightStickY = (AxisControl)setup.AddControl(CommonControls.rightStickY);
            rightStick = (Vector2Control)setup.AddControl(CommonControls.rightStick);

            action1 = (ButtonControl)setup.AddControl(CommonControls.action1);
            action2 = (ButtonControl)setup.AddControl(CommonControls.action2);
            action3 = (ButtonControl)setup.AddControl(CommonControls.action3);
            action4 = (ButtonControl)setup.AddControl(CommonControls.action4);
        }

        public AxisControl leftStickX { get; private set; }
        public AxisControl leftStickY { get; private set; }
        public Vector2Control leftStick { get; private set; }

        public AxisControl rightStickX { get; private set; }
        public AxisControl rightStickY { get; private set; }
        public Vector2Control rightStick { get; private set; }

        public ButtonControl action1 { get; private set; }
        public ButtonControl action2 { get; private set; }
        public ButtonControl action3 { get; private set; }
        public ButtonControl action4 { get; private set; }

        public override void PostProcessState(InputState state)
        {
            ((Vector2Control)state.controls[leftStick.index]).value = new Vector2(
                    ((AxisControl)state.controls[leftStickX.index]).value,
                    ((AxisControl)state.controls[leftStickY.index]).value);
        }

        public void SetValue<T>(InputControl<T> control, T value)
        {
            T currentValue = control.value;
            if (value.Equals(currentValue))
                return;

            var inputEvent = InputSystem.CreateEvent<GenericControlEvent<T>>();
            inputEvent.device = this;
            inputEvent.controlIndex = control.index;
            inputEvent.value = value;
            InputSystem.QueueEvent(inputEvent);
        }
    }
}
