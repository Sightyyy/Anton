using UnityEngine;

[DisallowMultipleComponent]
public class LayerSorting : MonoBehaviour
{
    [Header("Player (auto-find by tag if null)")]
    public Transform player;

    [Header("Sorting settings")]
    public int lowerSortingOrder = 1;
    public int upperSortingOrder = 4;
    [Tooltip("Jika (player.y - object.y) >= yThreshold -> pakai upperSortingOrder")]
    public float yThreshold = 1.5f;

    private SpriteRenderer spriteRenderer;
    private int lastSortingOrder = int.MinValue;

    private void Awake()
    {
        Transform visual = transform.Find("Visual");
        if (visual != null)
            spriteRenderer = visual.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogWarning($"LayerSorting: SpriteRenderer tidak ditemukan pada {gameObject.name} (tidak ada 'Visual' dan tidak ada SpriteRenderer).");

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else Debug.LogWarning("LayerSorting: Player with tag 'Player' not found in scene.");
        }

        UpdateSortingImmediate();
    }

    private void Update()
    {
        if (player == null || spriteRenderer == null) return;
        UpdateSorting();
    }

    private void UpdateSortingImmediate()
    {
        if (player == null || spriteRenderer == null) return;
        float dy = player.position.y - transform.position.y;
        int order = (dy >= yThreshold) ? upperSortingOrder : lowerSortingOrder;
        spriteRenderer.sortingOrder = order;
        lastSortingOrder = order;
    }

    private void UpdateSorting()
    {
        float dy = player.position.y - transform.position.y;
        int desired = (dy >= yThreshold) ? upperSortingOrder : lowerSortingOrder;

        if (desired != lastSortingOrder)
        {
            spriteRenderer.sortingOrder = desired;
            lastSortingOrder = desired;
        }
    }
}
