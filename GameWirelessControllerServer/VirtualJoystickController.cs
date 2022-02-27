using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace GameWirelessControllerServer
{
    class VirtualJoystickController: IDisposable
    {

        private static VirtualJoystickController INSTANCE = null;
        private static readonly object LOCK = new object();
        private static int ABS_MAX_ROCKER = 32767;
        private static int ABS_MAX_TRIGGER = 255;

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

        private ViGEmClient client;
        IXbox360Controller controller;


        public void InitJoystick()
        {
            client = new ViGEmClient();
            controller = client.CreateXbox360Controller();
            controller.Connect();
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
                switch(keyType)
                {
                    case KeyType.LEFT_ROCKER:
                        controller.SetButtonState(Xbox360Button.LeftThumb, action == "DOWN");
                        break;
                    case KeyType.RIGHT_ROCKER:
                        controller.SetButtonState(Xbox360Button.RightThumb, action == "DOWN");
                        break;
                }
            }
            if (type == "move")
            {
                int x = int.Parse(data["x"]?.ToString() ?? "0");
                int y = int.Parse(data["y"]?.ToString() ?? "0");
                x = ABS_MAX_ROCKER * x / 100;
                y = ABS_MAX_ROCKER * y / 100;
                if (action == "UP")
                {
                    x = 0;
                    y = 0;
                }
                switch (keyType)
                {
                    case KeyType.LEFT_ROCKER:
                        controller.SetAxisValue(Xbox360Axis.LeftThumbX, short.Parse(x.ToString()));
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, short.Parse(y.ToString()));
                        break;
                    case KeyType.RIGHT_ROCKER:
                        controller.SetAxisValue(Xbox360Axis.RightThumbX, short.Parse(x.ToString()));
                        controller.SetAxisValue(Xbox360Axis.RightThumbY, short.Parse(y.ToString()));
                        break;
                }

            }
        }

        public void DealCrossKey(KeyType type, string action)
        {
           switch(type)
            {
                case KeyType.CROSS_LEFT:
                    controller.SetButtonState(Xbox360Button.Left, action == "DOWN");
                    break;
                case KeyType.CROSS_BOTTOM:
                    controller.SetButtonState(Xbox360Button.Down, action == "DOWN");
                    break;
                case KeyType.CROSS_RIGHT:
                    controller.SetButtonState(Xbox360Button.Right, action == "DOWN");
                    break;
                case KeyType.CROSS_TOP:
                    controller.SetButtonState(Xbox360Button.Up, action == "DOWN");
                    break;
            }
        }

        public void DealABXY(KeyType type, string action)
        {
            switch (type)
            {
                case KeyType.A:
                    controller.SetButtonState(Xbox360Button.A, action == "DOWN");
                    break;
                case KeyType.B:
                    controller.SetButtonState(Xbox360Button.B, action == "DOWN");
                    break;
                case KeyType.X:
                    controller.SetButtonState(Xbox360Button.X, action == "DOWN");
                    break;
                case KeyType.Y:
                    controller.SetButtonState(Xbox360Button.Y, action == "DOWN");
                    break;
            }
        }

        public void DealTrigger(KeyType type, string action, Dictionary<string, object> data)
        {
            int trigger = int.Parse(data["trigger"]?.ToString() ?? "0");
            trigger = ABS_MAX_TRIGGER * trigger / 100;
            if (action == "UP")
            {
                trigger = 0;
            }
            switch (type)
            {
                case KeyType.LEFT_TRIGGER:
                    controller.SetSliderValue(Xbox360Slider.LeftTrigger, byte.Parse(trigger.ToString()));
                    break;
                case KeyType.RIGHT_TRIGGER:
                    controller.SetSliderValue(Xbox360Slider.RightTrigger, byte.Parse(trigger.ToString()));
                    break;
            }

        }

        public void DealButton(KeyType type, string action)
        {
            switch (type)
            {
                case KeyType.LEFT_BUTTON:
                    controller.SetButtonState(Xbox360Button.LeftShoulder, action == "DOWN");
                    break;
                case KeyType.RIGHT_BUTTON:
                    controller.SetButtonState(Xbox360Button.RightShoulder, action == "DOWN");
                    break;
                case KeyType.MENU:
                    controller.SetButtonState(Xbox360Button.Start, action == "DOWN");
                    break;
                case KeyType.VIEW:
                    controller.SetButtonState(Xbox360Button.Back, action == "DOWN");
                    break;
            }
        }

        public void DealAddition(KeyType type, string action)
        {
           
        }

        public void Dispose()
        {
            try
            {
                controller?.Disconnect();
                client?.Dispose();
            } catch(Exception e) {}
            finally
            {
                controller = null;
                client = null;
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
