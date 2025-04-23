using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items")]
    public GameObject[] shopItems; // De tre shop-items
    public Image[] highlightFrames; // UI-billeder til highlighting
    public TextMeshProUGUI descriptionText; // <- her indsætter du din desc Text i Inspector

    [Header("References")]
    public GameObject shopUI; // <- dette skal være hele din shop canvas/panel
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
        shopUI.SetActive(false); // shop skjult fra start
        shopCamera.enabled = false;
        mainCamera.enabled = true;
    }

    void Update()
    {
        // Åbn shop når man trykker L
        if (Keyboard.current.lKey.wasPressedThisFrame && !shopOpen)
        {
            OpenShop();
        }

        if (!shopOpen) return;

        // Controller navigation (venstre analog)
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

        // Vælg med controller (A-knap)
        if (Gamepad.current?.buttonSouth.wasPressedThisFrame == true)
        {
            SelectItem();
        }
    }

    public void OpenShop()
    {
        shopOpen = true;
        selectedIndex = 0;

        // Flyt hele shop-objektet til spawnpunktet (hvis nødvendigt)
        transform.position = shopSpawnPoint.position;

        shopUI.SetActive(true);
        shopCamera.enabled = true;
        mainCamera.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        // 🔍 Debug + aktiver items
        foreach (var item in shopItems)
        {
            Debug.Log("Aktiverer: " + item.name);
            item.SetActive(true);
        }

        UpdateSelection();
    }

    public void SelectItem()
    {
        Debug.Log("Valgte: " + shopItems[selectedIndex].name);

        CloseShop();

        // HER kan du kalde fx:
        // FindObjectOfType<WaveManager>().StartNextWave();
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopUI.SetActive(false);
        shopCamera.enabled = false;
        mainCamera.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }
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
