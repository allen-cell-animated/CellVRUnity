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
        SpawnTargets();
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

            Instantiate( prefab, randomPositionInThrowableSpawnArea, Random.rotation, transform );

            yield return new WaitForSeconds( Random.Range( 0.05f, 0.3f ) );
        }

        yield return null;
    }

    void SpawnTargets ()
    {
        GameObject prefab = Resources.Load( "Target" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for Target" );
            return;
        }

        Vector3 targetPosition = 2f * Vector3.forward;
        for (int i = 0; i < 6; i++)
        {
            Instantiate( prefab, targetPosition + Vector3.up, Quaternion.LookRotation( -targetPosition, Vector3.up ), transform );
            targetPosition = Quaternion.Euler( 0, 360f / 6f, 0 ) * targetPosition;
        }
    }
}
