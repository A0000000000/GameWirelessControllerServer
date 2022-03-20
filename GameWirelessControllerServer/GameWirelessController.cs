using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectionServiceServer;
using ConnectionServiceServer.Internal;
using GameControllerServiceClient;

namespace GameWirelessControllerServer
{
    using ConnectionServiceServer = ConnectionServiceServer.Internal.ConnectionServiceServer;

    public class GameWirelessController: IDisposable
    {
        public static string Guid
        {
            get;
            set;
        } = "6ef82393-6cab-4749-b0b5-df0109fb7dec";

        public static string Id
        {
            get;
            set;
        } = "0";

        public static int Type
        {
            get;
            set;
        } = Joystick.TYPE_XBOX360;

        private ConnectionServiceController mConnectionServiceController;
        private ConnectionServiceServer mConnectionServiceServer;
        private JoystickController mJoystickController;

        public GameWirelessController()
        {
            mConnectionServiceController = ConnectionServiceController.Instance;
            mConnectionServiceServer = mConnectionServiceController.GetConnectionServiceServer(Guid);
            mJoystickController = JoystickController.Instance;
        }

        public void Init()
        {
            mConnectionServiceServer.Init(client =>
            {
                ConnectionServiceClientDecorator<TransferObject> conn = new ConnectionServiceClientDecorator<TransferObject>(client);
                Joystick joystick = mJoystickController.GetJoystick(Id, Type);
                Connections.Add(() => 
                {
                    conn?.Dispose();
                    joystick?.Dispose();
                });
                conn.OnDisconnect = () =>
                {
                    joystick.Dispose();
                };
                joystick.Init();
                joystick.Xbox360FeedbackReceived = (largeMotor, smallMotor, ledNumber) =>
                {
                    JoystickEvent joystickEvent = new JoystickEvent(largeMotor, smallMotor, ledNumber);
                    conn.Write(TransferObject.CreateJoystickEvent(joystickEvent));
                };
                conn.OnNameReady = () =>
                {
                    joystick.Id = conn.Name;  
                };
                conn.Init(obj =>
                {
                    if (obj != null && obj.Type == TransferObject.TYPE_GAME_EVENT && obj.GameEvent != null)
                    {
                        joystick.SendControllerEvent(obj.GameEvent);
                    }
                });
            });
        }

        private HashSet<Action> Connections = new HashSet<Action>();

        public void Dispose()
        {
            mJoystickController = null;
            mConnectionServiceController = null;
            mConnectionServiceServer.Dispose();
            foreach (Action a in Connections)
            {
                a();
            }
            Connections.Clear();
        }
    }
}
