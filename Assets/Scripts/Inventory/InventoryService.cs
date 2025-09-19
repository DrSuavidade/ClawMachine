#nullable enable
using System.Collections.Generic;

namespace Project.Inventory
{
    /// <summary>Tracks collected prize IDs and basic collection completion.</summary>
    public sealed class InventoryService
    {
        private readonly HashSet<string> _ownedPrizeIds = new();
        public IReadOnlyCollection<string> Owned => _ownedPrizeIds;

        public bool Add(string prizeId) => _ownedPrizeIds.Add(prizeId);
        public bool Has(string prizeId) => _ownedPrizeIds.Contains(prizeId);
    }
}
