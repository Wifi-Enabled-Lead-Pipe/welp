using System;
using System.Collections.Generic;

namespace Welp.ServerData;

public class ServerDataBaseModel { }

public class PlayerActionInput
{
    public bool IsValid { get; set; }
}

public class PlayerActionValidationOutput
{
    public string Status { get; set; } = string.Empty;
    public GameState GameState { get; set; } = new();
}

public class GameRoom
{
    public string RoomName { get; set; } = string.Empty;

    // public static bool operator ==(GameRoom room1, GameRoom room2)
    // {
    //     return room1.getRoomName.Equals(room2.RoomName);
    // }

    // public static bool operator !=(GameRoom room1, GameRoom room2)
    // {
    //     return !room1.getRoomName.Equals(room2.RoomName);
    // }

    public bool Equals(GameRoom room)
    {
        return RoomName.Equals(room.RoomName);
    }
}

public class GameBoard
{
    public List<GameRoom> GameRooms { get; set; } = new List<GameRoom>(); // list of rooms
    public Dictionary<string, GameRoom> Positions { get; set; } =
        new Dictionary<string, GameRoom>(); // IdOrName to GameRoom

    public List<GameRoom> getAdjGameRooms(GameRoom gameRoom)
    {
        List<GameRoom> adjGameRooms = new List<GameRoom>();
        // TODO: add adjacent rooms
        return adjGameRooms;
    }

    public bool isOccupied(GameRoom gameRoom)
    {
        foreach (var entry in Positions)
        {
            if (entry.Value.Equals(gameRoom))
            {
                return true;
            }
        }

        return false;
    }
}

public class GameState
{
    public GameRoom GameRoom { get; set; } = new(); // player current room
    public GameBoard GameBoard { get; set; } = new(); // current board
}
