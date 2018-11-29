using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Leaderboard : MonoBehaviour 
{
    List<HighScore> highScores;
    HighScore currentScore;
    LeaderboardEntry currentEntry;
    bool editingPlayerName;

    void Awake ()
    {
        LoadSavedRankings();
    }

    void LoadSavedRankings ()
    {
        highScores = new List<HighScore>();
        float timeSeconds;
        string[] data, scores = PlayerPrefs.GetString( "highscores", "" ).Split( ',' );
        foreach (string score in scores)
        {
            data = score.Split( ':' );

            if (data.Length < 2)
            {
                if (score.Length > 0)
                {
                    Debug.LogWarning( "Can't parse score: " + score );
                }
                continue;
            }

            if (!float.TryParse( data[1], out timeSeconds ))
            {
                Debug.LogWarning( "Can't parse time for user " + data[0] );
                continue;
            }

            highScores.Add( new HighScore( data[0], timeSeconds ) );
        }
    }

    public void RecordNewScore (float timeSeconds)
    {
        currentScore = new HighScore( "[your name here!]", timeSeconds );
        highScores.Add( currentScore );
        highScores.Sort();
        editingPlayerName = true;

        ClearDisplay();
        DisplayHighscores();
    }

    void DisplayHighscores ()
    {
        int currentIndex = highScores.FindIndex( s => s == currentScore );
        int firstRankToDisplay = Mathf.Max( 0, currentIndex - 3 );
        List<HighScore> scores = highScores.GetRange( firstRankToDisplay, Mathf.Min( 7, highScores.Count ) );
        for (int i = 0; i < scores.Count; i++)
        {
            CreateLeaderboardEntry( firstRankToDisplay + i, i, scores[i] );
        }
    }

    GameObject CreateLeaderboardEntry (int rank, int listIndex, HighScore score)
    {
        GameObject prefab = Resources.Load( score == currentScore ? "LeaderboardEntryCurrent" : "LeaderboardEntry" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for " + (score == currentScore ? "LeaderboardEntryCurrent" : "LeaderboardEntry") );
            return null;
        }
        GameObject entry = Instantiate( prefab ) as GameObject;

        entry.transform.SetParent( transform );
        entry.transform.localPosition = new Vector3( 4f, -20f - 10f * listIndex, -1.5f );

        entry.GetComponent<LeaderboardEntry>().Populate( rank, score.playerName, score.timeSeconds );

        if (score == currentScore)
        {
            currentEntry = entry.GetComponent<LeaderboardEntry>();
        }

        return entry;
    }

    void ClearDisplay ()
    {
        LeaderboardEntry[] entries = GetComponentsInChildren<LeaderboardEntry>();
        foreach (LeaderboardEntry entry in entries)
        {
            Destroy( entry.gameObject );
        }
    }

    void Update ()
    {
        if (editingPlayerName)
        {
            UpdateCurrentPlayerName( "" );
        }
    }

    public void UpdateCurrentPlayerName (string _newPlayerName)
    {
        currentScore.playerName = _newPlayerName;
        currentEntry.UpdatePlayerName( _newPlayerName );
    }

    public void ClearAllRankings ()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log( "Cleared all highscores!" );
    }

    void OnDestroy ()
    {
        SaveRankings();
    }

    void SaveRankings ()
    {
        string scores = "";
        foreach (HighScore score in highScores)
        {
            scores += score.playerName + ":" + score.timeSeconds + ",";
        }

        PlayerPrefs.SetString( "highscores", scores );
    }

    public void ReturnToLobby ()
    {
        VisualGuideManager.Instance.ReturnToLobby();
    }
}

public class HighScore : IComparable
{
    public string playerName;
    public float timeSeconds;

    public HighScore (string _playerName, float _timeSeconds)
    {
        playerName = _playerName;
        timeSeconds = _timeSeconds;
    }

    public int CompareTo (object obj)
    {
        HighScore otherHighscore = obj as HighScore;
        if (otherHighscore != null)
        {
            return timeSeconds.CompareTo( otherHighscore.timeSeconds );
        }
        return 1;
    }
}
