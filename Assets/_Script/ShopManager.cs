using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Item Prefabs")]
    public GameObject[] allItemPrefabs;

    [Header("Spawn Points")]
    public Transform[] spawnPositions;

    [Header("UI")]
    public Image[] highlightFrames;
    public TextMeshProUGUI descriptionText;
    public GameObject shopUI;

    [Header("References")]
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

    private List<GameObject> currentShopItems = new List<GameObject>();
    private List<string> lastUsedItemNames = new List<string>();

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
        originalScales = new Vector3[3];
        scaleCoroutines = new Coroutine[3];
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
        selectedIndex = (selectedIndex + direction + currentShopItems.Count) % currentShopItems.Count;
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

        if (cameraTransition != null) StopCoroutine(cameraTransition);
        cameraTransition = StartCoroutine(SmoothCameraTransition(shopCameraTarget.position, shopCameraTarget.rotation, false));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SpawnRandomItems();
        UpdateSelection();
    }

    void SpawnRandomItems()
    {
        foreach (var item in currentShopItems)
        {
            Destroy(item);
        }
        currentShopItems.Clear();

        var possibleItems = allItemPrefabs
            .Select(prefab => prefab.GetComponent<ShopItem>())
            .Where(item => item != null && Random.value * 100f <= item.spawnChance)
            .Where(item => item.canRepeat || !lastUsedItemNames.Contains(item.name))
            .OrderBy(x => Random.value)
            .Distinct()
            .Take(3)
            .ToList();

        lastUsedItemNames.Clear();

        for (int i = 0; i < possibleItems.Count && i < spawnPositions.Length; i++)
        {
            GameObject newItem = Instantiate(possibleItems[i].gameObject, spawnPositions[i].position, spawnPositions[i].rotation, transform);
            currentShopItems.Add(newItem);
            lastUsedItemNames.Add(possibleItems[i].name);
            originalScales[i] = newItem.transform.localScale;
        }
    }

    public void SelectItem()
    {
        Debug.Log("Valgte: " + currentShopItems[selectedIndex].name);
        CloseShop();
    }

    public void CloseShop()
    {
        shopOpen = false;
        shopUI.SetActive(false);
        isReturningToPlayer = true;

        if (cameraTransition != null) StopCoroutine(cameraTransition);
        cameraTransition = StartCoroutine(SmoothCameraTransition(mainCamSavedPosition, mainCamSavedRotation, true));

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateSelection()
    {
        if (scaleCoroutines != null)
        {
            foreach (var coroutine in scaleCoroutines)
            {
                if (coroutine != null) StopCoroutine(coroutine);
            }
        }

        for (int i = 0; i < currentShopItems.Count; i++)
        {
            bool isSelected = (i == selectedIndex);

            if (i < highlightFrames.Length)
            {
                highlightFrames[i].enabled = isSelected;
            }

            var renderers = currentShopItems[i].GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.color = isSelected ? highlightedColor : normalColor;
            }

            Vector3 targetScale = isSelected ? originalScales[i] * highlightScale : originalScales[i];
            scaleCoroutines[i] = StartCoroutine(SmoothScale(currentShopItems[i].transform, targetScale));
        }

        var item = currentShopItems[selectedIndex].GetComponent<ShopItem>();
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
