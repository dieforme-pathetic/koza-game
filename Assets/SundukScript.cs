using UnityEngine;
using TMPro;
using System.Collections;

public class ChestLock : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string correctCode = "1234";
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private TMP_Text displayText;
    
    [Header("Сундук")]
    [SerializeField] private GameObject chestObject;
    
    [Header("Предмет")]
    [SerializeField] private GameObject itemInside;
    
    private string currentInput = "";
    private bool isPlayerNear = false;
    private bool isOpen = false;
    private bool isLockActive = false;
    private Color originalColor;
    
    void Start()
    {
        lockPanel.SetActive(false);
        if (itemInside != null) itemInside.SetActive(false);
        
        originalColor = displayText.color;
        UpdateDisplay();
    }
    
    void Update()
    {
        if (isOpen) return;
        
        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isLockActive)
                    CloseLockPanel();
                else
                    OpenLockPanel();
            }
        }
        
        if (isLockActive)
        {
            HandleKeyboardInput();
            if (Input.GetKeyDown(KeyCode.Escape))
                CloseLockPanel();
        }
    }
    
    void HandleKeyboardInput()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                if (currentInput.Length < 6)
                    currentInput += i.ToString();
                UpdateDisplay();
                break;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateDisplay();
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
            SubmitCode();
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentInput = "";
            UpdateDisplay();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (isLockActive)
                CloseLockPanel();
        }
    }
    
    void OpenLockPanel()
    {
        isLockActive = true;
        lockPanel.SetActive(true);
        currentInput = "";
        UpdateDisplay();
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void CloseLockPanel()
    {
        isLockActive = false;
        lockPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void SubmitCode()
    {
        if (currentInput == correctCode)
            StartCoroutine(UnlockChest());
        else
            StartCoroutine(WrongCodeFeedback());
    }
    
    IEnumerator UnlockChest()
    {
        displayText.text = "!!!!";
        displayText.color = Color.green;
        yield return new WaitForSecondsRealtime(0.5f);
        
        isOpen = true;
        
        if (chestObject != null)
            chestObject.SetActive(false);
        
        if (itemInside != null)
            itemInside.SetActive(true);
        
        CloseLockPanel();
    }
    
    IEnumerator WrongCodeFeedback()
    {
        displayText.text = "!!!!";
        displayText.color = Color.red;
        yield return new WaitForSecondsRealtime(0.8f);
        currentInput = "";
        UpdateDisplay();
        displayText.color = originalColor;
    }
    
    void UpdateDisplay()
    {
        string stars = new string('*', currentInput.Length);
        displayText.text = stars;
    }
}