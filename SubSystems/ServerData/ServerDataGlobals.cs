namespace Welp.ServerData
{
    public static class ServerDataGlobals
    {
        public static Dictionary<Character, (int, int)> CharacterInitialPositions = new Dictionary<Character, (int, int)>
        {
            { Character.MissScarlet, (1, 2) },
            { Character.ProfessorPlum, (-2, 1) },
            { Character.ColonelMustard, (2, 1) },
            { Character.MrsPeacock, (-2, -1) },
            { Character.MrGreen, (-1, -2) },
            { Character.MrsWhite, (1, -2) }
        };
    }
}
