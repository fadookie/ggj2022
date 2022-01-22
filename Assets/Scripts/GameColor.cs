using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameColor
{
    Black = 0,
    White = 1,
}

public static class GameColorUtil
{
    public static GameColor GetOpposite(this GameColor gameColor)
    {
        switch(gameColor)
        {
            case GameColor.Black: return GameColor.White;
            case GameColor.White: return GameColor.Black;
            default: throw new System.NotImplementedException(gameColor.ToString());
        }
    }
    public static Color GetColor(this GameColor gameColor)
    {
        switch (gameColor)
        {
            case GameColor.Black: return UnityEngine.Color.black;
            case GameColor.White: return UnityEngine.Color.white;
            default: throw new System.NotImplementedException(gameColor.ToString());
        }
    }
}
