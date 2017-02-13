using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using EntitySystem.Util;

namespace EntitySystem
{
    // Manages entities and their components. Contains methods for retrieving components. Filters entities to registered systems.
    public class ECSWorld
    {
        private ComponentIDManager componentIDManager = ComponentIDManager.Instance;
        private EntityIDManager entityIDManager = EntityIDManager.Instance;
        private List<SystemAndComponentBits> systems = new List<SystemAndComponentBits>();
        private GrowableArray<BitVector32> componentsBitmaskPerEntity = new GrowableArray<BitVector32>();
        private GrowableArray<GrowableArray<IComponent>> componentsByType = new GrowableArray<GrowableArray<IComponent>>();

        public ECSWorld()
        {
            for (int i = 0; i < componentsByType.getLength(); i++)
            {
                componentsByType[i] = new GrowableArray<IComponent>();
            }
        }

        public void addSystem(EntityProcessingSystem system)
        {
            system.setECSWorld(this);
            Type[] componentTypes = system.getComponentTypes();
            BitVector32 componentsBitmask = new BitVector32(0);

            foreach (Type compType in componentTypes)
            {
                int typeID = componentIDManager.getComponentTypeID(compType);
                componentsBitmask[(int)Math.Pow(2, typeID)] = true;
            }

            systems.Add(new SystemAndComponentBits(system, componentsBitmask));
        }

        public int addEntity(IComponent[] componentSet)
        {
            int id = entityIDManager.getNewID();
            BitVector32 componentsBitmask = new BitVector32(0);

            foreach (IComponent comp in componentSet)
            {
                comp.EntityID = id;
                int typeID = componentIDManager.getComponentTypeID(comp);
                componentsByType[typeID][id] = comp;
                componentsBitmask[(int)Math.Pow(2, typeID)] = true;
            }

            componentsBitmaskPerEntity[id] = componentsBitmask;

            filterEntityToSystems(id);

            return id;
        }

        public void removeEntity(int id)
        {
            foreach (GrowableArray<IComponent> compArray in componentsByType)
            {
                compArray[id] = null;
            }

            foreach (SystemAndComponentBits sys in systems)
            {
                sys.System.removeEntity(id);
            }

            componentsBitmaskPerEntity[id] = new BitVector32(0);

            entityIDManager.deleteID(id);
        }

        public void addComponentToEntity(int id, IComponent comp)
        {
            comp.EntityID = id;
            int typeID = componentIDManager.getComponentTypeID(comp);
            BitVector32 componentsBitmask = componentsBitmaskPerEntity[id];
            componentsBitmask[(int)Math.Pow(2, typeID)] = true;
            componentsBitmaskPerEntity[id] = new BitVector32(componentsBitmask);
            componentsByType[typeID][id] = comp;
            filterEntityToSystems(id);
        }

        public void removeComponentFromEntity<T>(int id) where T : IComponent
        {
            int typeID = componentIDManager.getComponentTypeID(typeof(T));
            BitVector32 componentsBitmask = componentsBitmaskPerEntity[id];
            componentsBitmask[(int)Math.Pow(2, typeID)] = false;
            componentsBitmaskPerEntity[id] = new BitVector32(componentsBitmask);
            componentsByType[typeID][id] = null;
            filterEntityToSystems(id);
        }

        // Register an entity under the systems that will be processing its components.
        private void filterEntityToSystems(int id)
        {
            foreach (SystemAndComponentBits sys in systems)
            {
                if ((componentsBitmaskPerEntity[id].Data & sys.ComponentsBitmask.Data) == sys.ComponentsBitmask.Data)
                    sys.System.addEntity(id);
                else
                    sys.System.removeEntity(id);
            }
        }

        public T getComponent<T>(int id) where T : IComponent
        {
            return (T)componentsByType[componentIDManager.getComponentTypeID(typeof(T))][id];
        }

        public List<T> getAllComponentsOfType<T>() where T : IComponent
        {
            List<T> components = new List<T>();

            foreach (IComponent component in componentsByType[componentIDManager.getComponentTypeID(typeof(T))])
            {
                if (component != null)
                {
                    components.Add((T)component);
                }
            }
            return components;
        }

        public List<IComponent> getComponentsForEntity(int id)
        {
            List<IComponent> components = new List<IComponent>();
            BitVector32 componentsBitmask = componentsBitmaskPerEntity[id];

            for (int i = 1; i < componentsByType.getLength(); i++)
            {
                if (componentsBitmask[(int)Math.Pow(2, i)] == true)
                {
                    components.Add(componentsByType[i][id]);
                }
            }
            return components;
        }

        public void clearEntities()
        {
            entityIDManager.clear();
            componentsBitmaskPerEntity.clear();

            foreach (GrowableArray<IComponent> array in componentsByType)
            {
                array.clear();
            }

            foreach (SystemAndComponentBits system in systems)
            {
                system.System.entityIDsToProcess.Clear();
            }
        }

        // Used to perform fast comparisons between a system's set of components and entity components.
        private class SystemAndComponentBits
        {
            public EntityProcessingSystem System;
            public BitVector32 ComponentsBitmask;

            public SystemAndComponentBits(EntityProcessingSystem system, BitVector32 componentsBitmask)
            {
                this.System = system;
                this.ComponentsBitmask = componentsBitmask;
            }
        }
    }
}
