#nullable enable
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Project.UI
{
    /// <summary>Minimal HUD binding via exposed methods (hook up in inspector).</summary>
    public sealed class HUDController : MonoBehaviour
    {
        public TMP_Text machineNameText = default!;
        public TMP_Text creditsText = default!;
        public TMP_Text ticketsText = default!;
        public TMP_Text hintText = default!;

        public void SetMachineName(string name_) { if (machineNameText) machineNameText.text = name_; }
        public void SetCredits(int v) { if (creditsText) creditsText.text = $"Credits: {v}"; }
        public void SetTickets(int v) { if (ticketsText) ticketsText.text = $"Tickets: {v}"; }
        public void SetHint(string msg) { if (hintText) hintText.text = msg; }
    }
}
