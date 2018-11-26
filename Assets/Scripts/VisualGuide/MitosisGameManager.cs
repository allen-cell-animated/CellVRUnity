using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitosisGameManager : MonoBehaviour 
{
    public string currentStructureName;
    public float waitBetweenThrowableSpawn = 0.2f;
    public float throwableSpawnHeight = 3f;
    public Vector2 throwableSpawnRingExtents = new Vector2( 0.5f, 0.6f );
    public float throwableSpawnScale = 0.4f;
    public float throwableBoundsRadius = 1.5f;
    public float targetHeight = 1.5f;
    public float targetDistanceFromCenter = 2.2f;

    string[] throwableNames = { "ProphaseCell", "PrometaphaseCell", "MetaphaseCell", "AnaphaseCell", "TelophaseCell"};
    ThrowableCell[] throwableCells;
    GameObject[] targets;
    float lastThrowableCheckTime = 5f;
    float timeBetweenThrowableChecks = 3f;
    int correctlyPlacedThrowables;
    int animationPhase;
    bool rePlaceWhenOutOfBounds = true;

    public void StartGame (string _structureName, float timeBeforeCellDrop)
    {
        correctlyPlacedThrowables = 0;
        currentStructureName = _structureName;
        SpawnWalls();
        SpawnTargetsAndArrows();
        StartCoroutine( SpawnThrowables( currentStructureName, timeBeforeCellDrop ) );
    }

    void Update ()
    {
        if (rePlaceWhenOutOfBounds)
        {
            PlaceThrowablesIfOutOfBounds();
        }
    }

    Vector3 randomPositionInThrowableSpawnArea
    {
        get
        {
            return Quaternion.Euler( 0, Random.Range( 0, 360f ), 0 ) * (Random.Range( throwableSpawnRingExtents.x, throwableSpawnRingExtents.y ) * Vector3.forward) + throwableSpawnHeight * Vector3.up;
        }
    }

    public IEnumerator SpawnAllThrowables (string[] structureNames)
    {
        rePlaceWhenOutOfBounds = false;
        for (int i = 0; i < structureNames.Length; i++)
        {
            StartCoroutine( SpawnThrowables( structureNames[i], i * structureNames.Length * waitBetweenThrowableSpawn ) );
        }

        yield return new WaitForSeconds( structureNames.Length * structureNames.Length * waitBetweenThrowableSpawn );

        throwableCells = GetComponentsInChildren<ThrowableCell>();
        rePlaceWhenOutOfBounds = true;
    }

    IEnumerator SpawnThrowables (string structureName, float waitTime)
    {
        yield return new WaitForSeconds( waitTime );

        GameObject prefab;
        throwableCells = new ThrowableCell[throwableNames.Length];
        for (int i = 0; i < throwableNames.Length; i++)
        {
            prefab = Resources.Load( structureName + "/" + throwableNames[i] ) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning( "Couldn't load prefab for " + structureName + " " + throwableNames[i] );
                continue;
            }

            throwableCells[i] = (Instantiate( prefab, transform ) as GameObject).GetComponent<ThrowableCell>();
            throwableCells[i].transform.position = transform.position + randomPositionInThrowableSpawnArea;
            throwableCells[i].transform.rotation = Random.rotation;
            throwableCells[i].transform.localScale = throwableSpawnScale * Vector3.one;

            yield return new WaitForSeconds( waitBetweenThrowableSpawn );
        }
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
                if (throwableCell != null && !throwableCell.boundToTarget && !throwableCell.isMoving && !throwableCell.IsGrabbed() && ThrowableIsOutOfBounds( throwableCell.transform ))
                {
                    throwableCell.ReleaseFromTarget( true );
                    StartCoroutine( PlaceThrowable( throwableCell.transform, 1f ) );
                }
            }
            lastThrowableCheckTime = Time.time;
        }
    }

    void SpawnTargetsAndArrows ()
    {
        GameObject targetPrefab = Resources.Load( "Target" ) as GameObject;
        if (targetPrefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for Target" );
            return;
        }
        GameObject arrowPrefab = Resources.Load( "Arrow" ) as GameObject;
        if (targetPrefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for Arrow" );
            return;
        }

        targets = new GameObject[throwableNames.Length + 1];
        Vector3 position = targetDistanceFromCenter * Vector3.forward;
        Quaternion dRotation = Quaternion.Euler( 0, 180f / (throwableNames.Length + 1f), 0 );
        position = dRotation * position;
        for (int i = 0; i < throwableNames.Length + 1; i++)
        {
            position = dRotation * position;
            targets[i] = Instantiate( targetPrefab, position + targetHeight * Vector3.up, Quaternion.LookRotation( -position, Vector3.up ), transform ) as GameObject;
            if (i < throwableNames.Length)
            {
                targets[i].name = throwableNames[i] + "Target";
            }
            else //interphase cell target
            {
                targets[i].GetComponent<SphereCollider>().enabled = false;
            }

            position = dRotation * position;
            Instantiate( arrowPrefab, position + targetHeight * Vector3.up, Quaternion.LookRotation( -position, Vector3.up ), transform );
        }
    }

    public IEnumerator TurnOffInterphaseCellTarget (float waitTime)
    {
        yield return new WaitForSeconds( waitTime );

        targets[throwableNames.Length].SetActive( false );
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
            VisualGuideManager.Instance.StartSuccessAnimation();
            foreach (ThrowableCell throwable in throwableCells)
            {
                throwable.isGrabbable = false;
            }
        }
    }

    public void RemoveCorrectHit ()
    {
        if (correctlyPlacedThrowables > 0)
        {
            correctlyPlacedThrowables--;
        }
    }

    public void AnimateNextPhase ()
    {
        if (animationPhase < throwableNames.Length)
        {
            if (animationPhase == 1)
            {
                VisualGuideManager.Instance.interphaseCell.gameObject.SetActive( false );
            }
            else if (animationPhase > 1)
            {
                throwableCells[animationPhase-2].gameObject.SetActive( false );
            }
            throwableCells[animationPhase].AnimateSuccess();
            animationPhase++;
        }
        else
        {
            throwableCells[animationPhase-2].gameObject.SetActive( false );
            StartCoroutine( WaitToTriggerMitoticCells() );
        }
    }

    IEnumerator WaitToTriggerMitoticCells ()
    {
        yield return new WaitForSeconds( 1f );

        throwableCells[throwableCells.Length-1].gameObject.SetActive( false );
        VisualGuideManager.Instance.TriggerMitoticCellsAnimation();
    }
}
