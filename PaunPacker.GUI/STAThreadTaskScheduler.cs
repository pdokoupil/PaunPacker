using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PaunPacker.GUI
{
    /// <summary>
    /// Task scheduler which executes task in the STAThread for the Main window
    /// The tasks are executed (intionally) one by one
    /// </summary>
    internal sealed class STAThreadTaskScheduler : TaskScheduler, IDisposable
    {
        /// <summary>
        /// Returns an instance of the scheduler
        /// </summary>
        public static STAThreadTaskScheduler Scheduler { get; } = new STAThreadTaskScheduler();

        /// <summary>
        /// The currently executing task
        /// </summary>
        public Task CurrentTask { get; private set; }

        /// <summary>
        /// Number of scheduled tasks
        /// </summary>
        public int TaskCount => scheduledTasks.Count;

        /// <summary>
        /// Returns the scheduled tasks
        /// </summary>
        /// <returns>The scheduled tasks</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return scheduledTasks;
        }

        /// <summary>
        /// Enqueues a new task
        /// </summary>
        /// <param name="task">The new task to be scheduled</param>
        protected override void QueueTask(Task task)
        {
            scheduledTasks.Add(task);
        }

        /// <inheritdoc />
        /// <remarks>Is not supported, therefore it always returns false</remarks>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        /// <summary>
        /// Constructs a new scheduler
        /// </summary>
        private STAThreadTaskScheduler()
        {
            scheduledTasks = new BlockingCollection<Task>();
            workerThread = new Thread(Work)
            {
                IsBackground = true
            };
            workerThread.SetApartmentState(ApartmentState.STA);
            workerThread.Start();
        }

        /// <summary>
        /// Working method of the background worker thread
        /// </summary>
        private void Work()
        {
            foreach (var task in scheduledTasks.GetConsumingEnumerable())
            {
                //In the case of our task scheduler, should always return true
                CurrentTask = task;
                if (!TryExecuteTask(task))
                {
                    //This should never happen
                    throw new InvalidOperationException();
                }
                CurrentTask = null;
            }
        }

        public void Dispose()
        {
            scheduledTasks.Dispose();
        }

        /// <summary>
        /// Scheduled tasks
        /// </summary>
        private readonly BlockingCollection<Task> scheduledTasks;

        /// <summary>
        /// The worker thread responsible for task execution
        /// </summary>
        private readonly Thread workerThread;
    }
}
