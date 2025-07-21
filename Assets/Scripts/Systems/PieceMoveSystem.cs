using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Systems
{
    public class PieceMoveSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<MoveToComponent, PositionComponent>, Exc<BlockerComponent, EmptyMarkerComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            var movePool = _filter.Pools.Inc1;
            var positionPool = _filter.Pools.Inc2;

            foreach (int entity in _filter.Value)
            {
                ref var move = ref movePool.Get(entity);
                ref var pos = ref positionPool.Get(entity);

                pos.Value = move.TargetPosition;

                movePool.Del(entity);
            }
        }
    }
}
