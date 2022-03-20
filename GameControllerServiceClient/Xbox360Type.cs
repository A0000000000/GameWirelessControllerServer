using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace GameControllerServiceClient
{
    public static class Xbox360Type
    {

        public const int KEY = 0;
        public const int AXIS = 1;
        public const int TRIGGER = 2;

        public static class KeyType
        {
            public const int A = 0;
            public const int B = 1;
            public const int X = 2;
            public const int Y = 3;
                   
            public const int LEFT = 4;
            public const int TOP = 5;
            public const int RIGHT = 6;
            public const int BOTTOM = 7;
                   
            public const int LEFT_BUTTON = 8;
            public const int RIGHT_BUTTON = 9;
            public const int RIGHT_ROCKER = 10;
            public const int LEFT_ROCKER = 11;
                   
            public const int MENU = 12;
            public const int VIEW = 13;
            public const int MAIN = 14;
            public const int FUNCTION = 15;

            public static Xbox360Button ToXbox360Button(int type)
            {
                switch (type)
                {
                    case A:
                        return Xbox360Button.A;
                    case B:
                        return Xbox360Button.B;
                    case X:
                        return Xbox360Button.X;
                    case Y:
                        return Xbox360Button.Y;
                    case LEFT:
                        return Xbox360Button.Left;
                    case RIGHT:
                        return Xbox360Button.Right;
                    case TOP:
                        return Xbox360Button.Up;
                    case BOTTOM:
                        return Xbox360Button.Down;
                    case LEFT_BUTTON:
                        return Xbox360Button.LeftShoulder;
                    case RIGHT_BUTTON:
                        return Xbox360Button.RightShoulder;
                    case RIGHT_ROCKER:
                        return Xbox360Button.RightThumb;
                    case LEFT_ROCKER:
                        return Xbox360Button.LeftThumb;
                    case MENU:
                        return Xbox360Button.Start;
                    case VIEW:
                        return Xbox360Button.Back;
                    case MAIN:
                        return Xbox360Button.Guide;
                }
                throw new Exception($"No such type. type = ${type}");
            }

        }


        public static class AxisType
        {
            public const int LEFT_ROCKER = 0;
            public const int RIGHT_ROCKER = 1;
            public const int LEFT_ROCKER_X = 2;
            public const int LEFT_ROCKER_Y = 3;
            public const int RIGHT_ROCKER_X = 4;
            public const int RIGHT_ROCKER_Y = 5;

            public static Xbox360Axis ToXbox360Axis(int type)
            {
                switch (type)
                {
                    case LEFT_ROCKER_X:
                        return Xbox360Axis.LeftThumbX;
                    case LEFT_ROCKER_Y:
                        return Xbox360Axis.LeftThumbY;
                    case RIGHT_ROCKER_X:
                        return Xbox360Axis.RightThumbX;
                    case RIGHT_ROCKER_Y:
                        return Xbox360Axis.RightThumbY;
                }
                throw new Exception($"No such type. type = ${type}");
            }

        }

        public static class TriggerType
        {
            public const int LEFT_TRIGGER = 0;
            public const int RIGHT_TRIGGER = 1;

            public static Xbox360Slider ToXbox360Slider(int type)
            {
                switch (type)
                {
                    case LEFT_TRIGGER:
                        return Xbox360Slider.LeftTrigger;
                    case RIGHT_TRIGGER:
                        return Xbox360Slider.RightTrigger;
                }
                throw new Exception($"No such type. type = ${type}");
            }

        }

    }
}
