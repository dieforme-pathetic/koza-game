using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    private SelectableSquare[] allSquares;
    private int currentIndex = 0;

    void Start()
    {
        allSquares = FindObjectsOfType<SelectableSquare>();
        
        if (allSquares.Length > 0)
        {
            allSquares[currentIndex].Select();
        }
    }

    void Update()
    {
        if (allSquares.Length == 0) return;
        if (Keyboard.current == null) return;
        
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            allSquares[currentIndex].Deselect();
            currentIndex = (currentIndex + 1) % allSquares.Length;
            allSquares[currentIndex].Select();
            Debug.Log("Выбран квадрат: " + currentIndex);
        }
        
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            allSquares[currentIndex].Deselect();
            currentIndex = (currentIndex - 1 + allSquares.Length) % allSquares.Length;
            allSquares[currentIndex].Select();
            Debug.Log("Выбран квадрат: " + currentIndex);
        }
    }
}