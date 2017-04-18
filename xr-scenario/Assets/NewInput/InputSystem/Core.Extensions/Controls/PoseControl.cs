using System;
using UnityEngine;

namespace UnityEngine.Experimental.Input
{
    // Note corresponding ActionSlot in Players/ActionSlots folder.
    public class PoseControl : InputControl<Pose>
    {
        public PoseControl()
        {
            defaultValue = Pose.identity;
            SetValue(defaultValue);
        }

        public PoseControl(string name)
            : this()
        {
            this.name = name;
        }
    }
}
