using UnityEngine;
public class DragMovement : MonoBehaviour
{
    private Vector3 mouseOffset;
    private float mouseZ;
    private bool isDragging = false;

    void OnMouseDown()
    {
        mouseZ = Camera.main.WorldToScreenPoint(transform.position).z;
        mouseOffset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPos = GetMouseWorldPos() + mouseOffset;
            transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZ;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}