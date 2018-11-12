using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitosisGameManager : MonoBehaviour 
{
    public bool inPlayMode;
    public string currentStructureName;
    public Vector2 waitBetweenThrowableSpawn = new Vector2( 0.05f, 0.3f );
    public float throwableSpawnHeight = 1.5f;
    public Vector2 throwableSpawnRingExtents = new Vector2( 0.5f, 0.6f );
    public float throwableSpawnScale = 0.4f;
    public float throwableBoundsRadius = 2f;
    public float targetHeight = 1.5f;
    public float targetDistanceFromCenter = 1.5f;

    string[] throwableNames = { "ProphaseCell", "PrometaphaseCell", "MetaphaseCell", "AnaphaseCell", "TelophaseCell"};
    ThrowableCell[] throwableCells;
    float lastThrowableCheckTime = 5f;
    float timeBetweenThrowableChecks = 3f;

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

    void Update ()
    {
        PlaceThrowablesIfOutOfBounds();
    }

    Vector3 randomPositionInThrowableSpawnArea
    {
        get
        {
            return Quaternion.Euler( 0, Random.Range( 0, 360f ), 0 ) * (Random.Range( throwableSpawnRingExtents.x, throwableSpawnRingExtents.y ) * Vector3.forward) + throwableSpawnHeight * Vector3.up;
        }
    }

    IEnumerator SpawnThrowables ()
    {
        yield return new WaitForSeconds( 3f );

        GameObject prefab, clone;
        foreach (string t in throwableNames)
        {
            prefab = Resources.Load( currentStructureName + "/" + t ) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning( "Couldn't load prefab for " + currentStructureName + " " + t );
                continue;
            }

            clone = Instantiate( prefab, transform ) as GameObject;
            clone.transform.position = transform.position + randomPositionInThrowableSpawnArea;
            clone.transform.rotation = Random.rotation;
            clone.transform.localScale = throwableSpawnScale * Vector3.one;

            yield return new WaitForSeconds( Random.Range( waitBetweenThrowableSpawn.x, waitBetweenThrowableSpawn.y ) );
        }

        throwableCells = GetComponentsInChildren<ThrowableCell>();
    }

    IEnumerator PlaceThrowable (Transform throwable, float waitTime)
    {
        yield return new WaitForSeconds( waitTime );

        throwable.position = transform.position + randomPositionInThrowableSpawnArea;
        throwable.rotation = Random.rotation;
        throwable.localScale = throwableSpawnScale * Vector3.one;
    }

    bool ThrowableIsOutOfBounds (Transform throwable)
    {
        Vector3 throwablePositionOnFloor = throwable.position - transform.position;
        throwablePositionOnFloor.y = 0;
        return throwablePositionOnFloor.magnitude > throwableBoundsRadius;
    }

    void PlaceThrowablesIfOutOfBounds ()
    {
        if (Time.time - lastThrowableCheckTime > timeBetweenThrowableChecks && throwableCells != null)
        {
            foreach (ThrowableCell throwableCell in throwableCells)
            {
                if (!throwableCell.boundToTarget && !throwableCell.isMoving && throwableCell.GetGrabbingObject() == null && ThrowableIsOutOfBounds( throwableCell.transform ))
                {
                    throwableCell.ReleaseFromTarget( true );
                    StartCoroutine( PlaceThrowable( throwableCell.transform, 1f ) );
                }
            }
            lastThrowableCheckTime = Time.time;
        }
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
        Vector3 targetPosition = targetDistanceFromCenter * Vector3.forward;
        for (int i = 0; i < throwableNames.Length; i++)
        {
            targetPosition = Quaternion.Euler( 0, 360f / (throwableNames.Length + 1f), 0 ) * targetPosition;
            target = Instantiate( prefab, targetPosition + targetHeight * Vector3.up, Quaternion.LookRotation( -targetPosition, Vector3.up ), transform ) as GameObject;
            target.name = throwableNames[i] + "Target";
        }
    }
}
