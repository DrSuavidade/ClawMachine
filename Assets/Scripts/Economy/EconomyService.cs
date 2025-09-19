#nullable enable
using System;
using UnityEngine;

namespace Project.Economy
{
    /// <summary>Manages credits and tickets.</summary>
    public sealed class EconomyService
    {
        public event Action<int>? OnCreditsChanged;
        public event Action<int>? OnTicketsChanged;

        public int Credits { get; private set; }
        public int Tickets { get; private set; }

        public EconomyService(int credits, int tickets)
        {
            Credits = Mathf.Max(0, credits);
            Tickets = Mathf.Max(0, tickets);
        }

        public bool SpendCredits(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (Credits < amount) return false;
            Credits -= amount;
            OnCreditsChanged?.Invoke(Credits);
            return true;
        }

        public void AddCredits(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            Credits += amount;
            OnCreditsChanged?.Invoke(Credits);
        }

        public bool SpendTickets(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (Tickets < amount) return false;
            Tickets -= amount;
            OnTicketsChanged?.Invoke(Tickets);
            return true;
        }

        public void AddTickets(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            Tickets += amount;
            OnTicketsChanged?.Invoke(Tickets);
        }
    }
}
