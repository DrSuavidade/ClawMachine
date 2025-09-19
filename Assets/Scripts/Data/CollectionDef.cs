#nullable enable
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "Data/Collection", fileName = "Collection")]
    public sealed class CollectionDef : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; } = System.Guid.NewGuid().ToString();
        [field: SerializeField] public string DisplayName { get; private set; } = "Starter Collection";
        [field: SerializeField] public PrizeItemDef[] Items { get; private set; } = System.Array.Empty<PrizeItemDef>();
    }
}
