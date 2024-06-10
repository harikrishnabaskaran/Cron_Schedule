using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var scheduler = services.GetRequiredService<IScheduler>();
            await scheduler.Start();
        }

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                services.AddSingleton<SampleJob>();
                services.AddSingleton(new JobSchedule(
                    jobType: typeof(SampleJob),
                    cronExpression: "*/3 * * * * ?")); // every 3 seconds

                services.AddSingleton<QuartzHostedService>();
                services.AddHostedService(provider => provider.GetRequiredService<QuartzHostedService>());

                services.AddSingleton<IScheduler>(provider =>
                {
                    var schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
                    return schedulerFactory.GetScheduler().Result;
                });
            });
}

public class SampleJob : IJob
{
    private readonly Queue<string> _urlsQueue;

    public SampleJob()
    {
        _urlsQueue = new Queue<string>(new[]
        {
            "https://gmcdotnet.digitechniq.in/api/product/RefreshAllProduct/IN",
            "https://gmcdotnet.digitechniq.in/api/product/RefreshAllProduct/AE",
            "https://gmcdotnet.digitechniq.in/api/product/RefreshAllProduct/MX",
            "https://gmcdotnet.digitechniq.in/api/product/RefreshAllProduct/CA",
            "https://gmcdotnet.digitechniq.in/api/product/RefreshAllProduct/US"
        });
    }

    public Task Execute(IJobExecutionContext context)
    {
        while (_urlsQueue.Count > 0)
        {
            string url = _urlsQueue.Dequeue();
            Console.WriteLine("Job executed at: " + DateTime.Now + " URL: " + url);
        }

        Console.WriteLine("No more URLs to display");

        return Task.CompletedTask;
    }
}

public class JobSchedule
{
    public Type JobType { get; }
    public string CronExpression { get; }

    public JobSchedule(Type jobType, string cronExpression)
    {
        JobType = jobType;
        CronExpression = cronExpression;
    }
}

public class QuartzHostedService : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly JobSchedule _jobSchedule;
    private IScheduler _scheduler;

    public QuartzHostedService(ISchedulerFactory schedulerFactory, JobSchedule jobSchedule)
    {
        _schedulerFactory = schedulerFactory;
        _jobSchedule = jobSchedule;
        _scheduler = null!;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scheduler = await _schedulerFactory.GetScheduler();
        var job = JobBuilder.Create(_jobSchedule.JobType)
            .WithIdentity(_jobSchedule.JobType.FullName ?? throw new ArgumentNullException(nameof(_jobSchedule.JobType.FullName)))
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{_jobSchedule.JobType.FullName}.trigger")
            .WithCronSchedule(_jobSchedule.CronExpression)
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
        await _scheduler.Start();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_scheduler != null)
        {
            await _scheduler.Shutdown();
        }
    }
}
