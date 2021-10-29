using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum eBlockType
{
    RED, BLUE, PURPLE, ORANGE, GREEN, UNICORN, BOMB, LEAF
}

public class Block : MonoBehaviour
{
    public eBlockType blockType;

    private EventTrigger trigger;
    private EventTrigger.Entry enter = new EventTrigger.Entry();

    private void Awake()
    {
        trigger = GetComponent<EventTrigger>();
        enter.eventID = EventTriggerType.PointerDown;

        enter.callback.AddListener((data) =>
        {
            Map.Move(this);
        });

        trigger.triggers.Add(enter);
    }
}