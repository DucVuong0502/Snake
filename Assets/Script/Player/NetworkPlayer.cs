using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public int PlayerId { get; set; }
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            PlayerId = Object.InputAuthority.PlayerId;
        }

        Debug.Log($"[NetworkPlayer] Spawned: PlayerId={PlayerId}");
    }
}