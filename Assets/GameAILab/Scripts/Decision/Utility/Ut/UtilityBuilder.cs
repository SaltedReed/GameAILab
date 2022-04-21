

namespace GameAILab.Decision.Ut
{

    public class UtilityBuilder
    {
        private UtilityAI m_utility;

        public UtilityBuilder Utility(string name, UtilityAI.Policy choosePolicy)
        {
            m_utility = new UtilityAI();
            m_utility.Name = name;
            m_utility.choosePolicy = choosePolicy;

            return this;
        }

        public UtilityAI EndUtility()
        {
            return m_utility;
        }

        public UtilityBuilder Action(Action action)
        {
            m_utility.AddAction(action);
            return this;
        }
    }

}