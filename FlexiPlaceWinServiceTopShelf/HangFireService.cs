using System;
using System.Collections.Generic;
using Hangfire;
using Hangfire.Common;
using Hangfire.Storage;
using log4net;

namespace FlexiPlaceWinServiceTopShelf
{
    /// <summary>
    /// HangFireService - We will add all the jobs to job manager,
    /// and we will use CRON expressions to schedule each job
    /// </summary>
    public class HangFireService
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Schedule the jobs
        /// </summary>
        public void ScheduleJobs()
        {
            StopJobs();
            var jobs = AddJobs();
            foreach (var job in jobs)
            {
                ScheduleJob(job);
            }
        }

        /// <summary>
        /// Schedule each job based on the job configuration details
        /// </summary>
        /// <param name="job"></param>
        private void ScheduleJob(JobConfigDetails job)
        {
            Logger.Info($"Starting job: {job.Id}");
            if (string.IsNullOrEmpty(job.Cron) || string.IsNullOrEmpty(job.TypeName))
            {
                return;
            }

            try
            {
                var jobManager = new RecurringJobManager();
                jobManager.RemoveIfExists(job.Id);
                var type = Type.GetType(job.TypeName);
                if (type != null && job.Enabled)
                {
                    var jobSchedule = new Job(type, type.GetMethod("Start"));
                    jobManager.AddOrUpdate(job.Id, jobSchedule, job.Cron, TimeZoneInfo.Local);
                    Logger.Info($"Job {job.Id} has started");
                }
                else
                {
                    Logger.Info($"Job {job.Id} of type {type} is not found or job is disabled");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception has been thrown when starting the job {ex.Message}");
            }
        }

        /// <summary>
        /// Stop and remove jobs from job manager
        /// </summary>
        public void StopJobs()
        {
            using (var conn = JobStorage.Current.GetConnection())
            {
                var manager = new RecurringJobManager();
                foreach (var job in conn.GetRecurringJobs())
                {
                    manager.RemoveIfExists(job.Id);
                    //LogManager.Log($"Job has been stopped: {job.Id}", LogLevel.Information);
                }
            }
        }

        /// <summary>
        /// Add all jobs with in a specific type name to the list
        /// Configure the job id, CRON expression for each job
        /// </summary>
        /// <returns></returns>
        public List<JobConfigDetails> AddJobs()
        {
            var configDetails = new List<JobConfigDetails>();

            configDetails.Add(new JobConfigDetails
            {
                Id = "FlexiPlacePromjenaStatusaOdobrenoNeobradjeno",
                Enabled = true,
                Cron = "0 0 * * *",
                TypeName = "FlexiPlaceWinServiceTopShelf.Service,FlexiPlaceWinServiceTopShelf"
            });
            return configDetails;
        }
    }

    /// <summary>
    /// Job configuration properties
    /// </summary>
    public class JobConfigDetails
    {
        public string Id { get; set; }
        public bool Enabled { get; set; }
        public string Cron { get; set; }
        public string TypeName { get; set; }
    }
}
