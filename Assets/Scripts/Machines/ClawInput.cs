#nullable enable
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Project.Machines
{
    /// <summary>Input wrapper for claw controls (keyboard/gamepad now, touch later).</summary>
    public sealed class ClawInput : MonoBehaviour
    {
        public UnityEvent<Vector2> OnMove = new();
        public UnityEvent OnDrop = new();
        public UnityEvent OnClamp = new();
        public UnityEvent OnCancel = new();

        public void Input_Move(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed && !ctx.canceled) return;
            OnMove.Invoke(ctx.ReadValue<Vector2>());
        }

        public void Input_Drop(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnDrop.Invoke();
        }

        public void Input_Clamp(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnClamp.Invoke();
        }

        public void Input_Cancel(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) OnCancel.Invoke();
        }
    }
}
