using System.Collections;
using UnityEngine;

public class TokenMover : MonoBehaviour
{
    public int CurrentIndex { get; private set; }

    [Header("Visual Offset")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 0.3f, 0f);

    public void SetOffset(Vector3 offset)
    {
        positionOffset = offset;
    }

    [SerializeField] private float moveSpeed = 6f;

    public void SetPosition(Transform cell, int index)
    {
        transform.position = cell.position + positionOffset;
        CurrentIndex = index;
    }

    public IEnumerator MoveSteps(Transform[] cells, int steps)
    {
        Debug.Log($"[TokenMover] Moving {steps} steps");

        for (int i = 0; i < steps; i++)
        {
            int next = Mathf.Min(CurrentIndex + 1, cells.Length - 1);

            Vector3 start = transform.position;
            Vector3 end = cells[next].position + positionOffset;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            CurrentIndex = next;
            Debug.Log($"[TokenMover] Reached cell {CurrentIndex}");
        }
    }

}
