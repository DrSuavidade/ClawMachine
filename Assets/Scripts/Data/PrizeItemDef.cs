#nullable enable
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "Data/Prize Item", fileName = "PrizeItemDef")]
    public sealed class PrizeItemDef : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; } = System.Guid.NewGuid().ToString();
        [field: SerializeField] public string DisplayName { get; private set; } = "Prize";
        [field: SerializeField] public GameObject PrizePrefab { get; private set; } = default!;
        [field: SerializeField, Min(0)] public int CreditReward { get; private set; } = 0;
        [field: SerializeField, Min(0)] public int TicketReward { get; private set; } = 0;
        [TextArea] public string Description;
    }
}
