using Leopotam.EcsLite.UnityEditor;
using Leopotam.EcsLite;
using UnityEngine;
using Data;
using Systems;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;

namespace Global
{
    public class GameStartup : MonoBehaviour
    {
        [SerializeField] private LevelConfiguration _levelConfig;
        [SerializeField] private List<MatchPattern> _matchPatterns;
        [SerializeField] private GameConfig _gameConfig;

        private EcsWorld _world;
        private IEcsSystems _systems;

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);

            var gridData = new GridData(_levelConfig.Width, _levelConfig.Height);

            _systems
                .Add(new LevelInitSystem(_levelConfig))

                .Add(new InputSystem())
                .Add(new SwapSystem())
                .Add(new MatchDetectionSystem(_matchPatterns))
                .Add(new ClearMatchedSystem())
                .Add(new BonusSpawnSystem())

                .Add(new GravitySystem())
                .Add(new PieceMoveSystem())

                .Add(new ViewMoveSystem())
                .Add(new DestroySystem())

                .Inject(_gameConfig)
                .Inject(gridData)

                .Add(new EcsWorldDebugSystem())
                .Add(new EcsSystemsDebugSystem());

            _systems.Init();
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void OnDestroy()
        {
            _systems?.Destroy();
            _systems = null;

            _world?.Destroy();
            _world = null;
        }
    }
}
