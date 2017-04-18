using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
    public abstract class InputDevice : InputControlProvider, IInputStateProvider
    {
        private InputDeviceProfile m_Profile;
        protected Dictionary<int, int> m_SupportedControlToControlIndex = new Dictionary<int, int>();
        private int[] m_SortedCachedUsedControlHashes;

        protected InputDevice()
        {
            isConnected = true;
        }

        public void SetControls(ControlSetup setup)
        {
            SetControls(setup.controls);
            m_SupportedControlToControlIndex = setup.supportedControlIndices;
            m_SortedCachedUsedControlHashes = new int[m_SupportedControlToControlIndex.Keys.Count];
            m_SupportedControlToControlIndex.Keys.CopyTo(m_SortedCachedUsedControlHashes, 0);
            Array.Sort(m_SortedCachedUsedControlHashes);
        }

        ////REVIEW: right now the devices don't check whether the event was really meant for them; they go purely by the
        ////  type of event they receive. should they check more closely?

        public override sealed bool ProcessEvent(InputEvent inputEvent)
        {
            // If event was used, set time, but never consume event.
            // Devices don't consume events, they only track the state changes
            // they represent.
            if (ProcessEventIntoState(inputEvent, state))
                lastEventTime = inputEvent.time;
            return false;
        }

        public virtual bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
        {
            GenericControlEvent controlEvent = inputEvent as GenericControlEvent;
            if (controlEvent == null)
                return false;

            var control = intoState.controls[controlEvent.controlIndex];
            if (!control.enabled)
                return false;
            controlEvent.CopyValueToControl(control);
            return true;
        }

        public virtual void BeginUpdate()
        {
            state.BeginUpdate();
        }

        public void EndUpdate()
        {
            PostProcess();
        }

        public void PostProcess()
        {
            PostProcessState(state);
        }

        ////REVIEW: why are these public?
        public virtual void PostProcessState(InputState intoState) {}

        public virtual void PostProcessEnabledControls(InputState intoState) {}

        public virtual bool RemapEvent(InputEvent inputEvent)
        {
            if (profile != null)
                return profile.Remap(inputEvent);
            return false;
        }

        public InputState GetDeviceStateForDeviceSlotKey(int deviceKey)
        {
            // For composite bindings on InputDevices the returned state is always
            // the state of the InputDevice itself. We don't need to look at the deviceKey.
            return state;
        }

        public int GetOrAddDeviceSlotKey(InputDevice device)
        {
            return DeviceSlot.kInvalidKey;
        }

        public bool isConnected { get; internal set; }

        public InputDeviceProfile profile
        {
            get { return m_Profile; }
        }

        // The controls the device assumes are present and may have shortcut properties for.
        public abstract void AddStandardControls(ControlSetup setup);

        public void SetupWithoutProfile()
        {
            ControlSetup setup = new ControlSetup(this);
            SetControls(setup);
        }

        public virtual void SetupFromProfile(InputDeviceProfile profile)
        {
            if (profile != null)
            {
                m_Profile = profile;
                SetControls(profile.GetControlSetup(this));
            }
        }

        // Some input providers need an identifier tag when there are
        // multiple devices of the same type (e.g. left and right hands).
        public virtual int tagIndex
        {
            get { return -1; } // -1 tag means unset or "Any"
            internal set { return;  }
        }

        private string m_Name;
        public string name
        {
            get
            {
                if (!string.IsNullOrEmpty(m_Name))
                    return m_Name;

                if (profile != null)
                    return profile.name;

                return GetType().Name;
            }
            set { m_Name = value; }
        }

        public string manufacturer { get; set; }
        public string serialNumber { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(manufacturer))
                return string.Format("{0} {1}", manufacturer, name);
            return name;
        }

        public InputControl GetControl(SupportedControl supportedControl)
        {
            return GetControlFromHash(supportedControl.hash);
        }

        public override int GetControlIndexFromHash(int hash)
        {
            int controlIndex = -1;
            m_SupportedControlToControlIndex.TryGetValue(hash, out controlIndex);
            return controlIndex;
        }

        public override int GetHashForControlIndex(int controlIndex)
        {
            foreach (var kvp in m_SupportedControlToControlIndex)
            {
                if (kvp.Value == controlIndex)
                    return kvp.Key;
            }
            return -1;
        }

        public int GetSupportScoreForSupportedControlHashes(List<int> neededHashes)
        {
            int currentNeededIndex = 0;
            int currentProvidedIndex = 0;
            int score = 0;
            bool finished = false;
            while (!finished)
            {
                if (currentNeededIndex >= neededHashes.Count || currentProvidedIndex >= m_SortedCachedUsedControlHashes.Length)
                {
                    score -= (neededHashes.Count - currentNeededIndex);
                    break;
                }

                int currentNeeded = neededHashes[currentNeededIndex];
                int currentProvided = m_SortedCachedUsedControlHashes[currentProvidedIndex];
                if (currentNeeded == currentProvided)
                {
                    currentNeededIndex++;
                    currentProvidedIndex++;
                    continue;
                }
                if (currentNeeded < currentProvided)
                {
                    currentNeededIndex++;
                    score--;
                    continue;
                }

                currentProvidedIndex++;
            }
            return score;
        }

        public int nativeId { get; internal set; }
    }
}
