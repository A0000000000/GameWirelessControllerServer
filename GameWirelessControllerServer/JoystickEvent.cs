using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWirelessControllerServer
{
    public class JoystickEvent
    {
        public int LargeMotor
        {
            get;
            set;
        }
        public int SmallMotor
        {
            get; 
            set;
        }
        public int LedNumber
        {
            get; 
            set;
        }

        public JoystickEvent(int largeMotor, int smallMotor, int ledNumber)
        {
            LargeMotor = largeMotor;
            SmallMotor = smallMotor;
            LedNumber = ledNumber;
        }
    }
}
