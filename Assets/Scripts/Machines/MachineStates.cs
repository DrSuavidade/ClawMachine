#nullable enable
namespace Project.Machines
{
    public enum ClawState
    {
        Idle,
        Positioning,
        Dropping,
        Clamping,
        Lifting,
        Resolving
    }
}
