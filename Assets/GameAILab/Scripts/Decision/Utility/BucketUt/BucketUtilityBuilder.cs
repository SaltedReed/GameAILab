using System;

namespace GameAILab.Decision.BucketUt
{

    public class BucketUtilityBuilder
    {
        private BucketUtilityAI m_utility;
        private Bucket m_curBucket;

        public BucketUtilityBuilder Utility(string name)
        {
            m_utility = new BucketUtilityAI();
            m_utility.Name = name;
            m_curBucket = null;

            return this;
        }

        public BucketUtilityAI EndUtility()
        {
            return m_utility;
        }

        public BucketUtilityBuilder Bucket(string name, Func<Bucket, float> doCalculation)
        {
            if (m_curBucket != null)
            {
                throw new InvalidOperationException();
            }

            Bucket bucket = new Bucket(name);
            if (doCalculation != null)
            {
                bucket.doCalculation = doCalculation;
            }          
            m_utility.AddBucket(bucket);

            m_curBucket = bucket;

            return this;
        }

        public BucketUtilityBuilder EndBucket()
        {
            m_curBucket = null;
            return this;
        }

        public BucketUtilityBuilder Action(Action action)
        {
            if (m_curBucket is null)
            {
                throw new InvalidOperationException();
            }
            if (action is null)
            {
                throw new NullReferenceException();
            }

            m_utility.AddAction(m_curBucket, action);
            return this;
        }
    }

}