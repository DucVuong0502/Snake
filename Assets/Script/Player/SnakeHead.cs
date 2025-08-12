using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;

public class SnakeHead3D : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float turnSpeed = 120f;
    public float maxTurnAngle = 90f;

    [Header("Body")]
    public GameObject bodyPrefab;
    public float bodySpacing;
    public bool canMove = false;

    [HideInInspector] public CameraManager camManager; // Nếu dùng camera switch

    private List<Transform> bodyParts = new List<Transform>();

    [Networked, Capacity(512)] public NetworkArray<Vector3> PathBuffer { get; }
    [Networked] public int PathIndex { get; set; }

    // Điểm của player (đồng bộ qua mạng)
    [Networked] public int Score { get; set; }

    private Vector3 moveDirection;

    public override void Spawned()
    {
        moveDirection = transform.forward;

        // Đăng ký player vào leaderboard
        LeaderboardManager.Instance?.RegisterPlayer(this);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Xoá khỏi leaderboard khi thoát
        LeaderboardManager.Instance?.UnregisterPlayer(this);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        // Điều khiển
        if (canMove == false) return;
        if (camManager == null || camManager.steeringModeLocal == 1)
        {
            HandleSteeringInput();
        }
        else if (camManager.steeringModeLocal == 2)
        {
            HandleDirectionalInput();
        }

        transform.position += moveDirection * moveSpeed * Runner.DeltaTime;

        PathIndex = (PathIndex + 1) % PathBuffer.Length;
        PathBuffer.Set(PathIndex, transform.position);
    }

    void HandleSteeringInput()
    {
        float h = 0f;
        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;

        transform.Rotate(Vector3.up, h * turnSpeed * Runner.DeltaTime);
        moveDirection = transform.forward;
    }

    void HandleDirectionalInput()
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) inputDir += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) inputDir += Vector3.back;
        if (Input.GetKey(KeyCode.A)) inputDir += Vector3.left;
        if (Input.GetKey(KeyCode.D)) inputDir += Vector3.right;

        if (inputDir == Vector3.zero) return;

        Vector3 desiredDirXZ = new Vector3(inputDir.x, 0, inputDir.z).normalized;
        Vector3 currentDir = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;

        float angle = Vector3.SignedAngle(currentDir, desiredDirXZ, Vector3.up);

        if (Mathf.Abs(angle) <= maxTurnAngle)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredDirXZ);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Runner.DeltaTime);
        }
        else
        {
            float step = Mathf.Clamp(angle, -turnSpeed * Runner.DeltaTime, turnSpeed * Runner.DeltaTime);
            transform.Rotate(Vector3.up, step);
        }

        moveDirection = transform.forward;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestEatFood(NetworkId who)
    {
        if (Object.HasStateAuthority && Object.Id == who)
        {
            RPC_AddBodyPart(who);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AddBodyPart(NetworkId who)
    {
        if (Object.Id != who) return;

        // Cộng điểm khi ăn
        if (Object.HasStateAuthority)
        {
            Score += 10;
        }

        Vector3 spawnPos = bodyParts.Count == 0
            ? transform.position - transform.forward * bodySpacing
            : bodyParts[^1].position - bodyParts[^1].forward * bodySpacing;

        var newPart = Runner.Spawn(bodyPrefab, spawnPos, Quaternion.identity);

        var bp = newPart.GetComponent<BodyPart>();
        bp.HeadId = Object.Id;
        bp.IndexOffset = (bodyParts.Count + 1) * 10;

        bodyParts.Add(newPart.transform);
    }
}