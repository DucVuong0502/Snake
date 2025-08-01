using Fusion;
using UnityEngine;

public class PlayerKillOnTouch : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Đảm bảo chỉ client có quyền điều khiển mới gửi yêu cầu
        if (!Object.HasInputAuthority) return;

        var target = other.GetComponent<NetworkObject>();
        if (target != null && target != Object) // không tự chạm chính mình
        {
            // Gửi yêu cầu giết đối tượng kia lên StateAuthority
            RPC_RequestKill(target.Id);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestKill(NetworkId targetId)
    {
        NetworkObject target = Runner.FindObject(targetId);
        if (target != null)
        {
            // Đảm bảo mình là người có quyền quản lý state trước khi xử lý
            if (HasStateAuthority)
            {
                Runner.Despawn(target); // Giết ngay lập tức
            }
        }
    }
}