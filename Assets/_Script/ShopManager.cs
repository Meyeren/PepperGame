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

    [Header("Highlight Settings")]
    public Color normalColor = Color.white;
    public Color highlightedColor = Color.yellow;
    public float highlightScale = 1.2f;
    [Range(0.1f, 1f)] public float scaleSpeed = 0.3f;

    private int selectedIndex = 0;
    private float inputCooldown = 0.3f;
    private float lastInputTime;
    private bool shopOpen = false;
    private bool isReturningToPlayer = false;
    private Coroutine cameraTransition;
    private Vector3[] originalScales;
    private Coroutine[] scaleCoroutines;

    private Vector3 mainCamSavedPosition;
    private Quaternion mainCamSavedRotation;

    void Start()
    {
        shopUI.SetActive(false);
        InitializeHighlightSystem();
    }

    void InitializeHighlightSystem()
    {
        originalScales = new Vector3[shopItems.Length];
        scaleCoroutines = new Coroutine[shopItems.Length];

        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i] != null)
            {
                originalScales[i] = shopItems[i].transform.localScale;
            }
        }
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
                ChangeSelection(1);
                lastInputTime = Time.time;
            }
            else if (nav.x < -0.5f)
            {
                ChangeSelection(-1);
                lastInputTime = Time.time;
            }
        }

        if (Gamepad.current?.buttonSouth.wasPressedThisFrame == true || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SelectItem();
        }
    }

    void ChangeSelection(int direction)
    {
        selectedIndex = (selectedIndex + direction + shopItems.Length) % shopItems.Length;
        UpdateSelection();
    }

    public void OpenShop()
    {
        shopOpen = true;
        selectedIndex = 0;
        isReturningToPlayer = false;

        mainCamSavedPosition = mainCamera.transform.position;
        mainCamSavedRotation = mainCamera.transform.rotation;

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
        isReturningToPlayer = true;

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
        // Stop alle igangværende skaleringseffekter
        if (scaleCoroutines != null)
        {
            foreach (var coroutine in scaleCoroutines)
            {
                if (coroutine != null) StopCoroutine(coroutine);
            }
        }

        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i] == null) continue;

            bool isSelected = (i == selectedIndex);

            // UI Highlight
            if (i < highlightFrames.Length)
            {
                highlightFrames[i].enabled = isSelected;
            }

            // 3D Object Highlight
            var renderers = shopItems[i].GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.color = isSelected ? highlightedColor : normalColor;
            }

            // Skaleringsanimation
            Vector3 targetScale = isSelected ? originalScales[i] * highlightScale : originalScales[i];
            scaleCoroutines[i] = StartCoroutine(SmoothScale(shopItems[i].transform, targetScale));
        }

        // Opdater beskrivelse
        var item = shopItems[selectedIndex].GetComponent<ShopItem>();
        if (item != null)
        {
            descriptionText.text = item.description;
        }
    }

    private IEnumerator SmoothScale(Transform target, Vector3 endScale)
    {
        Vector3 startScale = target.localScale;
        float elapsed = 0f;

        while (elapsed < scaleSpeed)
        {
            target.localScale = Vector3.Lerp(startScale, endScale, elapsed / scaleSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = endScale;
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

        if (isReturning && playerMovement != null)
        {
            playerMovement.SetCanMove(true);
            isReturningToPlayer = false;
        }
    }
}