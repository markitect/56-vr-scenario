using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Input;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum ButtonOption
        {
            Action1,
            Action2,
            Action3,
            Action4
        }

        public ButtonOption m_ButtonControl = ButtonOption.Action1;

        void OnEnable()
        {
            if (VirtualJoystick.current == null)
            {
                var virtualJoystick = new VirtualJoystick();
                virtualJoystick.SetupWithoutProfile();
                InputSystem.RegisterDevice(virtualJoystick);
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            VirtualJoystick joystick = VirtualJoystick.current;
            ButtonControl control;
            switch (m_ButtonControl)
            {
                case ButtonOption.Action1: control = joystick.action1; break;
                case ButtonOption.Action2: control = joystick.action2; break;
                case ButtonOption.Action3: control = joystick.action3; break;
                case ButtonOption.Action4: control = joystick.action4; break;
                default: return;
            }
            joystick.SetValue(control, 0);
        }

        public void OnPointerDown(PointerEventData data)
        {
            VirtualJoystick joystick = VirtualJoystick.current;
            ButtonControl control;
            switch (m_ButtonControl)
            {
                case ButtonOption.Action1: control = joystick.action1; break;
                case ButtonOption.Action2: control = joystick.action2; break;
                case ButtonOption.Action3: control = joystick.action3; break;
                case ButtonOption.Action4: control = joystick.action4; break;
                default: return;
            }
            joystick.SetValue(control, 1);
        }
    }
}
