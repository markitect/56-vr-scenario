using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.Input
{
    public class HeadMountedDisplay : TrackedInputDevice
    {
        internal enum Node
        {
            LeftEye = 1,
            RightEye = 2,
            CenterEye = 3,
            Head = 6,
            LeftHand = 4,
            RightHand = 5,
        }

        public override void AddStandardControls(ControlSetup setup)
        {
            base.AddStandardControls(setup);

            setup.GetControl(CommonControls.pose).name = "Head Pose";

            leftEyePosition = (Vector3Control)setup.AddControl(SupportedControl.Get<Vector3Control>("Left Eye Position"));
            leftEyeRotation = (QuaternionControl)setup.AddControl(SupportedControl.Get<QuaternionControl>("Left Eye Rotation"));
            leftEyePose = (PoseControl)setup.AddControl(SupportedControl.Get<PoseControl>("Left Eye Pose"));

            rightEyePosition = (Vector3Control)setup.AddControl(SupportedControl.Get<Vector3Control>("Right Eye Position"));
            rightEyeRotation = (QuaternionControl)setup.AddControl(SupportedControl.Get<QuaternionControl>("Right Eye Rotation"));
            rightEyePose = (PoseControl)setup.AddControl(SupportedControl.Get<PoseControl>("Right Eye Pose"));

            centerEyePosition = (Vector3Control)setup.AddControl(SupportedControl.Get<Vector3Control>("Center Eye Position"));
            centerEyeRotation = (QuaternionControl)setup.AddControl(SupportedControl.Get<QuaternionControl>("Center Eye Rotation"));
            centerEyePose = (PoseControl)setup.AddControl(SupportedControl.Get<PoseControl>("Center Eye Pose"));
        }

        public static HeadMountedDisplay current { get { return InputSystem.GetCurrentDeviceOfType<HeadMountedDisplay>(); } }

        public override bool ProcessEventIntoState(InputEvent inputEvent, InputState intoState)
        {
            var consumed = false;

            var trackingEvent = inputEvent as TrackingEvent;
            if (trackingEvent != null)
            {
                Pose pose = new Pose()
                {
                    rotation = trackingEvent.localRotation,
                    translation = trackingEvent.localPosition
                };

                switch (trackingEvent.nodeId)
                {
                    // Head is handled by the base class.
                    /*case (int)Node.Head:
                        consumed |= intoState.SetCurrentValue(headPosition.index, trackingEvent.localPosition);
                        consumed |= intoState.SetCurrentValue(headRotation.index, trackingEvent.localRotation);
                        consumed |= intoState.SetCurrentValue(headPose.index, pose);
                        break;*/
                    case (int)Node.LeftEye:
                        consumed |= intoState.SetCurrentValue(leftEyePosition.index, trackingEvent.localPosition);
                        consumed |= intoState.SetCurrentValue(leftEyeRotation.index, trackingEvent.localRotation);
                        consumed |= intoState.SetCurrentValue(leftEyePose.index, pose);
                        break;
                    case (int)Node.RightEye:
                        consumed |= intoState.SetCurrentValue(rightEyePosition.index, trackingEvent.localPosition);
                        consumed |= intoState.SetCurrentValue(rightEyeRotation.index, trackingEvent.localRotation);
                        consumed |= intoState.SetCurrentValue(rightEyePose.index, pose);
                        break;
                    case (int)Node.CenterEye:
                        consumed |= intoState.SetCurrentValue(centerEyePosition.index, trackingEvent.localPosition);
                        consumed |= intoState.SetCurrentValue(centerEyeRotation.index, trackingEvent.localRotation);
                        consumed |= intoState.SetCurrentValue(centerEyePose.index, pose);
                        break;
                }
            }

            if (!consumed)
                consumed = base.ProcessEventIntoState(inputEvent, intoState);

            return consumed;
        }

        public Vector3Control headPosition { get { return position; } }
        public QuaternionControl headRotation { get { return rotation; } }
        public PoseControl headPose { get { return pose; } }

        public Vector3Control leftEyePosition { get; private set; }
        public QuaternionControl leftEyeRotation { get; private set; }
        public PoseControl leftEyePose { get; private set; }

        public Vector3Control rightEyePosition { get; private set; }
        public QuaternionControl rightEyeRotation { get; private set; }
        public PoseControl rightEyePose { get; private set; }

        public Vector3Control centerEyePosition { get; private set; }
        public QuaternionControl centerEyeRotation { get; private set; }
        public PoseControl centerEyePose { get; private set; }
    }
}
