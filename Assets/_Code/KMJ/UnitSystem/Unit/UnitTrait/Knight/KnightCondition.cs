using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.TraitSystem
{
    public class KnightCondition : MonoBehaviour, IUnitCondition
    {
        [SerializeField] private int maxStack = 4;
        
        private int _stack = 0;

        public void Initialize(Unit unit)
        {
        }
        
        private void SetStack()
        {
            _stack += 1;
        }

        public void ResetStack()
        {
            _stack = 0;
        }

        public bool CheckCondition(GameObject unit)
        {
            SetStack();
            
            Bus<KnightSpecEvent>.Raise(new KnightSpecEvent(_stack));
            
            if (_stack >= maxStack)
            {
                return true;
            }
            
            return false;
        }
    }
}