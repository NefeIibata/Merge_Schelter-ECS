using Components;
using Data;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Systems
{
    public class MatchDetectionSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<GridData> _grid = default;
        private readonly EcsFilterInject<Inc<PieceComponent>, Exc<BlockerComponent>> _pieceFilter = default;

        private readonly List<MatchPattern> _matchPatterns;

        private readonly List<int> _horizontalMatch = new List<int>();
        private readonly List<int> _verticalMatch = new List<int>();
        private readonly HashSet<int> _checkedEntities = new HashSet<int>();

        public MatchDetectionSystem(List<MatchPattern> patterns)
        {
            _matchPatterns = patterns;

            _matchPatterns.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public void Run(IEcsSystems systems)
        {
            var piecePool = _world.Value.GetPool<PieceComponent>();
            var matchPool = _world.Value.GetPool<MatchComponent>();
            _checkedEntities.Clear();

            foreach (int entity in _pieceFilter.Value)
            {
                if (_checkedEntities.Contains(entity) || matchPool.Has(entity))
                    continue;

                var pieceType = piecePool.Get(entity).Type;

                FindMatches(entity, pieceType, piecePool);

                foreach (var pattern in _matchPatterns)
                {
                    if ((pattern.MinHorizontalMatch > 0 && _horizontalMatch.Count >= pattern.MinHorizontalMatch) ||
                        (pattern.MinVerticalMatch > 0 && _verticalMatch.Count >= pattern.MinVerticalMatch))
                    {
                        MarkForMatch(_horizontalMatch, matchPool);
                        MarkForMatch(_verticalMatch, matchPool);

                        if (pattern.BonusToSpawn != null)
                        {
                            ref var request = ref _world.Value.GetPool<SpawnBonusRequestComponent>().Add(entity);
                            request.BonusType = pattern.BonusToSpawn;
                        }

                        break;
                    }
                }
            }
        }

        private void FindMatches(int startEntity, PieceTypeSO pieceType, EcsPool<PieceComponent> piecePool)
        {
            _horizontalMatch.Clear();
            _verticalMatch.Clear();

            var posPool = _world.Value.GetPool<PositionComponent>();
            var startPos = posPool.Get(startEntity).Value;

            _horizontalMatch.Add(startEntity);
            SearchInDirection(startPos, new Vector2Int(1, 0), pieceType, piecePool, _horizontalMatch);
            SearchInDirection(startPos, new Vector2Int(-1, 0), pieceType, piecePool, _horizontalMatch);

            _verticalMatch.Add(startEntity);
            SearchInDirection(startPos, new Vector2Int(0, 1), pieceType, piecePool, _verticalMatch);
            SearchInDirection(startPos, new Vector2Int(0, -1), pieceType, piecePool, _verticalMatch);
        }

        private void SearchInDirection(Vector2Int startPos, Vector2Int dir, PieceTypeSO pieceType, EcsPool<PieceComponent> piecePool, List<int> results)
        {
            var grid = _grid.Value;
            for (int i = 1; i < Mathf.Max(grid.Width, grid.Height); i++)
            {
                var nextPos = startPos + dir * i;
                if (nextPos.x < 0 || nextPos.x >= grid.Width || nextPos.y < 0 || nextPos.y >= grid.Height)
                    break;

                if (grid.Entities[nextPos.x, nextPos.y].Unpack(_world.Value, out int nextEntity) &&
                    piecePool.Has(nextEntity) && 
                    piecePool.Get(nextEntity).Type == pieceType)
                {
                    results.Add(nextEntity);
                }
                else
                {
                    break;
                }
            }
        }

        private void MarkForMatch(List<int> matchedEntities, EcsPool<MatchComponent> matchPool)
        {
            if (matchedEntities.Count < 3) return;

            foreach (var entity in matchedEntities)
            {
                if (!matchPool.Has(entity))
                {
                    matchPool.Add(entity);
                }
                _checkedEntities.Add(entity);
            }
        }
    }
}
