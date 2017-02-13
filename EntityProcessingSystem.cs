using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EntitySystem
{
	public abstract class EntityProcessingSystem
	{
		protected ECSWorld world;
		public List<int> entityIDsToProcess = new List<int>();
		public bool Paused { get; set; }

		protected EntityProcessingSystem()
		{
			Paused = false;
		}

		// Returns the types of components that the system processes.
		public abstract Type[] getComponentTypes();

		public void setECSWorld(ECSWorld world)
		{
			this.world = world;
		}

		public abstract void processEntities(GameTime gameTime);

		public void addEntity(int id)
		{
			if (entityIDsToProcess.IndexOf(id) == -1)
				entityIDsToProcess.Add(id);
		}

		public void removeEntity(int id)
		{
			entityIDsToProcess.Remove(id);
		}
	}
}