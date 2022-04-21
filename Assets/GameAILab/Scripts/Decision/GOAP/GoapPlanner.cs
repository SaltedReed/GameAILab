using System.Collections.Generic;
using UnityEngine;
using GameAILab.Core;

namespace GameAILab.Decision.GOAP
{
    public class State
    {
        public int Count => atoms.Count;
        public Dictionary<string, bool> atoms = new Dictionary<string, bool>();

        public static State operator-(State s1, State s2)
        {
            if (s1 is null)
                return null;
            if (s2 is null)
                return s1.Clone();

            State result = new State();

            foreach (var atom in s1.atoms)
            {
                bool val;
                if (!s2.atoms.TryGetValue(atom.Key, out val) || val != atom.Value)
                {
                    result.atoms.Add(atom.Key, atom.Value);
                }
            }

            return result;
        }

        public static State operator+(State s1, State s2)
        {
            if (s1 is null && s2 is null)
                return null;
            if (s1 != null && s2 is null)
                return s1.Clone();
            if (s1 is null && s2 != null)
                return s2.Clone();

            State result = s1.Clone();
            State diff = s2 - s1;
            foreach (var pair in diff.atoms)
            {
                result.atoms.Add(pair.Key, pair.Value);
            }

            return result;
        }

        public State SimEffect(State effect)
        {
            if (effect is null)
                return Clone();

            State result = Clone();
            State diff = effect - this;
            //Debug.Log($"[SimEffect] diff: {diff.DebugStr()}");

            foreach (var pair in diff.atoms)
            {
                if (result.atoms.ContainsKey(pair.Key))
                {
                    result.atoms[pair.Key] = pair.Value;

                    //Debug.Log($"[SimEffect] set {pair.Key} as {pair.Value}");

                }
                else
                {
                    result.atoms.Add(pair.Key, pair.Value);

                    //Debug.Log($"[SimEffect] added {pair.Key}: {pair.Value}");

                }
            }

            //Debug.Log($"[SimEffect] result: {result.DebugStr()}");

            return result;
        }

        public State Clone()
        {
            State result = new State();

            foreach (var atom in atoms)
            {
                result.atoms.Add(atom.Key, atom.Value);
            }

            return result;
        }

        public void Add(string key, bool val)
        {
            atoms.Add(key, val);
        }

        public void Set(string key, bool val)
        {
            if (atoms.ContainsKey(key))
            {
                atoms[key] = val;
            }
            else
            {
                Add(key, val);
            }
        }

        public bool Has(State other)
        {
            if (other is null || other.atoms.Count > atoms.Count)
                return false;

            foreach (var atom in other.atoms)
            {
                bool val;
                if (atoms.TryGetValue(atom.Key, out val))
                {
                    if (val != atom.Value)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSame(State other)
        {
            return atoms.Count == other.atoms.Count && Has(other);
        }

        public string DebugStr()
        {
            string str = "{ ";
            foreach (var pair in atoms)
            {
                str += $"{pair.Key}: {pair.Value}, ";
            }
            str += " }";
            return str;
        }
    }


    public class Action
    {
        public string name;
        public State pre = new State();
        public State effect = new State();
        public float cost = 1;

        public string DebugStr()
        {
            string str = "";
            string prestr = pre is null ? "null" : pre.DebugStr();
            string efcstr = effect is null ? "null" : effect.DebugStr();
            str += $"name: {name}, pre: {prestr}, effect: {efcstr}, cost: {cost}";
            return str;
        }
    }


    public class GoapNode
    {
        public State state;
        public State goal;

        // for pathfinding
        public List<GoapNode> successors;
        public GoapNode parent;
        public Action actionFromParent;
        public float Cost => actionFromParent.cost;
        public float g;
        public float h;
        public float TotalCost => g + h;
        public bool isClosed = false;

        public float CalculateH()
        {
            return (goal - state).Count;
        }

        public string DebugStr()
        {
            string str = "";
            str += $"state: {state.DebugStr()}\ngoal: {goal.DebugStr()}";
            return str;
        }
    }


    public class GoapGraph
    {
        public int NodeCount => nodes.Count;

        public List<GoapNode> nodes = new List<GoapNode>();

        public GoapNode FindOrAddNode(State state, State goalState)
        {
            GoapNode node = FindNode(state, goalState);
            
            if (node != null)
                return node;

            return AddNode(state, goalState);
        }

        public GoapNode FindNode(State state, State goalState)
        {
            return nodes.Find((GoapNode n) => { return n.state.IsSame(state) && n.goal.IsSame(goalState); });
        }

        public GoapNode AddNode(State state, State goalState)
        {
            GoapNode node = new GoapNode();
            node.state = state;
            node.goal = goalState;
            nodes.Add(node);
            return node;
        }

        public void FindSuccessors(GoapNode node, GoapPlanner planner, out List<GoapNode> nodes, out List<Action> actions)
        {
            nodes = new List<GoapNode>();
            actions = new List<Action>();

            if (node is null)
                return;
            if (planner is null)
                throw new System.ArgumentNullException();

            foreach (Action a in planner.actions)
            {
                State newState = node.state.SimEffect(a.effect);
                State newGoal = node.goal + a.pre;
                GoapNode succ = FindNode(newState, newGoal);
                if (succ != null)
                {
                    nodes.Add(succ);
                    actions.Add(a);
                }                
            }
        }

        public string DebugStr()
        {
            string str = "";
            foreach (GoapNode n in nodes)
            {
                str += n.DebugStr() + "\n";
            }
            return str;
        }
    }


    public class GoapPlanner
    {
        public string name;
        public int ActionCount => actions.Count;
        public List<Action> actions = new List<Action>();

        private GoapAstar m_astar = new GoapAstar();

        public static List<Action> BuildUpPlan(GoapNode pn)
        {
            List<Action> plan = new List<Action>();
            if (pn is null)
                return plan;

            while (pn != null && pn.actionFromParent != null)
            {
                plan.Add(pn.actionFromParent);
                pn = pn.parent;
            }

            return plan;
        }

        public GoapGraph BuildUpGraph(State startState, State goalState)
        {
            GoapGraph graph = new GoapGraph();

            List<State> statesToProcess = new List<State>();
            statesToProcess.Add(startState);
            List<State> goalsToProcess = new List<State>();
            goalsToProcess.Add(goalState);

            while (statesToProcess.Count > 0)
            {
                State curState = statesToProcess[0];
                statesToProcess.RemoveAt(0);
                State curGoal = goalsToProcess[0];
                goalsToProcess.RemoveAt(0);

                if (graph.nodes.Find((GoapNode n) => { return n.state.IsSame(curState) && n.goal.IsSame(curGoal); })
                    != null)
                {
                    continue;
                }

                GoapNode node = graph.AddNode(curState, curGoal);

                if (curState.Has(curGoal))
                {
                    continue;
                }

                State diff = curGoal - curState;
                //Debug.Log("diff: " + diff.DebugStr());

                List<Action> potentialActs = FindActions((Action a) => { return a.effect.Has(diff); });
                foreach (Action a in potentialActs)
                {
                    State newState = curState.SimEffect(a.effect);
                    //Debug.Log($"new state: {newState.DebugStr()}, action: {a.name}");

                    State newGoal = curGoal + a.pre;
                    //Debug.Log($"new goal: {newGoal.DebugStr()}, action: {a.name}");

                    statesToProcess.Add(newState);
                    goalsToProcess.Add(newGoal);
                }
            }

            return graph;
        }

        public GoapNode StartFind(GoapGraph graph, State startState, State goalState)
        {
            return m_astar.StartFind(this, graph, startState, goalState);
        }

        public void AddAction(Action act)
        {
            if (act is null)
                throw new System.ArgumentNullException();

            if (actions.Contains(act))
            {
                Debug.LogError($"[AddAction failed] action {act.name} is already in goap planner {name}");
            }
            else
            {
                actions.Add(act);
            }
        }

        public List<Action> FindActions(System.Predicate<Action> predicate)
        {
            return actions.FindAll(predicate);
        }
    }


    public class GoapAstar
    {
        private GoapPlanner m_planner;
        private GoapGraph m_graph;
        private GoapNode m_start;
        private BinaryHeap<GoapNode> m_open;
        private GoapNode m_current;

        public GoapNode StartFind(GoapPlanner planner, GoapGraph graph, State startState, State goalState)
        {
            m_planner = planner;
            m_graph = graph;
            m_start = graph.FindOrAddNode(startState, goalState);
            m_open = new BinaryHeap<GoapNode>(graph.NodeCount, (GoapNode n1, GoapNode n2) =>
            {
                return (int)(n2.TotalCost * 1000) - (int)(n1.TotalCost * 1000);
            });

            m_start.parent = m_start;
            m_start.g = 0;
            m_start.h = m_start.CalculateH();
            m_open.Add(m_start);

            while (m_open.Count > 0)
            {
                m_current = m_open.RemoveTop();

                if (IsGoal(m_current))
                {
                    return m_current;
                }

                List<GoapNode> successors;
                List<Action> actions;
                graph.FindSuccessors(m_current, planner, out successors, out actions);
                for (int i=0; i<successors.Count; ++i)
                {
                    UpdateNode(successors[i], m_current, actions[i]);
                }
                m_current.isClosed = true;
            }

            return null;
        }

        private bool IsGoal(GoapNode node)
        {
            return node != null && node.state.Has(node.goal);
        }

        private void UpdateNode(GoapNode node, GoapNode parent, Action actionFromParent)
        {
            if (node.isClosed)
                return;

            if (!IsOpen(node))
            {
                node.parent = parent;
                node.actionFromParent = actionFromParent; // must give node action before update g
                UpdateG(node, parent);
                node.h = node.CalculateH();

                m_open.Add(node);
            }
            else
            {
                float newTotalCost = parent.g + node.Cost;
                if (newTotalCost < node.TotalCost)
                {
                    node.parent = parent;
                    node.actionFromParent = actionFromParent; // must give node action before update g
                    UpdateGRecursive(node, parent);
                }
            }
        }

        private bool IsOpen(GoapNode node)
        {
            return node.parent != null;
        }

        private void UpdateG(GoapNode node, GoapNode parent)
        {
            node.g = parent.g + node.Cost;
        }

        private void UpdateGRecursive(GoapNode node, GoapNode parent)
        {
            UpdateG(node, parent);

            List<GoapNode> successors;
            List<Action> actions;
            m_graph.FindSuccessors(node, m_planner, out successors, out actions);
            foreach (GoapNode succ in successors)
            {
                if (succ.parent == node)
                {
                    UpdateGRecursive(succ, node);
                }
            }
        }
    }

}
