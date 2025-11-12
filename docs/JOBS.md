# Background Jobs Documentation

Complete guide for Quartz.NET background jobs in FleetManager.

## Table of Contents

- [Overview](#overview)
- [MaintenanceCheckJob](#maintenancecheckjob)
- [Configuration](#configuration)
- [Monitoring](#monitoring)
- [Creating New Jobs](#creating-new-jobs)
- [Troubleshooting](#troubleshooting)

---

## Overview

FleetManager uses Quartz.NET for scheduling and executing background jobs. Currently, the system has one scheduled job that runs daily to check for vehicles with upcoming maintenance.

### Why Quartz.NET?

- **Reliable Scheduling**: Persistent job storage and execution
- **Flexible Scheduling**: Cron expressions for complex schedules
- **Dependency Injection**: Full DI support for job dependencies
- **Monitoring**: Built-in job execution tracking
- **Clustering**: Support for distributed job execution (future)

---

## MaintenanceCheckJob

The MaintenanceCheckJob runs on a scheduled basis to identify vehicles that require maintenance soon and log alerts.

### Purpose

- Identify vehicles with maintenance due within a configurable threshold (default: 3 days)
- Log alert messages for each vehicle requiring attention
- Provide proactive maintenance notifications

### Implementation

**Location:** `src/FleetManager.Infrastructure/Jobs/MaintenanceCheckJob.cs`

```csharp
public class MaintenanceCheckJob : IJob
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<MaintenanceCheckJob> _logger;
    private readonly IConfiguration _configuration;
    private const int DefaultDaysThreshold = 3;
    
    public MaintenanceCheckJob(
        IVehicleRepository vehicleRepository,
        ILogger<MaintenanceCheckJob> logger,
        IConfiguration configuration)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
        _configuration = configuration;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var daysThreshold = _configuration.GetValue<int>(
            "Quartz:MaintenanceDaysThreshold", 
            DefaultDaysThreshold);
        
        _logger.LogInformation(
            "MaintenanceCheckJob iniciado. Verificando manutenções nos próximos {Days} dias.",
            daysThreshold);
        
        var vehicles = await _vehicleRepository
            .GetVehiclesWithUpcomingMaintenanceAsync(daysThreshold);
        
        foreach (var vehicle in vehicles)
        {
            _logger.LogInformation(
                "[ALERTA] Veículo {Plate} ({Model}) com manutenção próxima em {NextMaintenanceDate:dd/MM/yyyy}. " +
                "Última manutenção: {LastMaintenanceDate:dd/MM/yyyy}",
                vehicle.Plate,
                vehicle.Model,
                vehicle.NextMaintenanceDate,
                vehicle.LastMaintenanceDate);
        }
        
        _logger.LogInformation(
            "MaintenanceCheckJob concluído. {Count} veículo(s) com manutenção próxima.",
            vehicles.Count());
    }
}
```

### Execution Flow

1. Job is triggered by Quartz scheduler based on cron expression
2. Retrieves threshold from configuration (default: 3 days)
3. Queries database for vehicles with `NextMaintenanceDate` within threshold
4. Logs alert for each vehicle found
5. Logs summary with total count

### Output Example

```
[10:00:00 INF] MaintenanceCheckJob iniciado. Verificando manutenções nos próximos 3 dias.
[10:00:00 INF] [ALERTA] Veículo ABC1234 (Toyota Corolla) com manutenção próxima em 14/11/2024. Última manutenção: 15/08/2024
[10:00:00 INF] [ALERTA] Veículo XYZ5678 (Honda Civic) com manutenção próxima em 15/11/2024. Última manutenção: 16/08/2024
[10:00:00 INF] MaintenanceCheckJob concluído. 2 veículo(s) com manutenção próxima.
```

---

## Configuration

### Quartz Configuration in Program.cs

**Location:** `src/FleetManager.Api/Program.cs`

```csharp
// Add Quartz services
builder.Services.AddQuartz(q =>
{
    // Use Microsoft DI for job creation
    q.UseMicrosoftDependencyInjectionJobFactory();
    
    // Define job
    var jobKey = new JobKey("MaintenanceCheckJob");
    q.AddJob<MaintenanceCheckJob>(opts => opts.WithIdentity(jobKey));
    
    // Define trigger with cron schedule
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("MaintenanceCheckJob-trigger")
        .WithCronSchedule(
            builder.Configuration["Quartz:MaintenanceCheckCron"] ?? "0 0 0 * * ?"));
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
```

### Schedule Configuration

**appsettings.json:**

```json
{
  "Quartz": {
    "MaintenanceCheckCron": "0 0 0 * * ?",
    "MaintenanceDaysThreshold": 3
  }
}
```

**appsettings.Development.json (more frequent for testing):**

```json
{
  "Quartz": {
    "MaintenanceCheckCron": "0 */5 * * * ?",
    "MaintenanceDaysThreshold": 7
  }
}
```

### Cron Expression Examples

| Expression | Description | Use Case |
|------------|-------------|----------|
| `0 0 0 * * ?` | Daily at midnight | Production default |
| `0 0 */6 * * ?` | Every 6 hours | Frequent checks |
| `0 */5 * * * ?` | Every 5 minutes | Development/testing |
| `0 0 9 * * MON-FRI` | Weekdays at 9 AM | Business hours only |
| `0 0 0 1 * ?` | First day of month | Monthly reports |

### Cron Expression Format

```
┌───────────── second (0-59)
│ ┌───────────── minute (0-59)
│ │ ┌───────────── hour (0-23)
│ │ │ ┌───────────── day of month (1-31)
│ │ │ │ ┌───────────── month (1-12)
│ │ │ │ │ ┌───────────── day of week (0-6) (Sunday=0)
│ │ │ │ │ │
* * * * * *
```

**Special Characters:**
- `*` - Any value
- `?` - No specific value (day of month or day of week)
- `-` - Range (e.g., `1-5`)
- `,` - List (e.g., `1,3,5`)
- `/` - Increment (e.g., `*/5` = every 5)

---

## Monitoring

### Viewing Job Execution Logs

**Console Output:**

```bash
dotnet run --project src/FleetManager.Api
```

Look for log entries with `MaintenanceCheckJob` in the message.

**Log Files:**

```bash
# View latest log file
cat logs/fleetmanager-*.log | grep MaintenanceCheckJob

# Tail log file in real-time
tail -f logs/fleetmanager-*.log | grep MaintenanceCheckJob
```

**Seq Dashboard:**

1. Open http://localhost:5341
2. Search for: `MaintenanceCheckJob`
3. Filter by log level: Information
4. View structured log data

### Verifying Job Schedule

Check that the job is registered and scheduled:

```csharp
// Add to a controller for debugging
[HttpGet("jobs/status")]
public async Task<IActionResult> GetJobStatus(
    [FromServices] ISchedulerFactory schedulerFactory)
{
    var scheduler = await schedulerFactory.GetScheduler();
    var jobKey = new JobKey("MaintenanceCheckJob");
    
    var jobDetail = await scheduler.GetJobDetail(jobKey);
    var triggers = await scheduler.GetTriggersOfJob(jobKey);
    
    return Ok(new
    {
        JobExists = jobDetail != null,
        Triggers = triggers.Select(t => new
        {
            t.Key.Name,
            NextFireTime = t.GetNextFireTimeUtc()?.LocalDateTime,
            PreviousFireTime = t.GetPreviousFireTimeUtc()?.LocalDateTime
        })
    });
}
```

### Manual Job Execution

For testing, you can trigger the job manually:

```csharp
[HttpPost("jobs/maintenance/trigger")]
public async Task<IActionResult> TriggerMaintenanceJob(
    [FromServices] ISchedulerFactory schedulerFactory)
{
    var scheduler = await schedulerFactory.GetScheduler();
    var jobKey = new JobKey("MaintenanceCheckJob");
    
    await scheduler.TriggerJob(jobKey);
    
    return Ok(new { Message = "Job triggered successfully" });
}
```

---

## Creating New Jobs

### Step 1: Create Job Class

Create a new job class in `src/FleetManager.Infrastructure/Jobs/`:

```csharp
public class MyCustomJob : IJob
{
    private readonly IMyService _myService;
    private readonly ILogger<MyCustomJob> _logger;
    
    public MyCustomJob(IMyService myService, ILogger<MyCustomJob> logger)
    {
        _myService = myService;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("MyCustomJob started");
        
        try
        {
            // Your job logic here
            await _myService.DoSomethingAsync();
            
            _logger.LogInformation("MyCustomJob completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MyCustomJob failed");
            throw; // Re-throw to mark job as failed
        }
    }
}
```

### Step 2: Register Job in Program.cs

```csharp
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    
    // Existing MaintenanceCheckJob
    var maintenanceJobKey = new JobKey("MaintenanceCheckJob");
    q.AddJob<MaintenanceCheckJob>(opts => opts.WithIdentity(maintenanceJobKey));
    q.AddTrigger(opts => opts
        .ForJob(maintenanceJobKey)
        .WithIdentity("MaintenanceCheckJob-trigger")
        .WithCronSchedule("0 0 0 * * ?"));
    
    // New custom job
    var customJobKey = new JobKey("MyCustomJob");
    q.AddJob<MyCustomJob>(opts => opts.WithIdentity(customJobKey));
    q.AddTrigger(opts => opts
        .ForJob(customJobKey)
        .WithIdentity("MyCustomJob-trigger")
        .WithCronSchedule("0 0 */6 * * ?")); // Every 6 hours
});
```

### Step 3: Add Configuration

**appsettings.json:**

```json
{
  "Quartz": {
    "MaintenanceCheckCron": "0 0 0 * * ?",
    "MaintenanceDaysThreshold": 3,
    "MyCustomJobCron": "0 0 */6 * * ?"
  }
}
```

### Step 4: Write Tests

```csharp
public class MyCustomJobTests
{
    private readonly Mock<IMyService> _mockService;
    private readonly Mock<ILogger<MyCustomJob>> _mockLogger;
    private readonly MyCustomJob _job;
    
    public MyCustomJobTests()
    {
        _mockService = new Mock<IMyService>();
        _mockLogger = new Mock<ILogger<MyCustomJob>>();
        _job = new MyCustomJob(_mockService.Object, _mockLogger.Object);
    }
    
    [Fact]
    public async Task Execute_CallsService()
    {
        // Arrange
        var context = Mock.Of<IJobExecutionContext>();
        
        // Act
        await _job.Execute(context);
        
        // Assert
        _mockService.Verify(s => s.DoSomethingAsync(), Times.Once);
    }
}
```

---

## Best Practices

### Error Handling

Always wrap job logic in try-catch and log errors:

```csharp
public async Task Execute(IJobExecutionContext context)
{
    try
    {
        _logger.LogInformation("Job started");
        
        // Job logic
        
        _logger.LogInformation("Job completed");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Job failed with error: {Message}", ex.Message);
        throw; // Re-throw to mark job as failed
    }
}
```

### Idempotency

Ensure jobs can be safely re-run:

```csharp
public async Task Execute(IJobExecutionContext context)
{
    // Check if work is already done
    var lastRun = await _repository.GetLastJobRunAsync("MyJob");
    if (lastRun?.Date.Date == DateTime.UtcNow.Date)
    {
        _logger.LogInformation("Job already ran today, skipping");
        return;
    }
    
    // Do work
    await DoWorkAsync();
    
    // Record completion
    await _repository.RecordJobRunAsync("MyJob", DateTime.UtcNow);
}
```

### Timeouts

Set reasonable timeouts for long-running jobs:

```csharp
public async Task Execute(IJobExecutionContext context)
{
    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(30));
    
    try
    {
        await DoWorkAsync(cts.Token);
    }
    catch (OperationCanceledException)
    {
        _logger.LogWarning("Job timed out after 30 minutes");
        throw;
    }
}
```

### Dependency Injection

Use constructor injection for all dependencies:

```csharp
public class MyJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public MyJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        // Create scope for scoped services
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IMyService>();
        
        await service.DoWorkAsync();
    }
}
```

---

## Troubleshooting

### Job Not Running

**Check job is registered:**

```csharp
// Add temporary endpoint
[HttpGet("debug/jobs")]
public async Task<IActionResult> ListJobs(
    [FromServices] ISchedulerFactory schedulerFactory)
{
    var scheduler = await schedulerFactory.GetScheduler();
    var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
    
    var jobs = new List<object>();
    foreach (var jobKey in jobKeys)
    {
        var detail = await scheduler.GetJobDetail(jobKey);
        var triggers = await scheduler.GetTriggersOfJob(jobKey);
        
        jobs.Add(new
        {
            JobName = jobKey.Name,
            JobType = detail.JobType.Name,
            Triggers = triggers.Select(t => new
            {
                TriggerName = t.Key.Name,
                NextFire = t.GetNextFireTimeUtc()?.LocalDateTime,
                PreviousFire = t.GetPreviousFireTimeUtc()?.LocalDateTime
            })
        });
    }
    
    return Ok(jobs);
}
```

**Check cron expression is valid:**

Use online cron expression validator: https://crontab.guru/

**Check logs for errors:**

```bash
grep -i "quartz" logs/fleetmanager-*.log
grep -i "error" logs/fleetmanager-*.log
```

### Job Failing

**Check exception details in logs:**

```bash
grep -A 10 "MaintenanceCheckJob failed" logs/fleetmanager-*.log
```

**Verify dependencies are registered:**

Ensure all services used by the job are registered in DI container.

**Test job logic independently:**

Write unit tests to isolate and test job logic.

### Job Running Multiple Times

**Check for duplicate registrations:**

Ensure job is only registered once in `Program.cs`.

**Check cron expression:**

Verify cron expression matches intended schedule.

**Check for clustering issues:**

If running multiple instances, ensure proper clustering configuration.

### Performance Issues

**Add timing logs:**

```csharp
public async Task Execute(IJobExecutionContext context)
{
    var sw = Stopwatch.StartNew();
    _logger.LogInformation("Job started");
    
    await DoWorkAsync();
    
    sw.Stop();
    _logger.LogInformation("Job completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);
}
```

**Optimize database queries:**

Use indexes and efficient queries for large datasets.

**Consider batching:**

Process large datasets in batches to avoid memory issues.

---

## Future Enhancements

### Job Persistence

Store job state in database for reliability:

```csharp
builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(s =>
    {
        s.UseSqlServer(connectionString);
        s.UseJsonSerializer();
    });
});
```

### Job Clustering

Run jobs across multiple instances:

```csharp
builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(s =>
    {
        s.UseClustering();
    });
});
```

### Job Dashboard

Add Quartz.NET dashboard for monitoring:

```bash
dotnet add package Quartz.AspNetCore
```

```csharp
app.UseQuartzDashboard();
```

Access at: http://localhost:5000/quartz
