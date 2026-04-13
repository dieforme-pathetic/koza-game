using UnityEngine;
using UnityEngine.InputSystem;

public class SquareMovement : MonoBehaviour
{
    public float speed = 5f;
    private SelectableSquare selectable;

    void Start()
    {
        selectable = GetComponent<SelectableSquare>();
    }

    void Update()
    {
        if (selectable == null || !selectable.IsSelected()) return;
        
        float moveX = 0f;
        float moveY = 0f;
        
        if (Keyboard.current == null) return;
        
        if (Keyboard.current.aKey.isPressed)
            moveX = -1f;
        else if (Keyboard.current.dKey.isPressed)
            moveX = 1f;
        
        if (Keyboard.current.wKey.isPressed)
            moveY = 1f;
        else if (Keyboard.current.sKey.isPressed)
            moveY = -1f;
        
        if (moveX != 0) moveY = 0;
        
        transform.Translate(new Vector2(moveX, moveY) * speed * Time.deltaTime);
    }
}