using Components;
using Data;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;

namespace Systems
{
    public class GravitySystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<GridData> _gridData = default;

        private readonly EcsFilterInject<Inc<PositionComponent, GravityDirectionComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            var grid = _gridData.Value;
            var movePool = _world.Value.GetPool<MoveToComponent>();

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (!grid.Entities[x, y].Unpack(_world.Value, out int entity))
                        continue;
                    
                    if (!_filter.Value.GetRawEntities().Contains(entity))
                        continue;

                    if (movePool.Has(entity))
                        continue;


                    ref var pos = ref _world.Value.GetPool<PositionComponent>().Get(entity);
                    ref var gravity = ref _world.Value.GetPool<GravityDirectionComponent>().Get(entity);

                    var targetPosition = pos.Value + gravity.Direction;

                    if (targetPosition.x < 0 || targetPosition.x >= grid.Width ||
                        targetPosition.y < 0 || targetPosition.y >= grid.Height)
                    {
                        continue;
                    }

                    if (!grid.Entities[targetPosition.x, targetPosition.y].Unpack(_world.Value, out _))
                    {
                        ref var moveRequest = ref movePool.Add(entity);
                        moveRequest.TargetPosition = targetPosition;

                        grid.Entities[targetPosition.x, targetPosition.y] = grid.Entities[pos.Value.x, pos.Value.y];

                        grid.Entities[pos.Value.x, pos.Value.y] = default;
                    }
                }
            }
        }
    }
}
