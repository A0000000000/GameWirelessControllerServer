using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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

        private static int _id = 0;

        public static string Id
        {
            get {

                return (_id++).ToString();
            }
        }

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
                OnClientConnection(conn);
            });
        }

        public void OnClientConnection(ConnectionServiceClientDecorator<TransferObject> conn)
        {
            Joystick joystick = mJoystickController.GetJoystick(Id, Type);
            Action action = () =>
            {
                conn.Dispose();
            };
            Clients.Add(action);
            conn.OnDisconnect = () =>
            {
                Clients.Remove(action);
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
        }

        private List<Action> Clients = new List<Action>();

        public void Dispose()
        {
            foreach(Action a in Clients)
            {
                a();
            }
            Clients.Clear();
            mJoystickController = null;
            mConnectionServiceController = null;
            mConnectionServiceServer.Dispose();
        }
    }
}
