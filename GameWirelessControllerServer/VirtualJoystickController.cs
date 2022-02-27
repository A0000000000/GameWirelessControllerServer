using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace GameWirelessControllerServer
{
    class VirtualJoystickController
    {

        private static VirtualJoystickController INSTANCE = null;
        private static readonly object LOCK = new object();

        public static VirtualJoystickController GetInstance()
        {
            if (INSTANCE == null)
            {
                lock (LOCK)
                {
                    if (INSTANCE == null)
                    {
                        INSTANCE = new VirtualJoystickController();
                    }
                }
            }
            return INSTANCE;
        }

        private vJoy joystick = new vJoy();
        private readonly uint id = 1;

        public void InitJoystick()
        {
            joystick.AcquireVJD(id);
            joystick.ResetVJD(id);
            joystick.ResetVJD(id);
        }

        public bool Enable()
        {
            return joystick.vJoyEnabled();
        }

        public bool CheckConfiguration()
        {
            bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            bool AxisSL0 = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_SL0);
            int btnCount = joystick.GetVJDButtonNumber(id);
            int discNum = joystick.GetVJDDiscPovNumber(id);
            return true || AxisX && AxisY && AxisZ && AxisRZ && AxisSL0 && btnCount >= 10 && discNum >= 1;
        }

        public bool DriverMatch(ref uint dllVer, ref uint drvVer)
        {
            return joystick.DriverMatch(ref dllVer, ref drvVer);
        }

        public string GetvJoyManufacturer()
        {
            return joystick.GetvJoyManufacturerString();
        }

        public string GetvJoyProduct()
        {
            return joystick.GetvJoyProductString();
        }

        public void ResetInput()
        {

        }

        public void EventReciver(Dictionary<string, object> data)
        {
            if (data == null)
            {
                return;
            }
            string keyType = data["KeyType"]?.ToString();
            string action = data["Action"]?.ToString();
            if (keyType == null || action == null)
            {
                return;
            }
            KeyType type = (KeyType)Enum.Parse(typeof(KeyType), keyType);
            switch(type)
            {
                case KeyType.LEFT_ROCKER:
                case KeyType.RIGHT_ROCKER:
                    DealRocker(type, action, data);
                    break;
                case KeyType.CROSS_TOP:
                case KeyType.CROSS_RIGHT:
                case KeyType.CROSS_BOTTOM:
                case KeyType.CROSS_LEFT:
                    DealCrossKey(type, action);
                    break;
                case KeyType.A:
                case KeyType.B:
                case KeyType.X:
                case KeyType.Y:
                    DealABXY(type, action);
                    break;
                case KeyType.LEFT_TRIGGER:
                case KeyType.RIGHT_TRIGGER:
                    DealTrigger(type, action, data);
                    break;
                case KeyType.LEFT_BUTTON:
                case KeyType.RIGHT_BUTTON:
                case KeyType.VIEW:
                case KeyType.MENU:
                    DealButton(type, action);
                    break;
                case KeyType.MAIN:
                case KeyType.FUNCTION:
                    DealAddition(type, action);
                    break;
            }
           
        }


        public void DealRocker(KeyType keyType, string action, Dictionary<string, object> data)
        {
            string type = data["type"]?.ToString();
            if (type == null)
            {
                return;
            }
            if (type == "click")
            {
                uint keyCode = 0;
                if (keyType == KeyType.LEFT_ROCKER)
                {
                    keyCode = 9;
                }
                if (keyType == KeyType.RIGHT_ROCKER)
                {
                    keyCode = 10;
                }
                if (keyCode == 0)
                {
                    return;
                }
                if (action == "DOWN")
                {
                    joystick.SetBtn(true, id, keyCode);
                }
                if (action == "UP")
                {
                    joystick.SetBtn(false, id, keyCode);
                }
            }
            if (type == "move")
            {
                int x = int.Parse(data["x"]?.ToString() ?? "0");
                int y = -int.Parse(data["y"]?.ToString() ?? "0");
                x = x + 65535;
                x /= 4;
                y = y + 65535;
                y /= 4;
                if (action == "UP")
                {
                    x = 65535 / 4;
                    y = 65535 / 4;
                }
                if (keyType == KeyType.LEFT_ROCKER)
                {
                    joystick.SetAxis(x, id, HID_USAGES.HID_USAGE_X);
                    joystick.SetAxis(y, id, HID_USAGES.HID_USAGE_Y);
                }
                if (keyType == KeyType.RIGHT_ROCKER)
                {
                    joystick.SetAxis(x, id, HID_USAGES.HID_USAGE_Z);
                    joystick.SetAxis(y, id, HID_USAGES.HID_USAGE_RZ);
                }
            }
        }

        public void DealCrossKey(KeyType type, string action)
        {
            if (action == "MOVE")
            {
                return;
            }
            if (action == "UP")
            {
                joystick.SetDiscPov(-1, id, 1);
                return;
            }
            switch (type)
            {
                case KeyType.CROSS_TOP:
                    joystick.SetDiscPov(0, id, 1);
                    break;
                case KeyType.CROSS_LEFT:
                    joystick.SetDiscPov(3, id, 1);
                    break;
                case KeyType.CROSS_BOTTOM:
                    joystick.SetDiscPov(2, id, 1);
                    break;
                case KeyType.CROSS_RIGHT:
                    joystick.SetDiscPov(1, id, 1);
                    break;
                default:
                    joystick.SetDiscPov(-1, id, 1);
                    break;
            }
        }

        public void DealABXY(KeyType type, string action)
        {
            if (action == "MOVE")
            {
                return;
            }
            bool down = (action != "UP");
            switch(type)
            {
                case KeyType.A:
                    joystick.SetBtn(down, id, 1);
                    break;
                case KeyType.B:
                    joystick.SetBtn(down, id, 2);
                    break;
                case KeyType.X:
                    joystick.SetBtn(down, id, 3);
                    break;
                case KeyType.Y:
                    joystick.SetBtn(down, id, 4);
                    break;
            }
        }

        public void DealTrigger(KeyType type, string action, Dictionary<string, object> data)
        {
            int trigger = int.Parse(data["trigger"]?.ToString() ?? "0");
            trigger = 32767  * trigger / 100;
            if (action == "UP")
            {
                trigger = 32767 / 2;
            }
            else
            {
                if (type == KeyType.LEFT_TRIGGER)
                {
                    trigger = 32767 / 2 + trigger;
                }
                if (type == KeyType.RIGHT_TRIGGER)
                {
                    trigger = 32767 / 2 - trigger;
                }
            }
            joystick.SetAxis(trigger, id, HID_USAGES.HID_USAGE_SL0);

        }

        public void DealButton(KeyType type, string action)
        {
            if (action == "MOVE")
            {
                return;
            }
            bool down = (action != "UP");
            switch(type)
            {
                case KeyType.LEFT_BUTTON:
                    joystick.SetBtn(down, id, 5);
                    break;
                case KeyType.RIGHT_BUTTON:
                    joystick.SetBtn(down, id, 6);
                    break;
                case KeyType.VIEW:
                    joystick.SetBtn(down, id, 7);
                    break;
                case KeyType.MENU:
                    joystick.SetBtn(down, id, 8);
                    break;
            }

        }

        public void DealAddition(KeyType type, string action)
        {
            if (action == "MOVE")
            {
                return;
            }
            bool down = (action != "UP");
            switch (type)
            {
                case KeyType.MAIN:
                    joystick.SetBtn(down, id, 11);
                    break;
                case KeyType.FUNCTION:
                    joystick.SetBtn(down, id, 12);
                    break;
            }
        }

    }

    enum KeyType
    {
        LEFT_BUTTON,
        RIGHT_BUTTON,
        LEFT_TRIGGER,
        RIGHT_TRIGGER,
        LEFT_ROCKER,
        RIGHT_ROCKER,
        CROSS_TOP,
        CROSS_LEFT,
        CROSS_RIGHT,
        CROSS_BOTTOM,
        X,
        Y,
        A,
        B,
        VIEW,
        MENU,
        MAIN,
        FUNCTION
    }
}
