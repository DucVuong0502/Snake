using Fusion;
using UnityEngine;

public class PointFromPool : NetworkBehaviour
{
    [HideInInspector] public NetworkPool pool;

    private void OnTriggerEnter(Collider other)
    {
        var snake = other.GetComponent<SnakeHead3D>();
        if (snake != null)
        {
            snake.RPC_RequestEatFood(snake.Object.Id);

            if (HasStateAuthority && pool != null)
            {
                pool.Return(Object); // Trả về pool
            }
        }
    }
}
