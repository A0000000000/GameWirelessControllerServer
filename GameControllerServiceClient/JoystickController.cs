using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameControllerServiceClient
{
    public class JoystickController
    {
        private static volatile JoystickController INSTANCE;
        public static JoystickController Instance
        {
            get
            {
                if (INSTANCE == null)
                {
                    lock (typeof(JoystickController))
                    {
                        if (INSTANCE == null)
                        {
                            INSTANCE = new JoystickController();
                        }
                    }
                }
                return INSTANCE;
            }
        }

        public Joystick GetJoystick(string id, int type)
        {
            return new Joystick(id, type);
        }

    }
}
