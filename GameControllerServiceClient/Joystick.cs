using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace GameControllerServiceClient
{
    public class Joystick: IDisposable
    {
        public static readonly int TYPE_XBOX360 = 0;

        private static readonly ViGEmClient VIGEMCLIENT = new ViGEmClient();

        private static int ABS_MAX_ROCKER = 32767;
        private static int ABS_MAX_TRIGGER = 255;

        private String id;
        private int type;
        private IXbox360Controller xbox360Controller;

        public Joystick(String id, int type)
        {
            this.id = id;
            this.type = type;
            if (type == TYPE_XBOX360)
            {
                xbox360Controller = VIGEMCLIENT.CreateXbox360Controller();
            }
        }

        public static Joystick CreateXbox360Controller(String id)
        {
            return new Joystick(id, TYPE_XBOX360);
        }

        public void Init()
        {
            xbox360Controller.Connect();
            xbox360Controller.FeedbackReceived += Xbox360Controller_FeedbackReceived;
        }

        public void SendControllerEvent(GameEvent ev)
        {
            switch (ev.EventType)
            {
                case Xbox360Type.KEY:
                    KeyEvent(ev.Type, ev.Status);
                    break;
                case Xbox360Type.AXIS:
                    RockerEvent(ev.Type, ev.X, ev.Y);
                    break;
                case Xbox360Type.TRIGGER:
                    TriggerEvent(ev.Type, ev.Value);
                    break;
            }
        }


        private void TriggerEvent(int type, int value)
        {
            byte val = (byte)(value * ABS_MAX_TRIGGER / 100);
            xbox360Controller.SetSliderValue(Xbox360Type.TriggerType.ToXbox360Slider(type), val);
        }
        private void RockerEvent(int type, int x, int y)
        {
            short xVal = (short)(x * ABS_MAX_ROCKER / 100);
            short yVal = (short)(y * ABS_MAX_ROCKER / 100);
            switch (type)
            {
                case Xbox360Type.AxisType.LEFT_ROCKER:
                    xbox360Controller.SetAxisValue(Xbox360Type.AxisType.ToXbox360Axis(Xbox360Type.AxisType.LEFT_ROCKER_X), xVal);
                    xbox360Controller.SetAxisValue(Xbox360Type.AxisType.ToXbox360Axis(Xbox360Type.AxisType.LEFT_ROCKER_Y), yVal);
                    break;
                case Xbox360Type.AxisType.RIGHT_ROCKER:
                    xbox360Controller.SetAxisValue(Xbox360Type.AxisType.ToXbox360Axis(Xbox360Type.AxisType.RIGHT_ROCKER_X), xVal);
                    xbox360Controller.SetAxisValue(Xbox360Type.AxisType.ToXbox360Axis(Xbox360Type.AxisType.RIGHT_ROCKER_Y), yVal);
                    break;
            }
        }
        private void KeyEvent(int type, int status)
        {
            if (type == Xbox360Type.KeyType.FUNCTION)
            {
                if (OnFunctionPress != null)
                {
                    OnFunctionPress(status);
                }
            }
            else
            {
                xbox360Controller.SetButtonState(Xbox360Type.KeyType.ToXbox360Button(type), status == 0);
            }
        }


        public Action<int, int, int> Xbox360FeedbackReceived
        {
            get;
            set;
        }

        public Action<int> OnFunctionPress
        {
            get;
            set;
        }

        private void Xbox360Controller_FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            if (Xbox360FeedbackReceived != null)
            {
                Xbox360FeedbackReceived(e.LargeMotor, e.SmallMotor, e.LedNumber);
            }
        }

        public void Dispose()
        {
            xbox360Controller?.Disconnect();
            xbox360Controller = null;
        }

        public String Id
        {
            get => id;
            set => id = value;
        }

    }
}
