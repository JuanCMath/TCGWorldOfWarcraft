using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enums
{
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