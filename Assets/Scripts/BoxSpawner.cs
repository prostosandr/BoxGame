using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BoxSpawner : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private Box _prefab;
    [SerializeField] private int _capacity;
    [SerializeField] private int _maxSize;

    [Header("Spawn Animation Settings")]
    [SerializeField] private float _dropHeight = 5f;
    [SerializeField] private float _dropDuration = 0.5f;
    [SerializeField] private float _delayBetweenSpawns = 0.15f;

    [SerializeField] private Transform[] _spawnPoints;

    private ObjectPool<Box> _pool;
    private List<Box> _activeBox;

    private Box _currentBox;
    private int _numberOfSmashedBoxes;
    private bool _isSpawning;

    public Box CurrentBox => _currentBox;
    public int NumberOfSpawnPoints => _spawnPoints.Length;
    public List<Box> ActiveBox => _activeBox;

    public event Action AllBoxSmashed;

    private void Awake()
    {
        _numberOfSmashedBoxes = 0;

        _pool = new ObjectPool<Box>(
           createFunc: () => CreateItem(),
           actionOnGet: (item) => ActionOnGet(item),
           actionOnRelease: (item) => item.gameObject.SetActive(false),
           actionOnDestroy: (item) => Destroy(item.gameObject),
           collectionCheck: true,
           defaultCapacity: _capacity,
           maxSize: _maxSize);

        _activeBox = ListPool<Box>.Get();
    }

    public void Spawn()
    {
        _numberOfSmashedBoxes = 0;
        _isSpawning = true;

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            Transform spawnPoint = _spawnPoints[i];
            int index = i;

            var spawnedBox = _pool.Get();

            Vector3 targetPosition = spawnPoint.position;
            Vector3 startPosition = targetPosition + Vector3.up * _dropHeight;

            spawnedBox.transform.position = startPosition;

            Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            spawnedBox.transform.rotation = rotation;

            spawnedBox.gameObject.SetActive(true);
            spawnedBox.Initialize();

            bool hasHitGround = false;

            spawnedBox.transform.DOMove(targetPosition, _dropDuration)
                .SetDelay(index * _delayBetweenSpawns)
                .SetEase(Ease.OutBounce)
                .OnUpdate(() =>
                {
                    if (!hasHitGround && spawnedBox.transform.position.y <= targetPosition.y + 0.05f)
                    {
                        hasHitGround = true;
                        spawnedBox.Land();
                    }
                })
                .OnComplete(() =>
                {
                    if (index == _spawnPoints.Length - 1)
                    {
                        _isSpawning = false;
                    }
                });
        }
    }

    public void Clear()
    {
        for(int i = _activeBox.Count - 1; i >= 0; i--)
        {
            _activeBox[i].Deactivate();
        }

        _activeBox.Clear();
    }

    private Box CreateItem()
    {
        var item = Instantiate(_prefab, _container);

        return item;
    }

    private void ActionOnGet(Box box)
    {
        _currentBox = box;
        box.Deactivated += ReleaseItem;
        box.Smashed += CountSmashedBox;

        _activeBox.Add(box);
    }

    private void ReleaseItem(Box box)
    {
        box.Deactivated -= ReleaseItem;
        box.Smashed -= CountSmashedBox;

        _activeBox.Remove(box);
        _pool.Release(box);
    }

    private void CountSmashedBox()
    {
        _numberOfSmashedBoxes++;

        if (_numberOfSmashedBoxes == _activeBox.Count)
            AllBoxSmashed?.Invoke();
    }
}