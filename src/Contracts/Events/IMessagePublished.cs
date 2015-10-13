using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
	public interface IMessagePublished
	{
		String MessageId { get; set; }
		String ClientId { get; set; }
	}
}
