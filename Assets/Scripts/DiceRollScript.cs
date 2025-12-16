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

    private float forceX, forceY, forceZ;
    private float rollTimer = 0f;
    private bool isRolling = false;

    public string diceFaceNum = "?";
    public bool isLanded = false;
    public bool firstThrow = false;

    [HideInInspector] public bool inputEnabled = true;
    [HideInInspector] public bool rolledThisTurn = false;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();

        if (restPoint == null)
        {
            Debug.LogWarning("[Dice] restPoint is NOT assigned! Create an Empty on table and assign it.");
        }

        ForceReadyForNextRoll();
    }

    private void RollDice()
    {
        rBody.isKinematic = false;

        forceX = Random.Range(0f, maxRandForcVal);
        forceY = Random.Range(0f, maxRandForcVal);
        forceZ = Random.Range(0f, maxRandForcVal);

        rBody.AddForce(Vector3.up * Random.Range(800f, startRollingForce));
        rBody.AddTorque(forceX, forceY, forceZ);

        rollTimer = 0f;
        isRolling = true;

        Debug.Log("[Dice] Roll started");
    }

    public void RollFromManager()
    {
        bool canRoll = (isLanded || !firstThrow) && !rolledThisTurn && !isRolling;

        if (!canRoll)
        {
            Debug.Log($"[Dice] RollFromManager blocked: isLanded={isLanded}, firstThrow={firstThrow}, rolledThisTurn={rolledThisTurn}, isRolling={isRolling}");
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

        Debug.Log("[Dice] ForceReadyForNextRoll (landed=true, firstThrow=false)");
    }

    private void Update()
    {
        if (rBody == null) return;

        if (inputEnabled &&
            Input.GetMouseButtonDown(0) &&
            (isLanded || !firstThrow) &&
            !rolledThisTurn &&
            !isRolling)
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
