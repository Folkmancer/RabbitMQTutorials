﻿using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ReceiveLogsDirect
{
    class ReceiveLogsDirect
    {
        public static int Main(string[] args)
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);

                    var queueName = channel.QueueDeclare().QueueName;

                    if (args.Length < 1)
                    {
                        Console.Error.WriteLine("Usage: {0} [info] [warning] [error]", Environment.GetCommandLineArgs()[0]);
                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                        return 1;
                    }

                    foreach (var serenity in args)
                    {
                        channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: serenity);
                    }
                    
                    Console.WriteLine(" [*] Waiting for messages.");
                    
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        
                        Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                    
                    return 0;
                }
            }
        }
    }
}