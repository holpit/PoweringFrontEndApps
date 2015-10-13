using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Events;
using NServiceBus;
using Topics.Radical.Helpers;
using Topics.Radical;

namespace SampleScaledOutConsole
{
	class Program
	{
		static void Main( string[] args )
		{
			var commandLine = CommandLine.GetCurrent();

			var config = new BusConfiguration();
			config.UsePersistence<InMemoryPersistence>();
			config.UseSerialization<JsonSerializer>();
			config.UseTransport<RabbitMQTransport>()
				.DisableCallbackReceiver();

			String instance;
			if( commandLine.TryGetValue( "instance", out instance ) )
			{
				config.ScaleOut()
					.UniqueQueuePerEndpointInstance( $"-{instance}" );
			}

			config.Transactions()
				.DisableDistributedTransactions();

			config.EnableInstallers();
			config.Conventions()
				.DefiningCommandsAs( t => t.Namespace != null && t.Namespace.EndsWith( ".Commands" ) )
				.DefiningEventsAs( t => t.Namespace != null && t.Namespace.EndsWith( ".Events" ) )
				.DefiningMessagesAs( t => t.Namespace != null && t.Namespace.EndsWith( ".Messages" ) );

			using( Bus.Create( config ).Start() )
			{
				Console.WriteLine( "Bus started..." );
				Console.Read();
			}
		}
	}

	internal class H: IHandleMessages<Contracts.Events.IMessagePublished>
	{
		public void Handle(IMessagePublished message)
		{
			using (ConsoleColor.Green.AsForegroundColor())
			{
				Console.WriteLine( "IMessagePublished received." );
			}
		}
	}
}
