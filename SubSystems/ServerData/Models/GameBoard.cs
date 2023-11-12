namespace Welp.ServerData
{
    public class GameBoard
    {        public List<GameRoom> GameRooms { get; set; }

        public List<GameRoom> getAdjGameRooms(GameRoom gameRoom)
        {
            List<GameRoom> adjGameRooms = new List<GameRoom>();
            switch (gameRoom.RoomName)
            {
                case RoomName.Study:
                    adjGameRooms.Add(
                        new GameRoom() 
                        {
                            RoomName = RoomName.Hall,
                            Position = (0, 2),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Library,
                            Position = (-2, 0),
                            HasSecretPassageway = false
                        }
                    );
                    break;
                case RoomName.Hall:
                    adjGameRooms.Add(
                        new GameRoom() 
                        {
                            RoomName = RoomName.Study,
                            Position = (-2, 2),
                            HasSecretPassageway = true
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Lounge,
                            Position = (2, 2),
                            HasSecretPassageway = true
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.BilliardRoom,
                            Position = (0, 0),
                            HasSecretPassageway = false
                        }
                    );
                    break;
                case RoomName.Lounge:
                    adjGameRooms.Add(
                        new GameRoom() 
                        {
                            RoomName = RoomName.Hall,
                            Position = (0, 2),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.DiningRoom,
                            Position = (2, 0),
                            HasSecretPassageway = false
                        }
                    );
                    break;
                case RoomName.Library:
                    adjGameRooms.Add(
                        new GameRoom() 
                        {
                            RoomName = RoomName.Study,
                            Position = (-2, 2),
                            HasSecretPassageway = true
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.BilliardRoom,
                            Position = (0, 0),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Conservatory,
                            Position = (-2, -2),
                            HasSecretPassageway = true
                        }
                    )
                    break;
                case RoomName.BilliardRoom:
                    adjGameRooms.Add(
                        new GameRoom() 
                        {
                            RoomName = RoomName.Hall,
                            Position = (0, 2),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Library,
                            Position = (-2, 0),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.DiningRoom,
                            Position = (2, 0),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Ballroom,
                            Position = (0, -2),
                            HasSecretPassageway = false
                        }
                    )
                    break;
                case RoomName.DiningRoom:
                    adjGameRooms.Add(
                        new GameRoom() 
                        {
                            RoomName = RoomName.Lounge,
                            Position = (2, 2),
                            HasSecretPassageway = true
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.BilliardRoom,
                            Position = (0, 0),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Kitchen,
                            Position = (2, -2),
                            HasSecretPassageway = true
                        }
                    )
                    break;
                case RoomName.Conservatory:
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Library,
                            Position = (-2, 0),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Ballroom,
                            Position = (0, -2),
                            HasSecretPassageway = false
                        }
                    )
                    break;
                case RoomName.Ballroom:
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Conservatory,
                            Position = (-2, -2),
                            HasSecretPassageway = true
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.BilliardRoom,
                            Position = (0, 0),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Kitchen,
                            Position = (2, -2),
                            HasSecretPassageway = true
                        }
                    )
                    break;
                case RoomName.Kitchen:
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.Ballroom,
                            Position = (0, -2),
                            HasSecretPassageway = false
                        }
                    );
                    adjGameRooms.Add(
                        new GameRoom()
                        {
                            RoomName = RoomName.DiningRoom,
                            Position = (2, 0),
                            HasSecretPassageway = false
                        }
                    )
                    break;
            }
            return adjGameRooms;
        }
    }
}
