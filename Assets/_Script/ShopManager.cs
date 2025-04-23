using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items")]
    public GameObject[] shopItems; // De tre shop-items
    public Image[] highlightFrames; // UI-billeder til highlighting
    public TextMeshProUGUI descriptionText; // Text-felt til beskrivelse

    [Header("References")]
    public GameObject shopUI; // Canvas/panel for shoppen
    public Camera mainCamera;
    public Transform shopCameraTarget; // Hvor kamera skal hen ved shop
    public Transform shopSpawnPoint;
    public PlayerMovement playerMovement;

    [Header("Transition Settings")]
    public float transitionDuration = 1f;

    private int selectedIndex = 0;
    private float inputCooldown = 0.3f;
    private float lastInputTime;
    private bool shopOpen = false;
    private Coroutine cameraTransition;

    private Vector3 mainCamSavedPosition;
    private Quaternion mainCamSavedRotation;

    void Start()
    {
        shopUI.SetActive(false); // Shop skjult fra start
    }

    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame && !shopOpen)
        {
            OpenShop();
        }

        if (!shopOpen) return;

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

        if (Gamepad.current?.buttonSouth.wasPressedThisFrame == true || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SelectItem();
        }
    }

    public void OpenShop()
    {
        shopOpen = true;
        selectedIndex = 0;

        // GEM kamera-position FØRST (før alt andet)
        mainCamSavedPosition = mainCamera.transform.position;
        mainCamSavedRotation = mainCamera.transform.rotation;

        // Lås bevægelse HELT (inkl. fjern input/velocity)
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
            playerMovement.FreezePlayerImmediately(); // Kræver denne metode i PlayerMovement
        }

        // Flyt shop prefab til spawnpunkt
        transform.position = shopSpawnPoint.position;

        shopUI.SetActive(true);

        if (cameraTransition != null)
        {
            StopCoroutine(cameraTransition);
        }
        cameraTransition = StartCoroutine(SmoothCameraTransition(shopCameraTarget.position, shopCameraTarget.rotation));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var item in shopItems)
        {
            item.SetActive(true);
        }

        UpdateSelection();
    }

    public void SelectItem()
    {
        Debug.Log("Valgte: " + shopItems[selectedIndex].name);
        CloseShop();
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopUI.SetActive(false);

        if (cameraTransition != null)
        {
            StopCoroutine(cameraTransition);
        }
        cameraTransition = StartCoroutine(SmoothCameraTransition(mainCamSavedPosition, mainCamSavedRotation));

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

    private IEnumerator SmoothCameraTransition(Vector3 targetPos, Quaternion targetRot)
    {
        float elapsed = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
    }
}