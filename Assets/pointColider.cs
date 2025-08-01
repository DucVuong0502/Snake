using Fusion;
using UnityEngine;

public class pointColider : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SnakeHead3D snake = other.GetComponent<SnakeHead3D>();
        if (snake != null)
        {
            // Yêu cầu ăn food → Gửi từ Client → lên StateAuthority
            snake.RPC_RequestEatFood(snake.Object.Id);

            if (Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }
    }
}
