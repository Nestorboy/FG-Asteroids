using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] private Asteroid _asteroidPrefab;
        [SerializeField] private float _minSpawnTime;
        [SerializeField] private float _maxSpawnTime;
        [SerializeField] private int _minAmount;
        [SerializeField] private int _maxAmount;
        [SerializeField] private SpawnLocation _spawnLocations = SpawnLocation.Top | SpawnLocation.Bottom | SpawnLocation.Left | SpawnLocation.Right;
        
        private float _timer;
        private float _nextSpawnTime;
        private Camera _camera;

        [Flags]
        private enum SpawnLocation
        {
            Top = 1 << 0,
            Bottom = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3,
        }

        private void Start()
        {
            _camera = Camera.main;
            Spawn();
            UpdateNextSpawnTime();
        }

        private void Update()
        {
            UpdateTimer();

            if (!ShouldSpawn())
                return;

            Spawn();
            UpdateNextSpawnTime();
            _timer = 0f;
        }

        private void UpdateNextSpawnTime()
        {
            _nextSpawnTime = Random.Range(_minSpawnTime, _maxSpawnTime);
        }

        private void UpdateTimer()
        {
            _timer += Time.deltaTime;
        }

        private bool ShouldSpawn()
        {
            return _timer >= _nextSpawnTime;
        }

        private void Spawn()
        {
            if (_spawnLocations == 0)
                return;
            
            int amount = Random.Range(_minAmount, _maxAmount + 1);
            
            for (int i = 0; i < amount; i++)
            {
                SpawnLocation location = GetSpawnLocation();
                Vector3 position = GetStartPosition(location);
                Instantiate(_asteroidPrefab, position, Quaternion.identity);
            }
        }

        private SpawnLocation GetSpawnLocation()
        {
            int validLocationCount = 0;
            for (int i = 0; i < 4; i++)
            {
                SpawnLocation location = (SpawnLocation)(1 << i);
                if (!_spawnLocations.HasFlag(location))
                    continue;
                
                validLocationCount++;
            }
            
            int roll = Random.Range(0, validLocationCount);

            int validLocationIndex = 0;
            for (int i = 0; i < 4; i++)
            {
                SpawnLocation location = (SpawnLocation)(1 << i);
                if (!_spawnLocations.HasFlag(location))
                    continue;
                
                if (validLocationIndex++ != roll)
                    continue;
                
                return location;
            }

            return 0;
        }

        private Vector3 GetStartPosition(SpawnLocation spawnLocation)
        {
            Vector3 pos = new Vector3 { z = Mathf.Abs(_camera.transform.position.z) };
            
            const float padding = 5f;
            switch (spawnLocation)
            {
                case SpawnLocation.Top:
                    pos.x = Random.Range(0f, Screen.width);
                    pos.y = Screen.height + padding;
                    break;
                case SpawnLocation.Bottom:
                    pos.x = Random.Range(0f, Screen.width);
                    pos.y = 0f - padding;
                    break;
                case SpawnLocation.Left:
                    pos.x = 0f - padding;
                    pos.y = Random.Range(0f, Screen.height);
                    break;
                case SpawnLocation.Right:
                    pos.x = Screen.width - padding;
                    pos.y = Random.Range(0f, Screen.height);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(spawnLocation), spawnLocation, null);
            }
            
            return _camera.ScreenToWorldPoint(pos);
        }
    }
}
