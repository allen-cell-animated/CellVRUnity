using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitosisGameManager : MonoBehaviour 
{
    public bool inPlayMode;
    public string currentStructureName;
    public Bounds throwableSpawnArea;

    string[] throwableNames = {"ProphaseCell", "PrometaphaseCell", "MetaphaseCell", "AnaphaseCell", "TelophaseCell"};

    void Start ()
    {
        StartCoroutine( "SpawnThrowables" );
    }

    void OnDrawGizmos ()
    {
        Gizmos.DrawWireCube( throwableSpawnArea.center, throwableSpawnArea.size );
    }

    Vector3 randomPositionInThrowableSpawnArea
    {
        get
        {
            return new Vector3( Random.Range( throwableSpawnArea.min.x, throwableSpawnArea.max.x ),
                                Random.Range( throwableSpawnArea.min.y, throwableSpawnArea.max.y ),
                                Random.Range( throwableSpawnArea.min.z, throwableSpawnArea.max.z ));
        }
    }

    IEnumerator SpawnThrowables ()
    {
        GameObject prefab;
        foreach (string t in throwableNames)
        {
            prefab = Resources.Load( currentStructureName + "/" + t ) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning( "Couldn't load prefab for " + currentStructureName + " " + t );
                continue;
            }

            Instantiate( prefab, randomPositionInThrowableSpawnArea, Random.rotation );

            yield return new WaitForSeconds( Random.Range( 0.05f, 0.3f ) );
        }

        yield return null;
    }
}
