using System.Collections;
using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    private Rigidbody rBody;

    [Header("Rest Point (where dice is reset/ready)")]
    [SerializeField] private Transform restPoint;

    [Header("Roll Settings")]
    [SerializeField] private float maxRandForcVal = 20f;
    [SerializeField] private float startRollingForce = 900f;

    [Header("Safety")]
    [SerializeField] private float rollTimeout = 5f;

    [Header("Ground Reset (Position Only)")]
    [SerializeField] private float resetMoveSpeed = 2.5f;
    [SerializeField] private float resetStopDistance = 0.02f;
    [SerializeField] private float resetYawToRest = 12f;

    private float forceX, forceY, forceZ;
    private float rollTimer = 0f;
    private bool isRolling = false;

    private Coroutine resetPosRoutine;

    public bool IsResettingPos => resetPosRoutine != null;

    public string diceFaceNum = "?";
    public bool isLanded = false;
    public bool firstThrow = false;

    [HideInInspector] public bool inputEnabled = true;
    [HideInInspector] public bool rolledThisTurn = false;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();

        if (restPoint == null)
            Debug.LogWarning("[Dice] restPoint is NOT assigned! Create an Empty on table and assign it.");

        ForceReadyForNextRoll();
    }

    private void RollDice()
    {
        rBody.isKinematic = false;

        forceX = Random.Range(0f, maxRandForcVal);
        forceY = Random.Range(0f, maxRandForcVal);
        forceZ = Random.Range(0f, maxRandForcVal);

        rBody.AddForce(Vector3.up * Random.Range(800f, startRollingForce), ForceMode.Force);
        rBody.AddTorque(forceX, forceY, forceZ, ForceMode.Force);

        rollTimer = 0f;
        isRolling = true;

        Debug.Log("[Dice] Roll started");
    }

    public void RollFromManager()
    {
        bool canRoll = (isLanded || !firstThrow) && !rolledThisTurn && !isRolling && !IsResettingPos;

        if (!canRoll)
        {
            Debug.Log($"[Dice] RollFromManager blocked: isLanded={isLanded}, firstThrow={firstThrow}, rolledThisTurn={rolledThisTurn}, isRolling={isRolling}, IsResettingPos={IsResettingPos}");
            return;
        }

        firstThrow = true;
        rolledThisTurn = true;

        isLanded = false;
        diceFaceNum = "?";

        RollDice();
    }


    public void ResetDice()
    {
        ForceReadyForNextRoll();
        Debug.Log("[Dice] Reset");
    }

    public void ForceReadyForNextRoll()
    {
        if (rBody == null) rBody = GetComponent<Rigidbody>();

        if (restPoint != null)
        {
            transform.position = restPoint.position;
            transform.rotation = restPoint.rotation;
        }

        rBody.isKinematic = true;

        rBody.linearVelocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        isRolling = false;
        rollTimer = 0f;

        isLanded = true;
        firstThrow = false;
        rolledThisTurn = false;
        diceFaceNum = "?";
    }

    public void ForceReadyForNextRollStateOnly()
    {
        if (rBody == null) rBody = GetComponent<Rigidbody>();

        rBody.isKinematic = true;

        rBody.linearVelocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        isRolling = false;
        rollTimer = 0f;

        isLanded = true;
        firstThrow = false;
        rolledThisTurn = false;
        diceFaceNum = "?";
    }

    public void ResetDicePos()
    {
        if (!isLanded)
        {
            Debug.Log("[Dice] ResetDicePos blocked: dice is not landed.");
            return;
        }

        if (restPoint == null)
        {
            Debug.LogWarning("[Dice] ResetDicePos blocked: restPoint is not assigned.");
            return;
        }

        if (resetPosRoutine != null)
            StopCoroutine(resetPosRoutine);

        resetPosRoutine = StartCoroutine(MoveDiceToRestPointGroundOnly());
        Debug.Log("[Dice] ResetDicePos: moving to rest point (ground only)");
    }

    private IEnumerator MoveDiceToRestPointGroundOnly()
    {
        bool wasKinematic = rBody.isKinematic;
        rBody.isKinematic = true;

        rBody.linearVelocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        float fixedY = transform.position.y;

        while (true)
        {
            Vector3 current = transform.position;
            Vector3 target = new Vector3(restPoint.position.x, fixedY, restPoint.position.z);

            float dist = Vector3.Distance(new Vector3(current.x, fixedY, current.z), target);
            if (dist <= resetStopDistance)
                break;

            transform.position = Vector3.MoveTowards(current, target, resetMoveSpeed * Time.deltaTime);

            if (resetYawToRest > 0f)
            {
                float y = Mathf.MoveTowardsAngle(transform.eulerAngles.y, restPoint.eulerAngles.y, resetYawToRest * Time.deltaTime);
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, y, transform.eulerAngles.z);
            }

            yield return null;
        }

        transform.position = new Vector3(restPoint.position.x, fixedY, restPoint.position.z);

        rBody.isKinematic = wasKinematic;
        resetPosRoutine = null;
    }

    private void Update()
    {
        if (rBody == null) return;

        if (inputEnabled &&
            Input.GetMouseButtonDown(0) &&
            (isLanded || !firstThrow) &&
            !rolledThisTurn &&
            !isRolling &&
            !IsResettingPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) &&
                hit.collider != null &&
                hit.collider.gameObject == gameObject)
            {
                firstThrow = true;
                rolledThisTurn = true;

                isLanded = false;
                diceFaceNum = "?";

                RollDice();
            }
        }

        if (isRolling && !isLanded)
        {
            rollTimer += Time.deltaTime;
            if (rollTimer >= rollTimeout)
            {
                Debug.LogWarning("[Dice] Roll timeout -> ForceReadyForNextRoll()");
                ForceReadyForNextRoll();
            }
        }

        if (isLanded) isRolling = false;
    }
}
