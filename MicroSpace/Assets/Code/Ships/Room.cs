using System;

namespace Ships
{
    [Serializable]
    public class Room
    {
        public int Id;

        public Room(int id)
        {
            Id = id;
        }
    }
}