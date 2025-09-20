using TMPro;
using UnityEngine;

public class TicketManager : MonoBehaviour
{
    public static TicketManager Instance { get; private set; }

    public int Tickets { get; private set; } = 0;
    public TextMeshProUGUI ticketsText;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddTickets(int amount)
    {
        Tickets += amount;
        if (ticketsText) ticketsText.text = $"🎟 Tickets: {Tickets}";
    }
}
