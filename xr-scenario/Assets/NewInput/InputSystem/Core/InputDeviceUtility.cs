using UnityEngine;
using UnityEngine.Experimental.Input;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace UnityEngine.Experimental.Input
{
    public struct TypePair
    {
        public Type deviceType;
        public Type controlType;
        public TypePair(Type device, Type control) { deviceType = device; controlType = control; }
    }

    public static class InputDeviceUtility
    {
        static Dictionary<TypePair, List<DomainEntry>> s_DeviceControlEntriesOfType =
            new Dictionary<TypePair, List<DomainEntry>>();

        static List<DomainEntry> s_EmptyList = new List<DomainEntry>();

        static void InitializeDeviceControlInfoOfType(Type deviceType, TypePair key)
        {
            List<DomainEntry> entries = new List<DomainEntry>();

            // This will get all the supported controls for a device type,
            // including non-standard ones that are only supplied by certain device profiles.
            var dict = InputSystem.GetSupportedControlsForDeviceType(deviceType);

            // This will give us a dummy instance of the device.
            // We want to look up the names here, so that if it renames any controls,
            // we get the renamed name rather than the standardized name.
            InputDevice device = Activator.CreateInstance(deviceType) as InputDevice;
            device.SetupWithoutProfile();

            foreach (var kvp in dict)
            {
                if (key.controlType.IsAssignableFrom(kvp.Key.controlType.value))
                {
                    string usedName = kvp.Key.standardName;
                    InputControl control = device.GetControlFromHash(kvp.Key.hash);
                    if (control != null)
                        usedName = control.name;

                    entries.Add(new DomainEntry()
                    {
                        name = usedName,
                        hash = kvp.Key.hash,
                        standardized = kvp.Value
                    });
                }
            }

            s_DeviceControlEntriesOfType[key] = entries;
        }

        public static List<DomainEntry> GetDeviceControlEntriesOfType(Type deviceType, Type controlType)
        {
            if (deviceType == null || controlType == null)
                return s_EmptyList;
            var key = new TypePair(deviceType, controlType);
            List<DomainEntry> entries;
            if (!s_DeviceControlEntriesOfType.TryGetValue(key, out entries))
            {
                InitializeDeviceControlInfoOfType(deviceType, key);
                return s_DeviceControlEntriesOfType[key];
            }
            return entries;
        }

        public static string[] GetDeviceTags(Type type)
        {
            string[] tags = null;
            PropertyInfo info = type.GetProperty("Tags", BindingFlags.Static | BindingFlags.Public);
            if (info != null)
            {
                tags = (string[])info.GetValue(null, null);
            }
            return tags;
        }
    }
}
