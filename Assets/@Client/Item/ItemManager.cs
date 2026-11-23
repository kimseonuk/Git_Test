using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab = null;
    [SerializeField]
    private Collider[] platformArray = null;   // 아이템이 생성될 발판들
    [SerializeField]
    private int maxItemCount = 3;
    [SerializeField]
    private float spawnInterval = 5f;
    [SerializeField]
    private float edgeMargin = 2f;            // 발판 가장자리(벽)에서 띄울 거리
    [SerializeField]
    private float spawnHeight = 0.5f;         // 발판 표면에서 띄울 높이

    private List<Item> itemList = null;

    private void Awake()
    {
        itemList = new List<Item>();
    }

    public void Init()
    {
        StartCoroutine(CoSpawnItems());
    }

    private IEnumerator CoSpawnItems()
    {
        while (true)
        {
            if (itemList.Count < maxItemCount)
            {
                SpawnItem();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnItem()
    {
        if (TryGetRandomPointOnPlatform(out Vector3 spawnPosition) == false)
        {
            return;
        }

        GameObject go
            = Instantiate(itemPrefab, spawnPosition, Quaternion.identity, this.transform);

        Item itemComponent = go.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.Init(OnItemPickedUp);
            itemList.Add(itemComponent);
        }
    }

    // 무작위 발판을 고르고, 그 발판 범위 안의 무작위 지점에서 아래로 레이를 쏴 표면 위치를 구함
    private bool TryGetRandomPointOnPlatform(out Vector3 point)
    {
        point = Vector3.zero;
        if (platformArray == null || platformArray.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < 10; i++)
        {
            Collider platform = platformArray[Random.Range(0, platformArray.Length)];
            Bounds bounds = platform.bounds;

            float marginX = Mathf.Min(edgeMargin, bounds.extents.x);
            float marginZ = Mathf.Min(edgeMargin, bounds.extents.z);
            float x = Random.Range(bounds.min.x + marginX, bounds.max.x - marginX);
            float z = Random.Range(bounds.min.z + marginZ, bounds.max.z - marginZ);

            Ray ray = new Ray(new Vector3(x, bounds.max.y + 10f, z), Vector3.down);
            if (platform.Raycast(ray, out RaycastHit hit, bounds.size.y + 20f))
            {
                point = hit.point + Vector3.up * spawnHeight;
                return true;
            }
        }
        return false;
    }

    private void OnItemPickedUp(Item item)
    {
        itemList.Remove(item);
    }
}
