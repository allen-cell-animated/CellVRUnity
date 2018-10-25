using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitosisGameManager : MonoBehaviour 
{
    public bool inPlayMode;
    public string currentStructureName;
    public Bounds throwableSpawnArea;

    string[] throwableNames = {"ProphaseCell", "PrometaphaseCell", "MetaphaseCell", "AnaphaseCell", "TelophaseCell"};

    static MitosisGameManager _Instance;
    public static MitosisGameManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<MitosisGameManager>();
            }
            return _Instance;
        }
    }

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
        yield return new WaitForSeconds( 3f );

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

        GameObject target;
        Vector3 targetPosition = 1.5f * Vector3.forward;
        for (int i = 0; i < 5; i++)
        {
            targetPosition = Quaternion.Euler( 0, 360f / 6f, 0 ) * targetPosition;
            target = Instantiate( prefab, targetPosition + Vector3.up, Quaternion.LookRotation( -targetPosition, Vector3.up ), transform ) as GameObject;
            target.name = throwableNames[i] + "Target";
        }
    }
}
