using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IocpServer
{
    public class RoomPool
    {
        Stack<Room> m_pool;

        public RoomPool(int capacity)
        {
            m_pool = new Stack<Room>(capacity);
        }

        public void Push(Room item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }
        public Room Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }
        public int Count
        {
            get { return m_pool.Count; }
        }
    }
}
