namespace Enums
{
    public enum VariableType
    {
        Double,
        String,
    }
    public enum player
    {
        Player1,
        Player2
    }
    public enum slot 
    {
        M,   // Melee
        R,   // Range
        S,   // Siege
        MR,  // Melee & Range
        MS,  // Melee & Siege
        RS,  // Range & Siege
        MRS, // Melee & Range & Siege
        X    // Special Cards
    }
    public enum faction
    {
        Aspectos, 
        Arthas
    }
    public enum type
    {
        Unidad,
        Clima,
        Aumento,
        Despeje,
        Señuelo
    }
    
    public enum gameTracker
    {
        StartingTheGame, 
        StartingRound, 
        Player1Turn, 
        Player2Turn,
        FinalOfRound, 
        GameOver
    }
}