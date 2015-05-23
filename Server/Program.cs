using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Server.DataLayer;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(typeof(IDataSaverService).FullName);
            //DataSaverService saverService = DataSaverService.GetSaverService(new DALStub());
            //ServiceHost host = new ServiceHost(saverService);
            ServiceHost host = new ServiceHost(typeof(DataSaverService));
            Console.WriteLine("Listening address: " + host.BaseAddresses[0]);
            host.Open();
            Console.WriteLine("Server has started listening...");
            Console.ReadKey();
            host.Close();
        }
    }
}
