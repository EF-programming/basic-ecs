using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitySystem
{
	// Assigns a unique integer ID to each component type.
	class ComponentIDManager
	{
		#region Singleton
		private static readonly ComponentIDManager instance = new ComponentIDManager();

		private ComponentIDManager() { }

		public static ComponentIDManager Instance
		{
			get
			{
				return instance;
			}
		}
		#endregion

		Dictionary<Type, int> componentTypeIDs = new Dictionary<Type, int>();
		
		// The first assigned ID is 1. Component Types never get an ID of 0, because 0 is a value returned by the 
		// dictionary's TryGetValue() method when the Type is not found.
		int id = 0;

		private int addComponentTypeID(Type componentType)
		{
			componentTypeIDs.Add(componentType, ++id);
			return id;
		}

		public int getComponentTypeID(IComponent component)
		{
			return getComponentTypeID(component.GetType());
		}

		public int getComponentTypeID(Type componentType)
		{
			int getID;

			componentTypeIDs.TryGetValue(componentType, out getID);

			if (getID == 0)
				getID = addComponentTypeID(componentType);

			return getID;
		}

		public void clear()
		{
			id = 0;
			componentTypeIDs.Clear();
		}
	}
}
