using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Input;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class VirtualStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public enum AxisOption
        {
            // Options for which axes to use
            Both, // Use both
            OnlyHorizontal, // Only horizontal
            OnlyVertical // Only vertical
        }

        public enum StickOption
        {
            LeftStick,
            RightStick
        }

        public int m_MovementRange = 100;
        public AxisOption m_AxesToUse = AxisOption.Both; // The options for the axes that the still will use
        public StickOption m_StickChoice = StickOption.LeftStick;

        Vector3 m_StartPos;
        Vector2 m_PointerDownPos;
        bool m_UseX; // Toggle for using the x axis
        bool m_UseY; // Toggle for using the Y axis
        Camera m_EventCamera;

        void OnEnable()
        {
            if (VirtualJoystick.current == null)
            {
                var virtualJoystick = new VirtualJoystick();
                virtualJoystick.SetupWithoutProfile();
                InputSystem.RegisterDevice(virtualJoystick);
            }

            CreateVirtualAxes();
        }

        void Start()
        {
            m_StartPos = (transform as RectTransform).anchoredPosition;
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas)
                m_EventCamera = canvas.worldCamera;
        }

        void UpdateVirtualAxes(Vector3 delta)
        {
            VirtualJoystick joystick = VirtualJoystick.current;

            if (m_UseX)
            {
                AxisControl control = (m_StickChoice == StickOption.LeftStick ? joystick.leftStickX : joystick.rightStickX);
                joystick.SetValue(control, delta.x);
            }

            if (m_UseY)
            {
                AxisControl control = (m_StickChoice == StickOption.LeftStick ? joystick.leftStickY : joystick.rightStickY);
                joystick.SetValue(control, delta.y);
            }
        }

        void CreateVirtualAxes()
        {
            // Set axes to use
            m_UseX = (m_AxesToUse == AxisOption.Both || m_AxesToUse == AxisOption.OnlyHorizontal);
            m_UseY = (m_AxesToUse == AxisOption.Both || m_AxesToUse == AxisOption.OnlyVertical);
        }

        public void OnPointerDown(PointerEventData data)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, data.position, m_EventCamera, out m_PointerDownPos);
        }

        public void OnDrag(PointerEventData data)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, data.position, m_EventCamera, out position);
            Vector2 delta = position - m_PointerDownPos;

            if (!m_UseX)
                delta.x = 0;

            if (!m_UseY)
                delta.y = 0;

            delta = Vector2.ClampMagnitude(delta, m_MovementRange);

            (transform as RectTransform).anchoredPosition = m_StartPos + (Vector3)delta;
            UpdateVirtualAxes(delta / m_MovementRange);
        }

        public void OnPointerUp(PointerEventData data)
        {
            (transform as RectTransform).anchoredPosition = m_StartPos;
            UpdateVirtualAxes(Vector2.zero);
        }
    }
}
