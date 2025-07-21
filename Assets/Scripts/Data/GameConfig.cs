using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class GameConfig
    {
        public float CellSize = 1.0f;
        public List<PieceTypeSO> AvailablePieces;

        public PieceTypeSO GetRandomPiece()
        {
            if (AvailablePieces == null || AvailablePieces.Count == 0)
                return null;

            return AvailablePieces[UnityEngine.Random.Range(0, AvailablePieces.Count)];
        }
    }
}
