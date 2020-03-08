using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;

namespace EmitLogDirect
{
    class EmitLogDirect
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);

                    var severity = (args.Length > 0) ? args[0] : "Info";
                    var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "direct_logs", routingKey: severity, basicProperties: null, body: body);
                    
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
                }
                
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}