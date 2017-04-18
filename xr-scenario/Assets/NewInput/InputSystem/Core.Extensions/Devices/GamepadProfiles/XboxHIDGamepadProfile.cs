#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#define IS_WINDOWS
#endif

using System.IO;
using System.Collections.Generic;
using UnityEngineInternal.Input;
using Assets.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR || UNITY_STANDALONE

namespace UnityEngine.Experimental.Input
{
    // Xbox One and Xbox 360 gamepad interfacing through HID.
    // Unfortunately, the Xbox controllers aren't really HIDs but rather rely on driver support to
    // act as HIDs. Most importantly that means the HID descriptor of the gamepad is *not* coming from
    // the device but rather from the driver -- which means that Microsoft's drivers and third-party
    // drivers on other platforms can and will differ.
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class XboxHIDGamepadProfile : GamepadProfile
    {
        static XboxHIDGamepadProfile()
        {
            Register();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            InputSystem.RegisterDeviceProfile<XboxHIDGamepadProfile>();
        }

        public XboxHIDGamepadProfile()
        {
            name = "Xbox Controller";
            matchingDeviceRegexes = new List<string>()
            {
                ////REVIEW: the current string thing requires you to know the order of the properties... that sucks
                "product:[.*Xbox.+Controller.*].+interface:[HID]"
            };
            lastResortDeviceRegex = "xbox";

            ControlSetup setup = GetControlSetup(new Gamepad());

            // Setup mapping.
            setup.SplitMapping(0x010030, CommonControls.leftStickLeft, CommonControls.leftStickRight);
            setup.SplitMapping(0x010031, CommonControls.leftStickUp, CommonControls.leftStickDown);

            setup.SplitMapping(0x010033, CommonControls.rightStickLeft, CommonControls.rightStickRight);
            setup.SplitMapping(0x010034, CommonControls.rightStickUp, CommonControls.rightStickDown);

#if IS_WINDOWS
            setup.Mapping(0x090001, CommonControls.action1);
            setup.Mapping(0x090002, CommonControls.action2);
            setup.Mapping(0x090003, CommonControls.action3);
            setup.Mapping(0x090004, CommonControls.action4);

            setup.Mapping(0x090005, CommonControls.leftBumper);
            setup.Mapping(0x090006, CommonControls.rightBumper);

            // Triggers are combined into a single [-1..1] range. Left is positive, right is negative.
            // At the USB level, the controller properly splits the triggers. XInput is picking it up from there.
            // Unfortunately, the MS HID driver for Xbox controllers combines them.
            setup.SplitMapping(0x010032, CommonControls.rightTrigger, CommonControls.leftTrigger);

            // The dpad is done as a HID hatswitch.
            setup.HatMapping(0x010039, CommonControls.dPadLeft, CommonControls.dPadRight, CommonControls.dPadDown, CommonControls.dPadUp);

            setup.Mapping(0x090009, CommonControls.leftStickButton);
            setup.Mapping(0x09000A, CommonControls.rightStickButton);

            setup.Mapping(0x090007, CommonControls.back);
            setup.Mapping(0x090008, CommonControls.start);
#else
            setup.Mapping(0x090001, CommonControls.action1, Range.full, Range.positive);
            setup.Mapping(0x090002, CommonControls.action2, Range.full, Range.positive);
            setup.Mapping(0x090003, CommonControls.action3, Range.full, Range.positive);
            setup.Mapping(0x090004, CommonControls.action4, Range.full, Range.positive);

            setup.Mapping(0x090005, CommonControls.leftBumper, Range.full, Range.positive);
            setup.Mapping(0x090006, CommonControls.rightBumper, Range.full, Range.positive);

            setup.Mapping(0x090007, CommonControls.leftStickButton, Range.full, Range.positive);
            setup.Mapping(0x090008, CommonControls.rightStickButton, Range.full, Range.positive);

            setup.Mapping(0x010032, CommonControls.leftTrigger, Range.full, Range.positive);
            setup.Mapping(0x010035, CommonControls.rightTrigger, Range.full, Range.positive);

            setup.Mapping(0x09000C, CommonControls.dPadUp, Range.full, Range.positive);
            setup.Mapping(0x09000D, CommonControls.dPadDown, Range.full, Range.positive);
            setup.Mapping(0x09000E, CommonControls.dPadLeft, Range.full, Range.positive);
            setup.Mapping(0x09000F, CommonControls.dPadRight, Range.full, Range.positive);

            setup.Mapping(0x09000A, CommonControls.back, Range.full, Range.positive);
            setup.Mapping(0x090009, CommonControls.start, Range.full, Range.positive);
#endif

            mappings = setup.FinishMappings();
        }

        public override ControlSetup GetControlSetup(InputDevice device)
        {
            ControlSetup setup = new ControlSetup(device);

            setup.AddControl(CommonControls.back);
            setup.AddControl(CommonControls.start);

            // Add the two additional motors on the triggers.
            var leftTriggerMotor = new AxisOutput("Left Trigger Vibration");
            var rightTriggerMotor = new AxisOutput("Right Trigger Vibration");
            setup.AddControl(SupportedControl.Get<AxisOutput>("Left Trigger Vibration"), leftTriggerMotor);
            setup.AddControl(SupportedControl.Get<AxisOutput>("Right Trigger Vibration"), rightTriggerMotor);

            // Section for control name overrides.
            setup.GetControl(CommonControls.action1).name = "A";
            setup.GetControl(CommonControls.action2).name = "B";
            setup.GetControl(CommonControls.action3).name = "X";
            setup.GetControl(CommonControls.action4).name = "Y";

            return setup;
        }
    }
}

#endif
