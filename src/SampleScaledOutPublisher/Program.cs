using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Events;
using NServiceBus;

namespace SampleScaledOutPublisher
{
	class Program
	{
		static void Main( string[] args )
		{
			var config = new BusConfiguration();
			config.UsePersistence<InMemoryPersistence>();
			config.UseSerialization<JsonSerializer>();
			config.UseTransport<RabbitMQTransport>()
				.DisableCallbackReceiver();

			config.Transactions()
				.DisableDistributedTransactions();

			config.EnableInstallers();
			config.Conventions()
				.DefiningCommandsAs( t => t.Namespace != null && t.Namespace.EndsWith( ".Commands" ) )
				.DefiningEventsAs( t => t.Namespace != null && t.Namespace.EndsWith( ".Events" ) )
				.DefiningMessagesAs( t => t.Namespace != null && t.Namespace.EndsWith( ".Messages" ) );

			using( var bus = Bus.Create( config ).Start() )
			{
				Logic.Run( setup =>
				{
					setup.DefineAction( ConsoleKey.P, "Publish sample event", () =>
					{
						bus.Publish<IMessagePublished>(e =>
						{
							e.ClientId = "client-id";
							e.MessageId = "sample-message-id";
						});
					} );
				} );
			}
		}
	}
}
