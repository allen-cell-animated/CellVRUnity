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

    string[] throwableNames = { "Prophase", "Prometaphase", "Metaphase", "Anaphase", "Telophase"};
    ThrowableCell[] throwableCells;
    Target[] targets;
    GameObject[] arrows;
    float lastThrowableCheckTime = 5f;
    float timeBetweenThrowableChecks = 3f;
    int correctlyPlacedThrowables;
    int animationPhase;
    bool destroyWhenOutOfBounds;

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
        CheckIfThrowablesOutOfBounds();
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
        yield return new WaitForSeconds( 4f );
            
        destroyWhenOutOfBounds = true;
        for (int i = 0; i < structureNames.Length; i++)
        {
            StartCoroutine( SpawnThrowables( structureNames[i], i * structureNames.Length * waitBetweenThrowableSpawn, 1 ) );
        }

        yield return new WaitForSeconds( structureNames.Length * structureNames.Length * waitBetweenThrowableSpawn );

        throwableCells = GetComponentsInChildren<ThrowableCell>();
    }

    IEnumerator SpawnThrowables (string structureName, float waitTime, int colorSet = -1)
    {
        yield return new WaitForSeconds( waitTime );

        GameObject prefab;
        throwableCells = new ThrowableCell[throwableNames.Length];
        for (int i = 0; i < throwableNames.Length; i++)
        {
            prefab = Resources.Load( structureName + "/" + throwableNames[i] + "Cell" ) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning( "Couldn't load prefab for " + structureName + " " + throwableNames[i] );
                continue;
            }

            throwableCells[i] = (Instantiate( prefab, transform ) as GameObject).GetComponent<ThrowableCell>();
            throwableCells[i].transform.position = transform.position + randomPositionInThrowableSpawnArea;
            throwableCells[i].transform.rotation = Random.rotation;
            throwableCells[i].transform.localScale = throwableSpawnScale * Vector3.one;

            if (colorSet >= 0)
            {
                throwableCells[i].colorer.SetColor( colorSet );
            }

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

    void CheckIfThrowablesOutOfBounds ()
    {
        if (throwableCells != null && Time.time - lastThrowableCheckTime > timeBetweenThrowableChecks)
        {
            foreach (ThrowableCell throwableCell in throwableCells)
            {
                if (throwableCell != null && throwableCell.attachedTarget == null 
                    && (!throwableCell.isMoving || throwableCell.transform.position.y < 5f) 
                    && !throwableCell.IsGrabbed() && ThrowableIsOutOfBounds( throwableCell.transform ))
                {
                    if (destroyWhenOutOfBounds)
                    {
                        Destroy( throwableCell.gameObject );
                    }
                    else
                    {
                        throwableCell.ReleaseFromTarget( true );
                        StartCoroutine( PlaceThrowable( throwableCell.transform, 1f ) );
                    }
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

        targets = new Target[throwableNames.Length + 1];
        arrows = new GameObject[throwableNames.Length + 1];
        Vector3 position = targetDistanceFromCenter * Vector3.forward;
        Quaternion dRotation = Quaternion.Euler( 0, 180f / (throwableNames.Length + 1f), 0 );
        position = dRotation * position;
        for (int i = 0; i < throwableNames.Length + 1; i++)
        {
            position = dRotation * position;
            targets[i] = (Instantiate( targetPrefab, position + targetHeight * Vector3.up, Quaternion.LookRotation( -position, Vector3.up ), transform ) as GameObject).GetComponent<Target>();
            if (i < throwableNames.Length)
            {
                targets[i].SetGoalName( throwableNames[i] );
            }
            else //interphase cell target
            {
                targets[i].theCollider.enabled = false;
            }

            position = dRotation * position;
            arrows[i] = Instantiate( arrowPrefab, position + targetHeight * Vector3.up, Quaternion.LookRotation( -position, Vector3.up ), transform );
        }
    }

    public IEnumerator TurnOffInterphaseCellTarget (float waitTime)
    {
        yield return new WaitForSeconds( waitTime );

        targets[throwableNames.Length].Bind();
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
            VisualGuideManager.Instance.EnterSuccessMode();
            SetColorsetForThrowables( 1 );
            SetThrowablesGrabbable( false );
        }
    }

    void SetThrowablesGrabbable (bool grabbable)
    {
        foreach (ThrowableCell cell in throwableCells)
        {
            cell.isGrabbable = grabbable;
        }
    }

    void SetColorsetForThrowables (int colorSet)
    {
        foreach (ThrowableCell cell in throwableCells)
        {
            cell.colorer.SetColor( colorSet );
        }
    }

    public void RemoveCorrectHit ()
    {
        if (correctlyPlacedThrowables > 0)
        {
            correctlyPlacedThrowables--;
        }
    }

    public IEnumerator AnimateCellsForSuccess ()
    {
        for (int i = 0; i < throwableCells.Length; i++)
        {
            yield return new WaitForSeconds( 0.1f * (i + 1) );

            VisualGuideManager.Instance.AnimateSuccess( throwableCells[i].gameObject );
        }
    }
}
