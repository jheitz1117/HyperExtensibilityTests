using System;
using System.ServiceModel;
using Hyper.NodeServices;

namespace NodeService1
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHost(HyperNodeService.Instance);

            Console.WriteLine("Starting service...");
            host.Open();

            Console.WriteLine("Service started and is listening on the following addresses:");
            foreach (var endpoint in host.Description.Endpoints)
            {
                Console.WriteLine("    " + endpoint.Address);
            }

            Console.ReadKey();

            host.Close();
        }
    }
}
