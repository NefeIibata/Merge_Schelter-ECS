using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PieceType", menuName = "Match-3/Piece Type")]
    public class PieceTypeSO : ScriptableObject
    {
        [Tooltip("Name to Debug")]
        public string TypeName;

        [Tooltip("Prefab to create")]
        public GameObject Prefab;

        [Tooltip("Piece")]
        public bool IsPiece = true;

        [Tooltip("Bobus")]
        public bool IsBonus = false;

        [Tooltip("Blocker")]
        public bool IsBlocker = false;
    }
}
