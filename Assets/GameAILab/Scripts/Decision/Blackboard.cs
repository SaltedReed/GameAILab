using System.Collections.Generic;

namespace GameAILab.Decision
{

    public class Blackboard
    {
        protected Dictionary<string, int> m_ints;
        protected Dictionary<string, float> m_floats;
        protected Dictionary<string, bool> m_bools;
        protected Dictionary<string, string> m_strings;
        protected Dictionary<string, object> m_objects;

        #region int

        public void SetInt(string key, int value)
        {
            if (m_ints is null)
            {
                m_ints = new Dictionary<string, int>();
            }

            if (m_ints.ContainsKey(key))
            {
                m_ints[key] = value;
            }
            else
            {
                m_ints.Add(key, value);
            }
        }

        public void RemoveInt(string key)
        {
            if (m_ints != null)
            {
                m_ints.Remove(key);
            }
        }

        public bool TryGetInt(string key, out int value)
        {
            if (m_ints is null || !m_ints.ContainsKey(key))
            {
                value = 0;
                return false;
            }

            value = m_ints[key];
            return true;
        }

        #endregion


        #region float

        public void SetFloat(string key, float value)
        {
            if (m_floats is null)
            {
                m_floats = new Dictionary<string, float>();
            }

            if (m_floats.ContainsKey(key))
            {
                m_floats[key] = value;
            }
            else
            {
                m_floats.Add(key, value);
            }
        }

        public void RemoveFloat(string key)
        {
            if (m_floats != null)
            {
                m_floats.Remove(key);
            }
        }

        public bool TryGetFloat(string key, out float value)
        {
            if (m_floats is null || !m_floats.ContainsKey(key))
            {
                value = 0;
                return false;
            }

            value = m_floats[key];
            return true;
        }

        #endregion


        #region bool

        public void SetBool(string key, bool value)
        {
            if (m_bools is null)
            {
                m_bools = new Dictionary<string, bool>();
            }

            if (m_bools.ContainsKey(key))
            {
                m_bools[key] = value;
            }
            else
            {
                m_bools.Add(key, value);
            }
        }

        public void RemoveBool(string key)
        {
            if (m_bools != null)
            {
                m_bools.Remove(key);
            }
        }

        public bool TryGetBool(string key, out bool value)
        {
            if (m_bools is null || !m_bools.ContainsKey(key))
            {
                value = false;
                return false;
            }

            value = m_bools[key];
            return true;
        }

        #endregion


        #region string

        public void SetString(string key, string value)
        {
            if (m_strings is null)
            {
                m_strings = new Dictionary<string, string>();
            }

            if (m_strings.ContainsKey(key))
            {
                m_strings[key] = value;
            }
            else
            {
                m_strings.Add(key, value);
            }
        }

        public void RemoveString(string key)
        {
            if (m_strings != null)
            {
                m_strings.Remove(key);
            }
        }

        public bool TryGetString(string key, out string value)
        {
            if (m_strings is null || !m_strings.ContainsKey(key))
            {
                value = "";
                return false;
            }

            value = m_strings[key];
            return true;
        }

        #endregion


        #region object

        public void SetObject(string key, object value)
        {
            if (m_objects is null)
            {
                m_objects = new Dictionary<string, object>();
            }

            if (m_objects.ContainsKey(key))
            {
                m_objects[key] = value;
            }
            else
            {
                m_objects.Add(key, value);
            }
        }

        public void RemoveObject(string key)
        {
            if (m_objects != null)
            {
                m_objects.Remove(key);
            }
        }

        public bool TryGetObject(string key, out object value)
        {
            if (m_objects is null || !m_objects.ContainsKey(key))
            {
                value = "";
                return false;
            }

            value = m_objects[key];
            return true;
        }

        #endregion

    }


}