using System;
using System.Collections.Generic;
using System.Linq;
using Code.KMJ.UnitSystem.involveUnitSO;
using Code.Core.Debugs;
using Code.Core.Events.Bus;
using Code.Core.Managers;
using Code.UnitSystem;
using UnityEngine;

namespace Code.SkillSystem
{
    public abstract class SkillComponent : MonoBehaviour, IUnitComponent
    {
        [SerializeField] protected List<SkillSO> skillList = new List<SkillSO>();
        [SerializeField] private UnitSkillStorageSO skillStorage;
        public Dictionary<SkillSO, BaseSkill> Skills { get; private set; }

        protected UnitStatCompo statCompo;
        protected Unit unit;

        protected int basicDamage = 0;
        protected bool isUseSkill = true;

        public void Initialize(Unit owner)
        {
            unit = owner;
            
            if(skillStorage != null)
            {
                skillList = skillStorage.skills;
            }
            
            if (unit != null && statCompo == null)
                statCompo = unit.GetUnitCompo<UnitStatCompo>();

            foreach (var skill in SkillSendManager.Instance.GetEquipSkills(unit.unitSO.UnitType))
            {
                if(!skillList.Contains(skill))
                  skillList.Add(skill);
            }
            
            Skills = new Dictionary<SkillSO, BaseSkill>();
            

            foreach (var skillData in skillList)
            {
                if (skillData == null || string.IsNullOrEmpty(skillData.ClassName))
                    continue;

                Type type = GetTypeByName(skillData.ClassName);

                if (type == null)
                {
                    Debug.LogError($"[SkillComponent] '{unit.name}'의 스킬 '{skillData.SkillName}'에 해당하는 클래스 '{skillData.ClassName}'를 찾을 수 없습니다.");
                    continue;
                }

                var component = unit.GetComponentInChildren(type, true);

                if (component is BaseSkill baseSkill)
                {
                    if (component == null)
                        continue;
                    
                    Skills.TryAdd(skillData, baseSkill);
                }
                else
                    Debug.LogWarning($"[SkillComponent] '{unit.name}'에 스킬 컴포넌트 '{type.Name}'가 부착되어 있지 않습니다.");
            }

            SkillSetDamage();

            Bus<UsingSkillEvent>.Subscribe(BooleanSkill);
        }

        private void SkillSetDamage()
        {
            if (Skills.Count > 0)
            {
                foreach (var skill in Skills.Values)
                {
                    if (statCompo != null)
                    {
                        int skillDamageValue = statCompo.GetStat(StatInfo.AtkDamage);
                        int finallyDamage = (int)(skillDamageValue * skill.BasicSkillDamage);
                        basicDamage = finallyDamage;
                    }
                    else
                        basicDamage = statCompo.GetStat(StatInfo.AtkDamage);

                    skill.InitializeSkill();
                    skill.SetDamage(basicDamage);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            Bus<UsingSkillEvent>.Unsubscribe(BooleanSkill);
        }
        
        private Type GetTypeByName(string className)
        {
            Type type = Type.GetType(className);

            if (type != null)
                return type;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetTypes().FirstOrDefault(t =>
                    t.Name == className || t.FullName == className || t.FullName.EndsWith($".{className}"));

                if (type != null)
                    return type;
            }

            return null;
        }
        
        private void BooleanSkill(UsingSkillEvent evt)
        {
            isUseSkill = evt.isUsingSkill;
        }
        public void ResetSkillsCount()
        {
            foreach (var skill in Skills.Values)
            {
                skill.ResetSkillCnt();
            }
        }
        
        public void UpdateSkillUI()
        {
            Bus<SkillUIEvent>.Raise(new SkillUIEvent(skillList, this));
        }

        public void SetAddSkillDamage(float addDamage,SkillType skillType)
        {
            foreach (var skill in Skills.Values)
            {
                if (skill.SkillSO.SkillType == skillType)
                {
                    int damage = basicDamage + (int)addDamage;
                    skill.SetDamage(damage);
                }
            }
        }
        
        public void StartSkill(SkillSO skillSo)
        {
            if (!isUseSkill || !unit.isMyTurn)
                return;
            
            if (!Skills.ContainsKey(skillSo))
                return;

            BaseSkill skill = Skills.GetValueOrDefault(skillSo);

            if (skill != null)
            {
                StartSkill(skill,skillSo);
            }
        }

        public void CancelAllSkill()
        {
            if (!unit.isMyTurn)
                return;
            
            foreach (var skill in Skills.Values)
            {
                CancelSkill(skill);
            }   
        }

        protected virtual void StartSkill(BaseSkill skill, SkillSO skillSo)
        {
            
        }

        protected virtual void CancelSkill(BaseSkill skill)
        {
            
        }
    }
}
