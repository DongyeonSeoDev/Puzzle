using UnityEngine;
using UnityEngine.UI;

public enum eBlockType
{
    RED, BLUE, PURPLE, ORANGE, GREEN, UNICORN, BOMB, LEAF
}

public class Block : MonoBehaviour
{
    public eBlockType blockType;
    private Button button = null;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Map.Move(this);
        });
    }
}
