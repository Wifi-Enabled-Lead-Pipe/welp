using Welp.ServerData;

namespace Welp.ServerLogic;

public class ServerLogicService : IServerLogicService
{
    public async Task<PlayerActionValidationOutput> ValidatePlayerAction(
        PlayerActionInput actionInput,
        Game gameState
    )
    {
        if (actionInput.IsValid)
        {
            return await Task.FromResult(
                new PlayerActionValidationOutput() { Status = "valid", GameState = gameState }
            );
        }

        return await Task.FromResult(
            new PlayerActionValidationOutput() { Status = "invalid", GameState = gameState }
        );
    }

    public ActionOptions GenerateActionOptions(Game gameState, Player player)
    {
        ActionOptions currentOptions = new ActionOptions();

        if (gameState.ActionRegister.Values.Last().Player == player)
        {
            if (gameState.ActionRegister.Values.Last().ActionType == ActionType.MoveRoom)
            {
                // the player just moved into a room during their turn, now they can make a suggestion or accusation (or end turn)
                currentOptions.Suggestion = new ActionOption<Suggestion>()
                {
                    ActionType = ActionType.Suggestion
                };
                currentOptions.Accusation = new ActionOption<Accusation>()
                {
                    ActionType = ActionType.Accusation
                };
            }
            else if (gameState.ActionRegister.Values.Last().ActionType == ActionType.Suggestion)
            {
                // the player just made a suggestion, now their only option is to make an accusation (or end turn)
                currentOptions.Accusation = new ActionOption<Accusation>()
                {
                    ActionType = ActionType.Accusation
                };
            }
        }
        else
        {
            // it is the beginning of the player's turn
            currentOptions.Suggestion = new ActionOption<Suggestion>()
            {
                ActionType = ActionType.Suggestion
            };
            currentOptions.Accusation = new ActionOption<Accusation>()
            {
                ActionType = ActionType.Accusation
            };
            currentOptions.Movement = GenerateMovementOptions(gameState, player);
        }

        return currentOptions;
    }

    public List<ActionOption<Movement>> GenerateMovementOptions(Game gameState, Player player)
    {
        List<ActionOption<Movement>> movementOptions = new List<ActionOption<Movement>>();

        int playerInRoom = gameState.GameBoard.GameRooms.FindIndex(
            r => r.Position == player.Position
        );

        if (playerInRoom != -1)
        {
            GameRoom currRoom = gameState.GameBoard.GameRooms[playerInRoom];
            if (currRoom.HasSecretPassageway)
            {
                movementOptions.Add(
                    new ActionOption<Movement>()
                    {
                        ActionType = ActionType.MoveRoom,
                        Details = new Movement()
                        {
                            NewPosition = (currRoom.Position.x * -1, currRoom.Position.y * -1)
                        }
                    }
                );
            }

            List<GameRoom> adjRooms = gameState.GameBoard.getAdjGameRooms(currRoom);
            List<(int x, int y)> currPlayerPositions = gameState.Players
                .Select(p => p.Position)
                .ToList();
            foreach (GameRoom adjRoom in adjRooms)
            {
                // adding hallway coords for movement between curr room and adj rooms if hallway is not occupied
                (int x, int y) newPosition = (
                    (currRoom.Position.x + adjRoom.Position.x) / 2,
                    (currRoom.Position.y + adjRoom.Position.y) / 2
                );

                if (!currPlayerPositions.Contains(newPosition))
                {
                    movementOptions.Add(
                        new ActionOption<Movement>()
                        {
                            ActionType = ActionType.MoveHallway,
                            Details = new Movement() { NewPosition = newPosition }
                        }
                    );
                }
            }
        }
        else
        {
            // player is in a hallway so they can move into rooms; we calculate up/down/left/right and intersect with room list positions
            List<(int, int)> allMovements = new List<(int, int)>()
            {
                (player.Position.x, player.Position.y + 1),
                (player.Position.x, player.Position.y - 1),
                (player.Position.x - 1, player.Position.y),
                (player.Position.x + 1, player.Position.y)
            };

            List<(int x, int y)> validMovements = gameState.GameBoard.GameRooms
                .Select(r => r.Position)
                .ToList()
                .Intersect(allMovements)
                .ToList();
            foreach ((int x, int y) position in validMovements)
            {
                movementOptions.Add(
                    new ActionOption<Movement>()
                    {
                        ActionType = ActionType.MoveRoom,
                        Details = new Movement() { NewPosition = position }
                    }
                );
            }
        }

        return movementOptions;
    }
}
