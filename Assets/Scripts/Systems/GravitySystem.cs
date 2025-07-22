using Components;
using Data;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Linq;
using UnityEngine;

namespace Systems
{
    public class GravitySystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<GridData> _gridData = default;

        private readonly EcsFilterInject<Inc<PositionComponent, GravityDirectionComponent>> _filter = default;

        private readonly Vector2Int[] _fallDirections =
        {
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(1, -1)
        };

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

                    foreach (var direction in _fallDirections)
                    {
                        var targetPosition = pos.Value + direction;

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

                            break;
                        }
                    }
                }
            }
        }
    }
}
