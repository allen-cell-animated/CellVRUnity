using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour 
{
    public Text rank;
    public Text playerName;
    public Text time;

    public void Populate (int _rank, string _playerName, float _timeSeconds)
    {
        rank.text = "#" + _rank.ToString();
        playerName.text = _playerName;
        time.text = UIManager.Instance.FormatTime( _timeSeconds );
    }

    public void UpdatePlayerName (string _newPlayerName)
    {
        playerName.text = _newPlayerName;
    }
}
