using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Server.DataLayer;
using Server.Entities;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestDataLayer();

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

        private static void TestDataLayer()
        {
            IDAL dal = new MsSqlDAL();
            User user = new User();
            user.Id = Guid.NewGuid();
            user.Email = "fake@mail.com";
            user.Password = "fake_pwd";
            dal.SaveUser(user);


            //IDAL dal = new MsSqlDAL();
            //User user = dal.GetUser("fake@mail.com");
            //Console.WriteLine(user.Email + " " + user.Password);
        }
    }
}
