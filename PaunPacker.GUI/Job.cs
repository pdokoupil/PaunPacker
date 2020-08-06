using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using PaunPacker.Core;

namespace PaunPacker.GUI
{
    /// <summary>
    /// Represents a Job in the GUI (some action, for example, texture atlas generation, image processing, etc.)
    /// </summary>
    class Job : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructs a job with a given name
        /// </summary>
        /// <remarks>The job name is shown in the progress bar</remarks>
        /// <param name="jobName">The name of the job</param>
        /// <param name="isCancellable">Whether the specified job is cancellable or not</param>
        public Job(string jobName, bool isCancellable = true)
        {
            JobName = jobName;
            IsCancellable = isCancellable;
        }

        /// <summary>
        /// Starts a new job on a worker <paramref name="worker"/>
        /// </summary>
        /// <typeparam name="T">The result type of the job</typeparam>
        /// <param name="job">Delegate which represents the job</param>
        /// <param name="worker">Worker that does the job</param>
        /// <returns>The task represeting result of the worker from performing the job</returns>
        public async Task<T> StartJobAsync<T>(Func<T> job, IProgressReporter worker)
        {
            worker.ProgressChange += Worker_ProgressChange;
            Progress = 0;
            ReportsProgress = worker.ReportsProgress;
            return await Task.Factory.StartNew(job, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                .ContinueWith((x) =>
                {
                    worker.ProgressChange -= Worker_ProgressChange;
                    return x.Result;
                }, TaskScheduler.Default)
                .ConfigureAwait(true);
        }

        /// <summary>
        /// Starts a new job on a worker <paramref name="worker"/>
        /// </summary>
        /// <param name="job">Delegate which represents the job</param>
        /// <param name="worker">Worker that does the job</param>
        /// <returns>The task representing a completion of the job</returns>
        public async Task StartJobAsync(Action job, IProgressReporter worker)
        {
            worker.ProgressChange += Worker_ProgressChange;
            Progress = 0;
            ReportsProgress = worker.ReportsProgress;
            await Task.Factory.StartNew(job, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                .ContinueWith((x) =>
                {
                    worker.ProgressChange -= Worker_ProgressChange;
                }, TaskScheduler.Default)
                .ConfigureAwait(true);
        }

        /// <summary>
        /// Starts a new job
        /// </summary>
        /// <typeparam name="T">The result type of the job</typeparam>
        /// <param name="job">Delegate which represents the job</param>
        /// <returns>The task represting a result of the job</returns>
        public async Task<T> StartJobAsync<T>(Func<T> job)
        {
            Progress = 0;
            ReportsProgress = false;
            return await Task.Factory.StartNew(job, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                .ConfigureAwait(true);
        }

        /// <summary>
        /// Starts a new job
        /// </summary>
        /// <param name="job">Delegate which represents the job</param>
        /// <returns>The task represeting a completion of the job</returns>
        public async Task StartJobAsync(Action job)
        {
            Progress = 0;
            ReportsProgress = false;
            await Task.Factory.StartNew(job, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                .ConfigureAwait(true);
        }

        /// <summary>
        /// Notify the GUI about property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Progress of the worker, 0 if the Job was started without specifying the worker
        /// </summary>
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            }
        }

        /// <summary>
        /// Whether this job is cancellable or not
        /// </summary>
        public bool IsCancellable
        {
            get => isCancellable;
            set
            {
                isCancellable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            }
        }

        /// <summary>
        /// Whether the specified worker reports progress, if the worker was not specified then this value is always false
        /// </summary>
        public bool ReportsProgress
        {
            get => reportsProgress;
            set
            {
                reportsProgress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReportsProgress)));
            }
        }

        /// <summary>
        /// The name of the job
        /// </summary>
        public string JobName
        {
            get => jobName;
            set
            {
                jobName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JobName)));
            }
        }

        /// <summary>
        /// Handler for worker's ProgressChange event
        /// </summary>
        /// <param name="sender">The worker whose progress changed</param>
        /// <param name="newProgress">New progress of the worker</param>
        private void Worker_ProgressChange(object sender, int newProgress)
        {
            Progress = newProgress;
        }

        /// <see cref="ReportsProgress"/>
        private bool reportsProgress;

        /// <see cref="IsCancellable"/>
        private bool isCancellable;

        /// <see cref="Progress"/>
        private int progress;

        /// <see cref="JobName"/>
        private string jobName;
    }
}
