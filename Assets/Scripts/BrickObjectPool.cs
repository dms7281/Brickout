using UnityEngine;
using UnityEngine.Pool;

public class BrickObjectPool : MonoBehaviour
{
    public ObjectPool<Brick> brickObjectPool;
    private BrickPattern _brickPattern;
    private Transform _brickPoolTrans; // Transform of parent that will hold all inactive pool objects

    private int _maxPoolSize;
    private void Awake()
    {
        _brickPattern = GetComponent<BrickPattern>();
        _brickPoolTrans = transform.GetChild(0);
        _brickPoolTrans.SetAsLastSibling();
        _maxPoolSize = 250;

        // Creating a new pool with a size of 250, since that is the max size of a pattern
        brickObjectPool = new ObjectPool<Brick>(CreateBrick, OnTakeBrickFromPool, OnReturnBrickToPool, OnDestoryBrick, true, _maxPoolSize, _maxPoolSize);

        for(int i = 0; i < _maxPoolSize; i++)
        {
            Brick brick = CreateBrick();
            brickObjectPool.Release(brick);
        }
    }

    private Brick CreateBrick()
    {
        Brick brick = Instantiate(_brickPattern._brickPrefab);
        brick.SetPool(brickObjectPool);
        return brick;
    }

    private void OnTakeBrickFromPool(Brick brick)
    {
        brick.gameObject.transform.SetParent(transform);

        brick.gameObject.SetActive(true);
    }

    private void OnReturnBrickToPool(Brick brick)
    {
        brick.gameObject.transform.SetParent(_brickPoolTrans);
        brick.gameObject.transform.position = Vector3.zero;
        brick.gameObject.SetActive(false);
    }

    private void OnDestoryBrick(Brick brick)
    {
        Destroy(brick.gameObject);
    }
}