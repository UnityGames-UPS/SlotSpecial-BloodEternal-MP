using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using JetBrains.Annotations;

public class SocketModel
{

    public Player playerData;
    public UiData uIData;

    public GameData initGameData;

    public Root resultGameData;

    public GambleData gambleData;
    public int currentBetIndex = 0;

    internal SocketModel()
    {
        this.playerData = new Player();
        this.uIData = new UiData();
        this.initGameData = new GameData();
        this.resultGameData = new Root();
        this.gambleData = new GambleData();
    }

}


public class GameData
{
    public List<List<int>> lines { get; set; }
    public List<double> bets { get; set; }
}

public class Paylines
{
    public List<Symbol> symbols { get; set; }
}

public class Player
{
    public double balance { get; set; }
}

public class ReelsInstance
{
    [JsonProperty("0")]
    public int _0 { get; set; }

    [JsonProperty("1")]
    public int _1 { get; set; }

    [JsonProperty("2")]
    public int _2 { get; set; }

    [JsonProperty("3")]
    public int _3 { get; set; }

    [JsonProperty("4")]
    public int _4 { get; set; }

    [JsonProperty("5")]
    public int _5 { get; set; }
}

public class Root
{
    public string id { get; set; }
    public GameData gameData { get; set; }
    public UiData uiData { get; set; }
    public Player player { get; set; }

    public bool success { get; set; }
    public List<List<string>> matrix { get; set; }

    public Payload payload { get; set; }
    public Features features { get; set; }

}

public class Symbol
{
    public int id { get; set; }
    public string name { get; set; }
    public ReelsInstance reelsInstance { get; set; }
    public List<int> multiplier { get; set; }
}

public class UiData
{
    public Paylines paylines { get; set; }
}
[Serializable]
public class AuthTokenData
{
    public string cookie;
    public string socketURL;
    public string nameSpace; //BackendChanges
}



// result



public class Bats
{
    public List<string> positions { get; set; }
    public int payout { get; set; }
}

public class BloodSplash
{
    public string index { get; set; }
    public string symbolId { get; set; }
}

public class Features
{
    public FreeSpin freeSpin { get; set; }
    public Bats bats { get; set; }
    public List<List<string>> winningSymbols { get; set; }
}

public class FreeSpin
{
    // public bool isFreeSpin { get; set; }
    public bool isTriggered { get; set; }
    public int freeSpinCount { get; set; }

    public List<List<string>> newVampHumanPositions { get; set; }
    // public List<string> vampHuman { get; set; }

    public Substitutions substitutions { get; set; }
}

public class LineWin
{
    public int lineIndex { get; set; }
    public List<int> positions { get; set; }
    // public string symbol { get; set; }
    // public int matchCount { get; set; }
    //  public string direction { get; set; }
}

public class Payload
{
    public double winAmount { get; set; }
    public List<LineWin> lineWins { get; set; }


    public bool playerWon { get; set; }
    public double currentWinning { get; set; }
    public string coin { get; set; }
}


[Serializable]
public class MessageData
{
    public string type;

    public SentDeta payload;


}

[Serializable]
public class SentDeta
{
    public int betIndex;
    public string Event;
    public double lastWinning;
    public int index;


    public string selectedSide;
    public string gambleOption;
}

public class Substitutions
{
    public List<List<string>> vampHuman { get; set; }
    public List<BloodSplash> bloodSplash { get; set; }
}

public class GambleData
{

    public bool playerWon { get; set; }
    public double currentWinning { get; set; }
    public string coin { get; set; }
    public double balance { get; set; }

}



// [Serializable]
// public class ResultGameData
// {
//     public List<List<int>> ResultReel { get; set; }
//     public List<int> linesToEmit { get; set; }
//     public List<List<string>> symbolsToEmit { get; set; }
//     public bool isFreeSpin {get; set;}
//     public List<string> vampHuman {get; set;}

//     public List<string> bloodSplash {get; set;}

//     public List<string> batPositions {get; set;}
//     // public freeSpin freeSpin{get; set;}
//     public int count {get; set;}
//     // public int freeSpinCount { get; set; }
//     public double jackpot { get; set; }
// }


// [Serializable]
// public class InitGameData
// {
//     // public List<List<int>> Lines { get; set; }
//     public List<double> Bets { get; set; }
//     public bool canSwitchLines { get; set; }
//     public List<int> LinesCount { get; set; }
//     public List<List<int>> lineData {get; set;}
//     public double freeSpinCount{get; set;}
// }


// [Serializable]
// public class UIData
// {
//     public List<Symbol> symbols { get; set; }
//         public List<double> BatsMultiplier {get; set;}

//     public List<double> wildMultiplier {get; set;}
// }

// [Serializable]
// public class freeSpin{



// }

// [Serializable]
// public class BetData
// {
//     public double currentBet;
//     public double currentLines;
//     public double spins;
//     //public double TotalLines;
// }

// [Serializable]
// public class AuthData
// {
//     public string GameID;
//     //public double TotalLines;
// }

// [Serializable]
// public class MessageData
// {
//     public BetData data;
//     public string id;
// }

// [Serializable]
// public class InitData
// {
//     public AuthData Data;
//     public string id;
// }

// [Serializable]
// public class AbtLogo
// {
//     public string logoSprite { get; set; }
//     public string link { get; set; }
// }



// [Serializable]
// public class Symbol
// {
//     public int ID { get; set; }
//     public string Name { get; set; }
//     public List<List<int>> Multiplier { get; set;}
//     public object defaultAmount { get; set; }
//     public object symbolsCount { get; set; }
//     public object increaseValue { get; set; }
//     public object description { get; set; }
//     public int freeSpin { get; set; }
// }

// [Serializable]

// public class GambleData{

//     public bool playerWon{get; set;}
//     public double currentWinning{get; set;}
//     public string coin{get; set;}
//     public double balance{get; set;}

// }


// [Serializable]
// public class PlayerData
// {
//     public double Balance { get; set; }
//     public double haveWon { get; set; }
//     public double currentWining { get; set; }
// }

// [Serializable]
// public class AuthTokenData
// {
//     public string cookie;
//     public string socketURL;
//     public string nameSpace;
// }
