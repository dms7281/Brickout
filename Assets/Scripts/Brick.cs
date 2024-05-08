using UnityEngine;
using UnityEngine.Pool;

public class Brick : MonoBehaviour
{
    private ObjectPool<Brick> _brickPool;
    // A brick contains a non-kinematic rigidbody and collider. When it collides with the ball, the game object is destroyed
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Ball"))
        {
            //Destroy(transform.gameObject);
            _brickPool.Release(this);
        }
    }

    public void SetPool(ObjectPool<Brick> brickPool)
    {
        _brickPool = brickPool;
    } 
}
