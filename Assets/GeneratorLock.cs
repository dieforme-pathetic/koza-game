using UnityEngine;
using TMPro;
using System.Collections;

public class GeneratorLock : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string correctCode = "5678";
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private TMP_Text displayText;
    
    [Header("Сундук")]
    [SerializeField] private GameObject chestObject;
    
    [Header("Генератор")]
    [SerializeField] private Generator targetGenerator;
    
    private string currentInput = "";
    private bool isPlayerNear = false;
    private bool isOpen = false;
    private bool isLockActive = false;
    private Color originalColor;
    
    void Start()
    {
        lockPanel.SetActive(false);
        originalColor = displayText.color;
        UpdateDisplay();
    }
    
    void Update()
    {
        if (isOpen) return;
        
        // ПРОВЕРКА: игрок рядом и нажал E
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Игрок нажал E на втором сундуке");
            
            // Проверяем, есть ли у игрока рубильник
            PickupItem item = FindObjectOfType<PickupItem>();
            if (item != null && item.IsPickedUp())
            {
                Debug.Log("Рубильник есть! Открываем панель");
                OpenLockPanel();
            }
            else
            {
                Debug.Log("Нет рубильника! Нужно сначала получить рубильник из первого сундука");
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
        {
            isPlayerNear = true;
            Debug.Log("Коза рядом со вторым сундуком");
        }
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
        Debug.Log("Панель замка открыта");
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
        Debug.Log($"Введён код: {currentInput}, правильный: {correctCode}");
        
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
        
        // Удаляем рубильник у игрока
        PickupItem item = FindObjectOfType<PickupItem>();
        if (item != null)
        {
            Destroy(item.gameObject);
            Debug.Log("Рубильник использован и исчез");
        }
        
        // Включаем генератор
        if (targetGenerator != null)
        {
            targetGenerator.Activate();
            Debug.Log("Генератор активирован!");
        }
        else
        {
            Debug.LogError("Target Generator не назначен в инспекторе!");
        }
        
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