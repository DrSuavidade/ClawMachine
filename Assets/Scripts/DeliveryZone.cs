using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var prize = other.GetComponentInParent<Prize>() ?? other.GetComponent<Prize>();
        if (prize)
        {
            TicketManager.Instance?.AddTickets(prize.ticketValue);
            Destroy(prize.gameObject);
        }
    }
}
