using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
    // This control type has no corresponding ActionSlot since it's enough
    // for the user to access a DeltaAxisControl as an AxisControl.
    public class DeltaAxisControl : AxisControl
    {
        public DeltaAxisControl() {}
        public DeltaAxisControl(string name) : base(name) {}

        public override void AdvanceFrame()
        {
            base.AdvanceFrame();
            if (Time.inFixedTimeStep)
                m_ValueFixed = defaultValue;
            else
                m_ValueDynamic = defaultValue;
        }

        public override void SetValue(float newValue)
        {
            base.SetValue(value + newValue);
        }
    }
}
