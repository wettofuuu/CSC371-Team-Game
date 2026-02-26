// IPushableBlock.cs
using UnityEngine;

public interface IPushableBlock
{
    void TryPushFromPlayer(Collider playerCollider);
}