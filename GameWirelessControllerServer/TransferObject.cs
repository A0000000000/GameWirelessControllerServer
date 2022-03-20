using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GameControllerServiceClient;

namespace GameWirelessControllerServer
{
    public class TransferObject
    {
        public static readonly int TYPE_GAME_EVENT = 0;
        public static readonly int TYPE_JOYSTICK_EVENT = 1;

        public GameEvent GameEvent
        {
            get;
            set;
        }
        public JoystickEvent JoystickEvent
        {
            get;
            set;
        }

        public int Type
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }

        public static TransferObject CreateJoystickEvent(JoystickEvent joystickEvent)
        {
            return new TransferObject()
            {
                Type = TYPE_JOYSTICK_EVENT,
                JoystickEvent = joystickEvent
            };
        }

    }
}
