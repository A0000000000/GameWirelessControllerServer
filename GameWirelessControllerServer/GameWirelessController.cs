using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GameWirelessControllerServer
{

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
        } = GameControllerServiceClient.Joystick.TYPE_XBOX360;

        private ConnectionServiceServer.Raw.ConnectionServiceController mConnectionServiceController;
        private ConnectionServiceServer.Raw.ConnectionServiceServer mConnectionServiceServer;
        private GameControllerServiceClient.JoystickController mJoystickController;

        public GameWirelessController()
        {
            mConnectionServiceController = ConnectionServiceServer.Raw.ConnectionServiceController.Instance;
            mConnectionServiceServer = mConnectionServiceController.GetConnectionServiceServer(Guid);
            mJoystickController = GameControllerServiceClient.JoystickController.Instance;
        }

        public void Init()
        {
            mConnectionServiceServer.Init(client =>
            {
                OnClientConnection(client);
            });
        }

        public void OnClientConnection(ConnectionServiceServer.Raw.ConnectionServiceClient conn)
        {
            GameControllerServiceClient.Joystick joystick = mJoystickController.GetJoystick(Id, Type);
            Action action = () =>
            {
                joystick.Dispose();
                conn.Dispose();
            };
            Clients.Add(action);
            joystick.Init();
            joystick.Xbox360FeedbackReceived = (largeMotor, smallMotor, ledNumber) =>
            {
                JoystickEvent ev = new JoystickEvent(largeMotor, smallMotor, ledNumber);
                conn.Write(Encoding.UTF8.GetBytes(ConnectionServiceServer.Utils.JsonUtils<JoystickEvent>.ToJson(ev)));
            };
            conn.Init(data =>
            {
                GameControllerServiceClient.GameEvent ev = ConnectionServiceServer.Utils.JsonUtils<GameControllerServiceClient.GameEvent>.FromJson(Encoding.UTF8.GetString(data));
                joystick.SendControllerEvent(ev);
            }, e =>
            {
                joystick.Dispose();
                conn.Dispose();
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
