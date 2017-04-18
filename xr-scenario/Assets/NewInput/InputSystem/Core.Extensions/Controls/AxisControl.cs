using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
    // Note corresponding ActionSlot in Players/ActionSlots folder.
    public class AxisControl : InputControl<float>
    {
        public AxisControl() {}
        public AxisControl(string name)
        {
            this.name = name;
        }

        public override float GetCombinedValue(float[] values)
        {
            float value = 0;
            for (int i = 0; i < values.Length; i++)
            {
                var current = values[i];
                if (Mathf.Abs(current) > Mathf.Abs(value))
                    value = current;
            }
            return value;
        }
    }
}
