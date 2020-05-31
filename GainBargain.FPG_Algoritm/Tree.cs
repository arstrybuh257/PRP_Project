using System.Collections.Generic;
using System.Linq;

namespace GainBargain.FPG_Algoritm
{
    public class Node<T> where T : struct
    {
        public T Id { get; set; }
        public int Quantity { get; set; }
        public List<Node<T>> Nodes { get; set; }

        public Node(T id)
        {
            Id = id;
            Quantity = 1;
            Nodes = new List<Node<T>>();
        }

        public Node() //for Root
        {
            Nodes = new List<Node<T>>();
        }
    }


    public class Tree<T> where T : struct
    {
        public Node<T> Root { get; set; }
        public int MinSupport { get; set; }

        public Tree(List<List<T>> transactions, int minSupport)
        {
            Root = new Node<T>();
            MinSupport = minSupport;
            transactions = SortTransactions(transactions);
            BuildTree(transactions);
        }

        private Tree(T id)
        {
            Root = new Node<T>();
        }

        public Tree<T> GetConditionalTree(T id)
        {
            return BuildConditionalTree(id);
        }


        private void BuildTree(List<List<T>> Transactions)
        {
            foreach (var t in Transactions)
            {
                if (t.Count() > 0)
                {
                    var node = Root.Nodes.Where(n => n.Id.Equals(t[0])).FirstOrDefault();
                    if (node != null)
                    {
                        node.Quantity += 1;
                    }
                    else
                    {
                        node = new Node<T>(t[0]);
                        Root.Nodes.Add(node);
                    }
                    for (int i = 1; i < t.Count(); ++i)
                    {
                        var nextNode = node.Nodes.Where(n => n.Id.Equals(t[i])).FirstOrDefault();
                        if (nextNode != null)
                        {
                            nextNode.Quantity += 1;
                        }
                        else
                        {
                            nextNode = new Node<T>(t[i]);
                            node.Nodes.Add(nextNode);
                        }
                        node = nextNode;
                    }
                }
            }
        }



        private Tree<T> BuildConditionalTree(T id)
        {
            List<List<Node<T>>> paths = GetPaths(Root, id).ToList();
            List<Node<T>> nodes = paths.Select(p => p.Last()).ToList();

            for (int i = 0; i < paths.Count(); ++i)
            {
                if (paths[i].Count() <= 1)
                {
                    paths.RemoveAt(i);
                }
                else
                {
                    paths[i].RemoveAt(0);
                    paths[i].RemoveAt(paths[i].Count() - 1);
                }
            }

            paths = SortTransactions(paths);

            Tree<T> conditionalTree = new Tree<T>(id);

            foreach (var path in paths)
            {
                if (path.Count() > 0)
                {
                    var node = conditionalTree.Root.Nodes.Where(n => n.Id.Equals(path[0].Id)).FirstOrDefault();
                    if (node != null)
                    {
                        node.Quantity += 1;
                    }
                    else
                    {
                        node = path[0];
                        node.Quantity = 1;
                        node.Nodes = new List<Node<T>>();
                        conditionalTree.Root.Nodes.Add(node);
                    }
                    for (int i = 1; i < path.Count(); ++i)
                    {
                        var nextNode = node.Nodes.Where(n => n.Id.Equals(path[i].Id)).FirstOrDefault();
                        if (nextNode != null)
                        {
                            nextNode.Quantity += 1;
                        }
                        else
                        {
                            nextNode = path[i];
                            nextNode.Quantity = 1;
                            nextNode.Nodes = new List<Node<T>>();
                            node.Nodes.Add(nextNode);
                        }
                        node = nextNode;
                    }
                }
            }
            return conditionalTree;
        }

        private List<List<T>> SortTransactions(List<List<T>> transactions)
        {
            var list = transactions.SelectMany(x => x).ToList();
            for (int i = 0; i < transactions.Count(); ++i)
            {
                transactions[i] = transactions[i]
                    .OrderByDescending(t => list.Where(l => l.Equals(t)).Count()).ToList();
            }
            return transactions;
        }

        private List<List<Node<T>>> SortTransactions(List<List<Node<T>>> paths)
        {
            var list = paths.SelectMany(p => p).Select(x => x.Id).ToList();

            paths = paths.Select(path => path.OrderByDescending(el => list.Where(l => l.Equals(el.Id)).Count()).ToList()).ToList();

            return paths;
        }

        public List<T> GetAssociations(Node<T> root)
        {
            Dictionary<T, int> dict = new Dictionary<T, int>();
            GetPopularityLvl(root, dict);

            List<T> list = new List<T>();
            dict.OrderByDescending(kv => kv.Value).ToList();
            list = dict.Where(kv => kv.Value >= MinSupport).Select(kv => kv.Key).ToList();

            return list;
        }

        private void GetPopularityLvl(Node<T> root, Dictionary<T, int> dict)
        {
            foreach (var n in root.Nodes)
            {
                GetPopularityLvl(n, dict);
            }

            if (dict.ContainsKey(root.Id))
            {
                dict[root.Id] += root.Quantity;
            }
            else
            {
                dict[root.Id] = root.Quantity;
            }
        }

        public IEnumerable<List<Node<T>>> GetPaths(Node<T> root, T targetId, List<Node<T>> prevNodes = null)
        {
            if (prevNodes == null)
            {
                prevNodes = new List<Node<T>>();
            }
            else
            {
                prevNodes = prevNodes.ToList();
            }


            prevNodes.Add(root);

            if (root.Id.Equals(targetId))
            {
                yield return prevNodes;
            }

            foreach (var child in root.Nodes)
            {
                foreach (var childPath in GetPaths(child, targetId, prevNodes))
                {
                    yield return childPath;
                }
            }

        }
    }
}
