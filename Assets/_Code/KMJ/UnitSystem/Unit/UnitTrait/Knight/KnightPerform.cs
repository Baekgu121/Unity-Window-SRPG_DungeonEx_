using Code.Core;
using Code.Core.Events.Bus;
using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.UnitSystem.TraitSystem
{
    public class KnightPerform : MonoBehaviour , IUnitPerform
    {
        [SerializeField] private KnightCondition condition;
        [SerializeField] private ParticleSystem circleParticle;
        [SerializeField] private KnightDefenseRange defenseCompo;
        
        private Unit _unit;
        private CharacterUnit _characterUnit;
        
        private UnitHealth _healthCompo;
        
        public void Initialize(Unit unit)
        {
            _unit = unit;
            _healthCompo = _unit.GetUnitCompo<UnitHealth>();

            _characterUnit = _unit as CharacterUnit;
        }
        private void SetInvicibility(CharacterUnit unit, int maxTurn)
        {
            unit.GetUnitCompo<InvincibilityCompo>().SetUnitInvincibility(maxTurn);
        }

        public void Perform(GameObject target)
        {
            condition.ResetStack();
            Bus<KnightSpecEvent>.Raise(new KnightSpecEvent(0));
            
            foreach (var unit in defenseCompo.Targets)
            {
                SetInvicibility(unit,2);
                unit.GetUnitCompo<UnitHealth>().HealHp(20);
            }
            
            SetInvicibility(_characterUnit, 4);

            _healthCompo.ResetMaxHp();
            SoundManager.Instance.PlayClip("GlowSound");
                 
            circleParticle.Play();
        }

    }
}