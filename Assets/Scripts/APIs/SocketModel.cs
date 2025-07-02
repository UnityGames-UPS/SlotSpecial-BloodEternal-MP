using System.Collections.Generic;
using System;

public class SocketModel
{
  public PlayerData playerData;
  public UIData uIData;
  public InitGameData initGameData;
  public ResultGameData resultGameData;
  public GambleData gambleData;
  public int currentBetIndex = 0;

  internal SocketModel()
  {
    this.playerData = new PlayerData();
    this.uIData = new UIData();
    this.initGameData = new InitGameData();
    this.resultGameData = new ResultGameData();
    this.gambleData = new GambleData();
  }
}

[Serializable]
public class ResultGameData
{
  public List<List<int>> ResultReel { get; set; }
  public List<int> linesToEmit { get; set; }
  public List<List<string>> symbolsToEmit { get; set; }
  public bool isFreeSpin { get; set; }
  public List<string> vampHuman { get; set; }
  public List<string> bloodSplash { get; set; }
  public List<string> batPositions { get; set; }
  public int count { get; set; }
}

[Serializable]
public class InitGameData
{
  public List<double> Bets { get; set; }
  public List<List<int>> lineData { get; set; }
}

[Serializable]
public class UIData
{
  public List<Symbol> symbols { get; set; }
  public List<double> BatsMultiplier { get; set; }
  public List<double> wildMultiplier { get; set; }
}

[Serializable]
public class Symbol
{
  public List<List<int>> Multiplier { get; set; }
}

[Serializable]
public class GambleData
{
  public bool playerWon { get; set; }
  public double currentWinning { get; set; }
  public string coin { get; set; }
  public double balance { get; set; }

}

[Serializable]
public class PlayerData
{
  public double Balance { get; set; }
  public double currentWining { get; set; }
}

[Serializable]
public class AuthTokenData
{
  public string cookie;
  public string socketURL;
  public string nameSpace;
}
