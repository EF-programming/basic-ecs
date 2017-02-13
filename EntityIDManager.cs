using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitySystem
{
    // Dispenses unique integer IDs. Reuses freed IDs.
    class EntityIDManager
    {
        #region Singleton
        private static readonly EntityIDManager instance = new EntityIDManager();

        private EntityIDManager() { }

        public static EntityIDManager Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        int idCounter = -1;
        Stack<int> reusableID = new Stack<int>();

        public int getNewID()
        {
            if (reusableID.Count != 0)
                return reusableID.Pop();
            else
                return ++idCounter;
        }

        public void deleteID(int id)
        {
            reusableID.Push(id);
        }

        public void clear()
        {
            idCounter = -1;
            reusableID.Clear();
        }
    }
}
