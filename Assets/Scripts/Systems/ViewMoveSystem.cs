using Components;
using Data;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class ViewMoveSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ViewComponent, PositionComponent>> _viewFilter = default;
        private readonly EcsCustomInject<GameConfig> _gameConfig = default;

        public void Run(IEcsSystems systems)
        {
            var viewPool = _viewFilter.Pools.Inc1;
            var posPool = _viewFilter.Pools.Inc2;
            var config = _gameConfig.Value;

            foreach (int entity in _viewFilter.Value)
            {
                ref var view = ref viewPool.Get(entity);
                ref var pos = ref posPool.Get(entity);

                var targetPosition = new Vector3(pos.Value.x * config.CellSize, pos.Value.y * config.CellSize, 0);

                view.Transform.position = Vector3.Lerp(view.Transform.position, targetPosition, Time.deltaTime * 15f);
            }
        }
    }
}
