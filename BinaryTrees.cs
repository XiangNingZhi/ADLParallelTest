using System;
using System.Threading.Tasks;

namespace ADLParallelTest
{
    class BinaryTrees
    {
        class TreeNode
        {
            readonly TreeNode left, right;

            internal TreeNode(int d)
            {
                if (d != 0)
                {
                    left = new TreeNode(d - 1);
                    right = new TreeNode(d - 1);
                }
            }

            internal static int Check(TreeNode current)
            {
                int c = 0;
                while (current != null)
                {
                    c += Check(current.right) + 1;
                    current = current.left;
                }
                return c;
            }
        }

        const int MinDepth = 4;
        const int NoTasks = 4;

        public static void Test(int args)
        {
            int maxDepth = Math.Max(MinDepth + 2, args);

            //Console.WriteLine(string.Concat("stretch tree of depth ", maxDepth + 1,
            //    "\t check: ", TreeNode.Check(new TreeNode(maxDepth + 1))));

            var longLivedTree = new TreeNode(maxDepth);

            var results = new string[(maxDepth - MinDepth) / 2 + 1];

            for (int i = 0; i < results.Length; i++)
            {
                int depth = i * 2 + MinDepth;
                int n = (1 << maxDepth - depth + MinDepth) / NoTasks;
                var tasks = new Task<int>[NoTasks];
                for (int t = 0; t < tasks.Length; t++)
                {
                    tasks[t] = Task.Run(() =>
                    {
                        var check1 = 0;
                        for (int j = n; j > 0; j--)
                            check1 += TreeNode.Check(new TreeNode(depth));
                        return check1;
                    });
                }
                var check = tasks[0].Result;
                for (int t = 1; t < tasks.Length; t++)
                    check += tasks[t].Result;
                results[i] = string.Concat(n * NoTasks, "\t trees of depth ",
                    depth, "\t check: ", check);
            }

            //for (int i = 0; i < results.Length; i++)
            //    Console.WriteLine(results[i]);

            //Console.WriteLine(string.Concat("long lived tree of depth ", maxDepth,
            //    "\t check: ", TreeNode.Check(longLivedTree)));
        }
    }
}