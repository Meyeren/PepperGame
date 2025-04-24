using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [Header("Shop Info")]
    public string description;
    [Range(0, 100)] public float spawnChance = 100f;
    public bool canRepeat = false;
}
