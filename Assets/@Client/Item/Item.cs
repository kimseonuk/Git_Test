using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private float healAmount = 30f;

    private Action<Item> onPickedUp = null;

    public void Init(Action<Item> onPickedUp)
    {
        this.onPickedUp = onPickedUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 닿았을 때만 회복
        Player player = other.GetComponentInParent<Player>();
        if (player == null)
        {
            return;
        }

        player.Heal(healAmount);
        onPickedUp?.Invoke(this);
        Destroy(gameObject);
    }
}
