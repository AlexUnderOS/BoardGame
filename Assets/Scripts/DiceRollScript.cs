using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Vector3 startPosition;

    [Header("Roll Settings")]
    [SerializeField] private float maxRandForcVal = 20f;
    [SerializeField] private float startRollingForce = 900f;

    [Header("Safety")]
    [SerializeField] private float rollTimeout = 5f;

    float forceX, forceY, forceZ;
    float rollTimer = 0f;
    bool isRolling = false;

    public string diceFaceNum = "?";
    public bool isLanded = false;
    public bool firstThrow = false;

    [HideInInspector] public bool inputEnabled = true;
    [HideInInspector] public bool rolledThisTurn = false;

    void Awake()
    {
        startPosition = transform.position;
        Initialize();
    }

    void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.isKinematic = true;
        transform.rotation = Random.rotation;
    }

    void RollDice()
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
        if ((isLanded || !firstThrow) && !rolledThisTurn)
        {
            if (!firstThrow) firstThrow = true;

            rolledThisTurn = true;
            isLanded = false;
            diceFaceNum = "?";

            RollDice();
        }
    }

    public void ResetDice()
    {
        if (rBody == null) rBody = GetComponent<Rigidbody>();

        transform.position = startPosition;

        rBody.isKinematic = true;

        rBody.linearVelocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        isRolling = false;
        rollTimer = 0f;

        firstThrow = false;

        isLanded = false;
        rolledThisTurn = false;
        diceFaceNum = "?";

        transform.rotation = Random.rotation;

        Debug.Log("[Dice] Reset (firstThrow=false, rolledThisTurn=false)");
    }

    void Update()
    {
        if (rBody == null) return;

        if (inputEnabled &&
            Input.GetMouseButtonDown(0) &&
            (isLanded || !firstThrow) &&
            !rolledThisTurn)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) &&
                hit.collider != null &&
                hit.collider.gameObject == gameObject)
            {
                if (!firstThrow) firstThrow = true;

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
                Debug.LogWarning("[Dice] Roll timeout → force reset");
                isLanded = true;
                ResetDice();
            }
        }

        if (isLanded)
        {
            isRolling = false;
        }
    }
}
