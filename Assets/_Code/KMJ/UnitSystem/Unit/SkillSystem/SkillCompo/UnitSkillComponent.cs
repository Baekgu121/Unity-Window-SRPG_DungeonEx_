using System;
using Code.Core.Events.Bus;
using Input;
using UnityEngine;

namespace Code.SkillSystem
{
    public class UnitSkillComponent : SkillComponent
    {
        [SerializeField] private InputReader intputReader;
        [SerializeField] private SkillManageComponent skillManageCompo;
        
        private void Start()
        {
            intputReader.OnCancelEvent += CancelAllSkill;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            intputReader.OnCancelEvent -= CancelAllSkill;
        }

        protected override void StartSkill(BaseSkill skill, SkillSO skillSo)
        {
            skill.ConfigureSkillRange(skillSo);
            
            if(skillManageCompo.GetSkillInfo() != null)
                skillManageCompo.GetSkillInfo().SkillFinished(true);
            
            skill.ShowSkillRange();
            Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(false));
        }

        protected override void CancelSkill(BaseSkill skill)
        {
            BasicUnitSkill basicSkill = skill as BasicUnitSkill;

            if (basicSkill != null && skillManageCompo.GetSkillInfo() == basicSkill)
            {
                basicSkill.SkillFinished(true);
            }
            
            Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
            basicSkill.BooleanSkillUse(false);
        }
    }
}