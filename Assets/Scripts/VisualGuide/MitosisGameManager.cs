using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitosisGameManager : MonoBehaviour 
{
    public string currentStructureName;
    public Vector2 waitBetweenThrowableSpawn = new Vector2( 0.05f, 0.3f );
    public float throwableSpawnHeight = 3f;
    public Vector2 throwableSpawnRingExtents = new Vector2( 0.5f, 0.6f );
    public float throwableSpawnScale = 0.4f;
    public float throwableBoundsRadius = 1.5f;
    public float targetHeight = 1.5f;
    public float targetDistanceFromCenter = 2.2f;

    string[] throwableNames = { "ProphaseCell", "PrometaphaseCell", "MetaphaseCell", "AnaphaseCell", "TelophaseCell"};
    ThrowableCell[] throwableCells;
    float lastThrowableCheckTime = 5f;
    float timeBetweenThrowableChecks = 3f;
    int correctlyPlacedThrowables;

    public void StartGame (string _structureName, float timeBeforeCellDrop)
    {
        correctlyPlacedThrowables = 0;
        currentStructureName = _structureName;
        SpawnWalls();
        SpawnTargets();
        StartCoroutine( SpawnThrowables( timeBeforeCellDrop ) );
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

    IEnumerator SpawnThrowables (float waitTime)
    {
        yield return new WaitForSeconds( waitTime );

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
        if (throwableCells != null && Time.time - lastThrowableCheckTime > timeBetweenThrowableChecks)
        {
            foreach (ThrowableCell throwableCell in throwableCells)
            {
                if (!throwableCell.boundToTarget && !throwableCell.isMoving && !throwableCell.IsGrabbed() && ThrowableIsOutOfBounds( throwableCell.transform ))
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

    void SpawnWalls ()
    {
        GameObject prefab = Resources.Load( "Wall" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for Wall" );
            return;
        }

        Vector3 wallPosition = 16f * Vector3.forward;
        for (int i = 0; i < 6; i++)
        {
            wallPosition = Quaternion.Euler( 0, 360f / 6f, 0 ) * wallPosition;
            Instantiate( prefab, wallPosition + 20f * Vector3.up, Quaternion.LookRotation( -wallPosition, Vector3.up ), transform );
        }
    }

    public void RecordCorrectHit ()
    {
        correctlyPlacedThrowables++;
        if (correctlyPlacedThrowables >= throwableNames.Length)
        {
            StartAnimateSuccess();
        }
    }

    void StartAnimateSuccess ()
    {
        //turn off throwable cells and targets
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive( false );
        }

        //create mitosis animation
        GameObject prefab = Resources.Load( currentStructureName + "/MitoticCells" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for MitoticCells!" );
        }
        Animator mitoticCellsAnimation = (Instantiate( prefab, transform.position, transform.rotation, transform ) as GameObject).GetComponent<Animator>();
        mitoticCellsAnimation.SetTrigger( "Play" );
    }

    public void FinishAnimateSuccess ()
    {
        VisualGuideManager.Instance.CompleteGame( currentStructureName );
    }

    public void RemoveCorrectHit ()
    {
        if (correctlyPlacedThrowables > 0)
        {
            correctlyPlacedThrowables--;
        }
    }
}
