using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Match Pattern", menuName = "Match-3/Match Pattern")]
    public class MatchPattern : ScriptableObject
    {
        public int Priority;

        public PieceTypeSO BonusToSpawn;

        public int MinHorizontalMatch = 0;
        public int MinVerticalMatch = 0;
    }
}
