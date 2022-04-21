using System;

namespace GameAILab.Decision.DualUt
{
    public class DualUtilityBuilder
    {
        private DualUtilityAI m_utility;

        public DualUtilityBuilder Utility(string name)
        {
            m_utility = new DualUtilityAI();
            m_utility.Name = name;

            return this;
        }

        public DualUtilityAI EndUtility()
        {
            return m_utility;
        }

        public DualUtilityBuilder UtObject(UtObject utObject)
        {
            if (utObject is null)
            {
                throw new NullReferenceException();
            }

            m_utility.AddUtObject(utObject);
            return this;
        }
    }
}


#if false
namespace GameAILab.Decision.DualUt
{

    public class DualUtilityBuilder
    {
        private DualUtilityAI m_utility;
        
        public DualUtilityBuilder Utility(string name)
        {
            m_utility = new DualUtilityAI();
            m_utility.Name = name;

            return this;
        }

        public DualUtilityAI EndUtility()
        {
            return m_utility;
        }

        public DualUtilityBuilder Action(Action action)
        {
            if (action is null)
            {
                throw new NullReferenceException();
            }

            m_utility.AddAction(action);
            return this;
        }
    }

}
#endif