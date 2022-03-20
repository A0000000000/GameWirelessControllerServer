using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionServiceServer.Internal
{
    public class ConnectionServiceController
    {
        private static volatile ConnectionServiceController INSTANCE;

        public static ConnectionServiceController Instance
        {
            get
            {
                if (INSTANCE == null)
                {
                    lock (typeof(ConnectionServiceController))
                    {
                        if (INSTANCE == null)
                        {
                            INSTANCE = new ConnectionServiceController();
                        }
                    }
                }
                return INSTANCE;
            }
        }

        public ConnectionServiceServer GetConnectionServiceServer(string guid)
        {
            return new ConnectionServiceServer(guid);
        }

    }

}
