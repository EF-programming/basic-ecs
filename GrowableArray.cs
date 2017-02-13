using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitySystem.Util
{
    // Array that resizes itself when needed. Differs from a list mainly in that it accepts direct index insertion outside of its current length.
    class GrowableArray<E> : IEnumerable
    {
        private E[] data;

        public GrowableArray()
            : this(64)
        { }

        public GrowableArray(int capacity)
        {
            data = new E[capacity];
        }

        public E this[int index]
        {
            get
            {
                if (index > data.Length - 1)
                    return default(E);
                return data[index];
            }
            set
            {
                if (index > data.Length - 1)
                {
                    grow((index * 3) / 2 + 1);
                }
                data[index] = value;
            }
        }

        public E get(int index)
        {
            if (index > data.Length - 1)
                return default(E);
            return data[index];
        }

        public void set(int index, E e)
        {
            if (index > data.Length - 1)
            {
                grow((index * 3) / 2 + 1);
            }
            data[index] = e;
        }

        public void setDefault(int index)
        {
            data[index] = default(E);
        }

        public bool setDefault(E e)
        {
            for (int i = 0; i < data.Length; i++)
            {
                E e2 = data[i];

                if (e.Equals(e2))
                {
                    data[i] = default(E);
                    return true;
                }
            }
            return false;
        }

        private void grow()
        {
            int newCapacity = ((data.Length * 3) / 2 + 1);
            grow(newCapacity);
        }

        private void grow(int newCapacity)
        {
            E[] oldData = data;
            data = new E[newCapacity];
            Array.Copy(oldData, data, oldData.Length);
        }

        public bool contains(E e)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (e.Equals(data[i]))
                    return true;
            }
            return false;
        }

        //Removes elements in passed array from this array.
        public bool removeAll(GrowableArray<E> growAr)
        {
            bool modified = false;

            for (int i = 0; i < growAr.getLength(); i++)
            {
                E e1 = growAr[i];

                for (int j = 0; j < data.Length; j++)
                {
                    if (e1.Equals(data[j]))
                    {
                        setDefault(j);
                        modified = true;
                        break;
                    }
                }
            }
            return modified;
        }

        public int getLength()
        {
            return data.Length;
        }

        public E[] toArray()
        {
            return (E[])data.Clone();
        }

        public void clear()
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = default(E);
        }

        #region IEnumerator implementation
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public GrowableArrayEnum GetEnumerator()
        {
            return new GrowableArrayEnum(data);
        }

        public class GrowableArrayEnum : IEnumerator
        {
            public E[] data;

            // Enumerators are positioned before the first element 
            // until the first MoveNext() call. 
            int position = -1;

            public GrowableArrayEnum(E[] list)
            {
                data = list;
            }

            public bool MoveNext()
            {
                position++;
                return (position < data.Length);
            }

            public void Reset()
            {
                position = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public E Current
            {
                get
                {
                    try
                    {
                        return data[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }
        #endregion
    }
}
