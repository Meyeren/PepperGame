using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Fixed Shop Items")]
    public GameObject healthUpPrefab;
    public GameObject energyUpPrefab;
    public GameObject damageUpPrefab;

    
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
    private Coroutine cameraTransition;
    private Vector3[] originalScales;
    private Coroutine[] scaleCoroutines;

    private Vector3 mainCamSavedPosition;
    private Quaternion mainCamSavedRotation;

    public bool canOpenShop;

    GameObject player;

    void Start()
    {
        shopUI.SetActive(false);
        originalScales = new Vector3[3];
        scaleCoroutines = new Coroutine[3];
        player = GameObject.FindGameObjectWithTag("Player");
        canOpenShop = true;
    }

    void Update()
    {
        if (!shopOpen) return;
        if (Gamepad.current != null)
        {
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
        }
        else if (Gamepad.current == null)
        {
            HandleMouseHover();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                SelectItem();
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
        if (Gamepad.current == null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        shopOpen = true;
        canOpenShop = false;
        selectedIndex = 0;

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


        SpawnFixedItems();
        UpdateSelection();
    }

    void SpawnFixedItems()
    {
        foreach (var item in currentShopItems)
        {
            Destroy(item);
        }
        currentShopItems.Clear();
        lastUsedItemNames.Clear();

        GameObject[] fixedPrefabs = { healthUpPrefab, energyUpPrefab, damageUpPrefab };

        for (int i = 0; i < fixedPrefabs.Length && i < spawnPositions.Length; i++)
        {
            GameObject newItem = Instantiate(fixedPrefabs[i], spawnPositions[i].position, spawnPositions[i].rotation, transform);
            currentShopItems.Add(newItem);
            lastUsedItemNames.Add(fixedPrefabs[i].name);
            originalScales[i] = newItem.transform.localScale;
        }
    }

    public void SelectItem()
    {
        string itemName = currentShopItems[selectedIndex].name;

        if (itemName.Contains("HealthUp"))
        {
            Debug.Log("Health");
            player.GetComponent<Combat>().MaxPlayerHealth += 10;
            player.GetComponent<Combat>().playerHealth += 10;
        }
        else if (itemName.Contains("EnergyUp"))
        {
            Debug.Log("Stamina");
            player.GetComponent<PlayerMovement>().maxStamina += 10f;
        }
        else if (itemName.Contains("DamageUp"))
        {
            Debug.Log("Damage");
            player.GetComponent<Combat>().basicDamage += 5f;
        }

        CloseShop();
    }

    public void CloseShop()
    {
        if (Gamepad.current == null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        shopOpen = false;
        shopUI.SetActive(false);

        if (cameraTransition != null) StopCoroutine(cameraTransition);
        cameraTransition = StartCoroutine(SmoothCameraTransition(mainCamSavedPosition, mainCamSavedRotation, true));

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
        }
    }


    void HandleMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            for (int i = 0; i < currentShopItems.Count; i++)
            {
                if (hit.transform.gameObject == currentShopItems[i])
                {
                    if (selectedIndex != i)
                    {
                        selectedIndex = i;
                        UpdateSelection();
                    }
                    break;
                }
            }
        }
    }
}
