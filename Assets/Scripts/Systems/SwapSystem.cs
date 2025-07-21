using Components;
using Data;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class SwapSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<GridData> _grid = default;
        private readonly EcsFilterInject<Inc<SwapRequestComponent>, Exc<BlockerComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            var requestPool = _filter.Pools.Inc1;
            var blockerPool = _world.Value.GetPool<BlockerComponent>();
            var posPool = _world.Value.GetPool<PositionComponent>();

            foreach (int entity1 in _filter.Value)
            {
                ref var request = ref requestPool.Get(entity1);

                if (!request.Target.Unpack(_world.Value, out int entity2))
                {
                    requestPool.Del(entity1);
                    continue;
                }

                if (blockerPool.Has(entity2))
                {
                    requestPool.Del(entity1);
                    continue;
                }

                ref var pos1 = ref posPool.Get(entity1);
                ref var pos2 = ref posPool.Get(entity2);

                if (!IsNeighborPosition(ref pos1, ref pos2))
                {
                    requestPool.Del(entity1);
                    continue;
                }

                SwapEntities(entity1, entity2, ref pos1, ref pos2);

                requestPool.Del(entity1);
            }
        }

        private void SwapEntities(int entity1, int entity2, ref PositionComponent pos1, ref PositionComponent pos2)
        {
            var packed1 = _world.Value.PackEntity(entity1);
            var packed2 = _world.Value.PackEntity(entity2);
            _grid.Value.Entities[pos1.Value.x, pos1.Value.y] = packed2;
            _grid.Value.Entities[pos2.Value.x, pos2.Value.y] = packed1;

            var tempPos = pos1.Value;
            pos1.Value = pos2.Value;
            pos2.Value = tempPos;
        }

        private bool IsNeighborPosition(ref PositionComponent pos1, ref PositionComponent pos2)
        {
            int dx = Mathf.Abs(pos1.Value.x - pos2.Value.x);
            int dy = Mathf.Abs(pos1.Value.y - pos2.Value.y);

            bool areNeighbours = (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
            return areNeighbours;
        }
    }
}
