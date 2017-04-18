namespace UnityEngine.Experimental.Input
{
    // A combination of gamepad-style controls with motion tracking.
    // We mandate it has at least a "fire" button in addition to tracking.
    // Example: Oculus Touch, OpenVR controllers
    public class TrackedController : TrackedInputDevice
    {
        internal enum Tag
        {
            Left,
            Right
        }

        // Default constructor without parameters must be present so class can be created using Activator.
        // Having constructors with default values for arguments is not sufficient and will fail instantiation.
        public TrackedController() : this(-1) {}

        ////REVIEW: this fixed assignment probably won't be good enough; seems like we have to support controllers changing roles on the fly
        public TrackedController(int tagIndex)
        {
            m_TagIndex = tagIndex;
        }

        public override void AddStandardControls(ControlSetup setup)
        {
            base.AddStandardControls(setup);
            trigger = (ButtonControl)setup.AddControl(CommonControls.trigger);
        }

        public ButtonControl trigger { get; private set; }

        [SerializeField]
        private int m_TagIndex;
        public override int tagIndex
        {
            get { return m_TagIndex; }
            internal set { m_TagIndex = value; }
        }

        private static string[] s_Tags = new[] { "Left", "Right" };
        public static string[] Tags
        {
            get { return s_Tags; }
        }

        ////TODO: implement speedier lookups rather than crawling through all devices looking for left and right
        public static TrackedController leftHand
        {
            get
            {
                for (int i = 0; i < InputSystem.devices.Count; i++)
                {
                    if (InputSystem.devices[i] is TrackedController && InputSystem.devices[i].tagIndex == 0)
                        return (TrackedController)InputSystem.devices[i];
                }
                return null;
            }
        }

        public static TrackedController rightHand
        {
            get
            {
                for (int i = 0; i < InputSystem.devices.Count; i++)
                {
                    if (InputSystem.devices[i] is TrackedController && InputSystem.devices[i].tagIndex == 1)
                        return (TrackedController)InputSystem.devices[i];
                }
                return null;
            }
        }
    }
}
