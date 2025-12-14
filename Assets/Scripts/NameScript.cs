using TMPro;
using UnityEngine;

public class NameScript : MonoBehaviour
{
    TextMeshPro nicknameText;
    void Awake()
    {
        nicknameText = transform.Find("Nickname").gameObject.GetComponent<TextMeshPro>();
    }

    public void SetName(string name)
    {
        nicknameText.text = name;
        nicknameText.color = new Color32(
            (byte)Random.Range(0,255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }
}
