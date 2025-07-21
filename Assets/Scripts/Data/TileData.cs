using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class TileData
    {
        public Vector2Int Position;
        public TileType Type = TileType.Available;
        [Tooltip("Which Piece to create. Empty to random")]
        public PieceTypeSO InitialPiece;
        public Vector2Int GravityDirection = new Vector2Int(0, -1);
        public int Durability = 0;
    }
}
