using System;

namespace ProceduralRooms
{
    [Flags]
    public enum RoomDoorDirection
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Forward = 1 << 2,
        Backward = 1 << 3,

        All = Left | Right | Forward | Backward,
    }
}
    