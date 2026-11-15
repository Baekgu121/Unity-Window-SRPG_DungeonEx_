using UnityEngine;

namespace Code.SkillSystem
{
    public enum SkillType
    {
        BasicAttack,
        ActiveSkill,
    }
    
    [CreateAssetMenu(fileName = "SkillSO", menuName = "SO/UnitSO/Skill/UnitSkill", order = 0)]
    public class SkillSO : ScriptableObject
    {
        [Header("Basic Info")]
        public UnitType UnitType = UnitType.None;
        public string SkillName;
        public string SkillAnimationKey;
        
        [TextArea(3, 15)]
        public string SkillDescription;
        public int SkillValue;
        public int SkillCost;
        public Sprite skillUIImage;
        public string ClassName;
        public int SkillPrice;

        [Header("Detail Info")]
        [Range(0,11)] public float SkillDamage;
        public int SkillRange;
        public int MinRange;
        public bool IsOwnSkill;
        public SkillType SkillType;
    }
}