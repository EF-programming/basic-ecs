using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitySystem
{
	// All components implement this interface.
	public interface IComponent
	{
		int EntityID { get; set; }
	}
}
