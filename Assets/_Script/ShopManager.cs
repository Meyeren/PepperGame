using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items")]
    public GameObject[] shopItems; // De tre shop-items
    public Image[] highlightFrames; // UI-billeder til highlighting
    public Text descriptionText;

    [Header("References")]
    public GameObject shopUI;
    public Camera shopCamera;
    public Camera mainCamera;
    public Transform shopSpawnPoint;
    public PlayerMovement playerMovement;

    private int selectedIndex = 0;
    private float inputCooldown = 0.3f;
    private float lastInputTime;
    private bool shopOpen = false;

    void Start()
    {
        shopUI.SetActive(false);
        shopCamera.enabled = false;
        mainCamera.enabled = true;
    }

    void Update()
    {
        // Midlertidig: tryk på L for at åbne shop (simuler runde er færdig)
        if (Keyboard.current.lKey.wasPressedThisFrame && !shopOpen)
        {
            OpenShop();
        }

        if (!shopOpen) return;

        // Controller navigation
        Vector2 nav = Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;

        if (Time.time - lastInputTime > inputCooldown)
        {
            if (nav.x > 0.5f)
            {
                selectedIndex = (selectedIndex + 1) % shopItems.Length;
                UpdateSelection();
                lastInputTime = Time.time;
            }
            else if (nav.x < -0.5f)
            {
                selectedIndex = (selectedIndex - 1 + shopItems.Length) % shopItems.Length;
                UpdateSelection();
                lastInputTime = Time.time;
            }
        }

        // Vælg item med controller
        if (Gamepad.current?.buttonSouth.wasPressedThisFrame == true) // A-knap
        {
            SelectItem();
        }
    }

    public void OpenShop()
    {
        shopOpen = true;
        selectedIndex = 0;

        // Flyt shoppen til midten
        transform.position = shopSpawnPoint.position;

        shopUI.SetActive(true);
        shopCamera.enabled = true;
        mainCamera.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerMovement.SetCanMove(false);
        UpdateSelection();
    }

    public void SelectItem()
    {
        Debug.Log("Valgte: " + shopItems[selectedIndex].name);

        CloseShop();

        // Start næste bølge (her kan du kalde din wave manager)
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopUI.SetActive(false);
        shopCamera.enabled = false;
        mainCamera.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerMovement.SetCanMove(true);
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < highlightFrames.Length; i++)
        {
            highlightFrames[i].enabled = (i == selectedIndex);
        }

        var item = shopItems[selectedIndex].GetComponent<ShopItem>();
        if (item != null)
        {
            descriptionText.text = item.description;
        }
    }
}
