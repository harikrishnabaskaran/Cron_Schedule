# Quartz.NET Cron Job Example

This repository demonstrates how to set up a cron job in a .NET application using Quartz.NET. The cron job executes a task every 3 seconds, printing URLs to the console.

## Table of Contents

- [Introduction](#introduction)
- [Prerequisites](#prerequisites)
- [Setup](#setup)
- [Usage](#usage)
- [Contributing](#contributing)
- [Output](#Output)
## Introduction

This project sets up a basic cron job using Quartz.NET to execute a task every 3 seconds. The task in this example dequeues and prints URLs to the console. This project is useful for learning how to integrate Quartz.NET into your .NET applications for scheduled tasks.

## Prerequisites

- .NET SDK 5.0 or later
- Basic knowledge of C# and .NET
- Git

## Setup

1. **Clone the repository**:
    ```bash
    git clone https://github.com/your-username/quartz-net-cron-job.git
    cd quartz-net-cron-job
    ```

2. **Restore dependencies**:
    ```bash
    dotnet restore
    ```

3. **Build the project**:
    ```bash
    dotnet build
    ```

## Usage

1. **Run the application**:
    ```bash
    dotnet run
    ```

    The application will start and the cron job will execute every 3 seconds, printing URLs to the console.

## Code Overview

- **Program.cs**: The main entry point of the application. It sets up the dependency injection container and starts the Quartz.NET scheduler.
- **SampleJob.cs**: A sample job that dequeues and prints URLs.
- **JobSchedule.cs**: A class to define the job type and cron expression.
- **QuartzHostedService.cs**: A hosted service that manages the Quartz.NET scheduler and job lifecycle.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## Output

![Cron Scheduler Output](https://github.com/harikrishnabaskaran/Cron_Schedule/assets/136921665/26ed055e-ddaa-4228-902e-6699969e726d)

