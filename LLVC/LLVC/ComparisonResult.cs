using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLVC
{
   public class ComparisonResult
    {
        public enum ResultType
        {
            Synchronous,
            AIsAhead,
            BIsAhead,
            NotComparable,
            Dismerged
        }

        public ResultType Type { get; set; }
        public int HeadStart { get; set; } = -1;
        public int DismergedAt { get; set; } = -1;

        public ComparisonResult(Protocol protocolA, Protocol protocolB)
        {
            if (protocolA.InitialHash != protocolB.InitialHash)
            {
                Type = ResultType.NotComparable;
                return;
            }

            int c = Math.Min(protocolA.Commits.Count, protocolB.Commits.Count);
            var enumA = protocolA.Commits.GetEnumerator();
            var enumB = protocolB.Commits.GetEnumerator();
            for (int i = 0; i < c; i++)
            {
                enumA.MoveNext();
                enumB.MoveNext();
                var commitA = enumA.Current;
                var commitB = enumB.Current;

                if (commitA.Hash != commitB.Hash)
                {
                    this.DismergedAt = i;
                    this.Type = ResultType.Dismerged;
                    return;
                }
            }

            if (protocolA.Commits.Count > c)
            {
                this.Type = ResultType.AIsAhead;
                this.HeadStart = protocolA.Commits.Count - c;
            }else if (protocolB.Commits.Count > c)
            {
                this.Type = ResultType.BIsAhead;
                this.HeadStart = protocolB.Commits.Count - c;
            }
            else
            {
                this.Type = ResultType.Synchronous;
                this.HeadStart = 0;
            }
        }
    }
}
