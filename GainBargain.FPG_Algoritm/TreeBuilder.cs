using System.Collections.Generic;

namespace GainBargain.FPG_Algoritm
{
    public class TreeBuilder
    {
        public static Tree<int> Tree;
        public static void Build(List<List<int>> list)
        {
            Tree = new Tree<int>(list, 2);
        }
    }
}
