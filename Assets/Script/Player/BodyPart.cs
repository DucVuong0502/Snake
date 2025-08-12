using Fusion;
using UnityEngine;

public class BodyPart : NetworkBehaviour
{
    [Networked] public NetworkId HeadId { get; set; }
    [Networked] public int IndexOffset { get; set; }

    private SnakeHead3D cachedHead;
    private bool initialized;

    public override void FixedUpdateNetwork()
    {
        if (cachedHead == null && HeadId.Raw != 0)
        {
            var obj = Runner.FindObject(HeadId);
            if (obj != null) cachedHead = obj.GetComponent<SnakeHead3D>();
        }

        if (cachedHead == null || cachedHead.Object == null || !cachedHead.Object.IsValid)
            return;

        if (!initialized)
        {
            if (cachedHead.PathBuffer.Length > 0)
                initialized = true;
            return;
        }

        int readIndex = cachedHead.PathIndex - IndexOffset;
        while (readIndex < 0) readIndex += cachedHead.PathBuffer.Length;

        Vector3 target = cachedHead.PathBuffer.Get(readIndex);

        transform.position = Vector3.Lerp(transform.position, target, 10f * Runner.DeltaTime);

        Vector3 dir = (target - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.forward = Vector3.Lerp(transform.forward, dir, 10f * Runner.DeltaTime);
    }
}
