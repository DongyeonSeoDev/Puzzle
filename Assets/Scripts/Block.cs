using UnityEngine;

public enum eBlockType
{
    RED, BLUE, PURPLE, ORANGE, GREEN
}

public class Block : MonoBehaviour
{
    public Vector3 position;
    public eBlockType blockType;

    public Block(Vector3 position, int type)
    {
        this.position = position;
        blockType = (eBlockType)type;
    }
}
