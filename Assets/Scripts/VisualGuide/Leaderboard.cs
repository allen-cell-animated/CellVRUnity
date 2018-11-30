using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Leaderboard : MonoBehaviour 
{
    [SerializeField]
    List<HighScore> highScores;
    HighScore currentScore;
    LeaderboardEntry currentEntry;

    void OnEnable ()
    {
        LoadSavedRankings();
    }

    void OnDisable ()
    {
        SaveRankings();
    }

    void LoadSavedRankings ()
    {
        highScores = new List<HighScore>();
        float timeSeconds;
        Debug.Log(PlayerPrefs.GetString( "highscores", "" ));
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

    public void ClearAllRankings ()
    {
        PlayerPrefs.DeleteAll();
        highScores.Clear();
        Debug.Log( "Cleared all highscores!" );
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

    public void RecordNewScore (float timeSeconds)
    {
        Debug.Log("record " + timeSeconds);
        currentScore = new HighScore( "[your name here!]", timeSeconds );
        highScores.Add( currentScore );
        highScores.Sort();

        ClearDisplay();
        DisplayHighscores();
    }

    void DisplayHighscores ()
    {
        int currentIndex = highScores.FindIndex( s => s == currentScore );
        int firstRankToDisplay = Mathf.RoundToInt( Mathf.Max( Mathf.Min( highScores.Count - 7, currentIndex - 3 ), 0 ) );
        int rank = firstRankToDisplay;
        int duplicates = 0;
        while (rank > 0 && highScores.Count > 1 && Mathf.RoundToInt( highScores[rank-1].timeSeconds ) == Mathf.RoundToInt( highScores[rank].timeSeconds ))
        {
            rank--;
            duplicates++;
        }
        rank++;

        List<HighScore> scores = highScores.GetRange( firstRankToDisplay, Mathf.Min( 7, highScores.Count - firstRankToDisplay ) );
        for (int i = 0; i < scores.Count; i++)
        {
            if (i > 0)
            {
                if (Mathf.RoundToInt( scores[i-1].timeSeconds ) != Mathf.RoundToInt( scores[i].timeSeconds ))
                {
                    rank += 1 + duplicates;
                    duplicates = 0;
                }
                else
                {
                    duplicates++;
                }
            }
            CreateLeaderboardEntry( rank, i, scores[i] );
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
        GameObject entry = Instantiate( prefab, transform ) as GameObject;

        entry.GetComponent<RectTransform>().anchoredPosition = new Vector2( 4f, -20f - 10f * listIndex );
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

    public void UpdateCurrentPlayerName (string _newPlayerName)
    {
        currentScore.playerName = _newPlayerName;
        currentEntry.UpdatePlayerName( _newPlayerName );
    }

    public void ReturnToLobby ()
    {
        VisualGuideManager.Instance.ReturnToLobby();
    }
}

[System.Serializable]
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
