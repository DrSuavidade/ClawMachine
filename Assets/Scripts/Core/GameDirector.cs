#nullable enable
using UnityEngine;
using Project.Core;
using Project.Economy;
using Project.Inventory;

namespace Project
{
    /// <summary>Bootstraps services and scene-level state.</summary>
    public sealed class GameDirector : MonoBehaviour
    {
        [Header("Initial Balances")]
        [Min(0)] public int startingCredits = 0;
        [Min(0)] public int startingTickets = 0;

        private void Awake()
        {
            // Register services (singleton lifetime for now)
            var econ = new EconomyService(startingCredits, startingTickets);
            ServiceLocator.Register<EconomyService>(econ);

            var inv = new InventoryService();
            ServiceLocator.Register<InventoryService>(inv);

            // RNG could be injected here if needed
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }
    }
}
