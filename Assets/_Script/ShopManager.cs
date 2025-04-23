using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items")]
    public GameObject[] shopItems;
    public Image[] highlightFrames;
    public TextMeshProUGUI descriptionText;

    [Header("References")]
    public GameObject shopUI;
    public Camera mainCamera;
    public Transform shopCameraTarget;
    public Transform shopSpawnPoint;
    public PlayerMovement playerMovement;

    [Header("Transition Settings")]
    public float transitionDuration = 1f;

    private int selectedIndex = 0;
    private float inputCooldown = 0.3f;
    private float lastInputTime;
    private bool shopOpen = false;
    private bool isReturningToPlayer = false; // NY: Tjekker om kamera er i gang med at returnere
    private Coroutine cameraTransition;

    private Vector3 mainCamSavedPosition;
    private Quaternion mainCamSavedRotation;

    void Start()
    {
        shopUI.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame && !shopOpen && !isReturningToPlayer)
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
        isReturningToPlayer = false;

        // Gem kamera-position FØR bevægelse låses
        mainCamSavedPosition = mainCamera.transform.position;
        mainCamSavedRotation = mainCamera.transform.rotation;

        // Frys spilleren HELT
        if (playerMovement != null)
        {
            playerMovement.FreezePlayerImmediately();
        }

        transform.position = shopSpawnPoint.position;
        shopUI.SetActive(true);

        if (cameraTransition != null)
        {
            StopCoroutine(cameraTransition);
        }
        cameraTransition = StartCoroutine(SmoothCameraTransition(shopCameraTarget.position, shopCameraTarget.rotation, false));

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
        isReturningToPlayer = true; // Marker at vi returnerer

        if (cameraTransition != null)
        {
            StopCoroutine(cameraTransition);
        }
        cameraTransition = StartCoroutine(SmoothCameraTransition(mainCamSavedPosition, mainCamSavedRotation, true));

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    private IEnumerator SmoothCameraTransition(Vector3 targetPos, Quaternion targetRot, bool isReturning)
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

        // Gendan kun bevægelse hvis vi er færdige med at returnere til spilleren
        if (isReturning && playerMovement != null)
        {
            playerMovement.SetCanMove(true);
            isReturningToPlayer = false;
        }
    }
}