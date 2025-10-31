using SchedulerApi.DTOs;
using System.Linq;

namespace SchedulerApi.Services
{
    public interface ISchedulerService
    {
        ScheduleResponse Recommend(ScheduleRequest request);
    }

    public class SchedulerService : ISchedulerService
    {
        public ScheduleResponse Recommend(ScheduleRequest request)
        {
            var tasks = request.Tasks;
            var titleToTask = tasks.ToDictionary(t => t.Title);
            var inDegree = tasks.ToDictionary(t => t.Title, _ => 0);
            var adj = tasks.ToDictionary(t => t.Title, _ => new List<string>());

            foreach (var t in tasks)
            {
                foreach (var dep in t.Dependencies)
                {
                    if (!titleToTask.ContainsKey(dep)) continue;
                    adj[dep].Add(t.Title);
                    inDegree[t.Title]++;
                }
            }

            int Compare(string a, string b)
            {
                var ta = titleToTask[a];
                var tb = titleToTask[b];
                var da = ta.DueDate ?? DateTime.MaxValue;
                var db = tb.DueDate ?? DateTime.MaxValue;
                var cmp = da.CompareTo(db);
                if (cmp != 0) return cmp;
                cmp = ta.EstimatedHours.CompareTo(tb.EstimatedHours);
                if (cmp != 0) return cmp;
                return string.Compare(a, b, StringComparison.Ordinal);
            }

            var ready = inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key).ToList();
            ready.Sort(Compare);

            var order = new List<string>();
            while (ready.Count > 0)
            {
                var cur = ready[0];
                ready.RemoveAt(0);
                order.Add(cur);
                foreach (var nxt in adj[cur])
                {
                    inDegree[nxt]--;
                    if (inDegree[nxt] == 0)
                    {
                        ready.Add(nxt);
                    }
                }
                ready.Sort(Compare);
            }

            if (order.Count != tasks.Count)
            {
                var remaining = tasks.Select(t => t.Title).Except(order).ToList();
                remaining.Sort(Compare);
                order.AddRange(remaining);
            }

            return new ScheduleResponse { RecommendedOrder = order };
        }
    }
}
