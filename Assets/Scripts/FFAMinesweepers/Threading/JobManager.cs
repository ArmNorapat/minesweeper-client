using System;
using System.Collections.Concurrent;
using TrueAxion.FFAMinesweepers.Utilities;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Threading
{
    /// <summary>
    /// This class is uses for do action that want to run on main thread.
    /// </summary>
    public class JobManager : MonoSingleton<JobManager>
    {
        private ConcurrentQueue<Action> jobs = new ConcurrentQueue<Action>();

        public void AddJob(Action newJob)
        {
            if (newJob != null)
            {
                jobs.Enqueue(newJob);
            }
        }

        private void Update()
        {
            while (jobs.Count > 0)
            {
                if (jobs.TryDequeue(out Action job))
                {
                    job.Invoke();
                }
            }
        }
    }
}