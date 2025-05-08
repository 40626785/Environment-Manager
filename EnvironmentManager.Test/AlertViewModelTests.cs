
using Xunit;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using Microsoft.Maui.Controls;


namespace EnvironmentManager.Test;

public class AlertViewModelTests
{

    private DbContextOptions<AlertDbContext> CreateOptions() =>
    new DbContextOptionsBuilder<AlertDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

    private async Task SeedAlertsAsync(AlertDbContext context)
    {


        await context.AlertTable.AddRangeAsync(new[]
        {
        new Alert
        {
            AlertId = 1,
            LocationId = 101,
            Date_Time = DateTime.Now,
            Parameter = "PM2.5",
            Value = 55.5,
            Message = "High PM2.5 levels detected",
            IsResolved = false
        },
        new Alert
        {
            AlertId = 2,
            LocationId = 102,
            Date_Time = DateTime.Now.AddMinutes(-10),
            Parameter = "NO2",
            Value = 40.2,
            Message = "NO2 exceeded threshold",
            IsResolved = true
        }
    });
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task LoadActiveAlerts_LoadsOnlyUnresolved()
    {
        var options = CreateOptions();
        using var context = new AlertDbContext(options);
        await SeedAlertsAsync(context);

        var factory = new Mock<IDbContextFactory<AlertDbContext>>();
        factory.Setup(f => f.CreateDbContext()).Returns(() => new AlertDbContext(options));

        var viewModel = new AlertViewModel(factory.Object);
        await Task.Delay(500); // Give time for constructor to call LoadActiveAlerts

        Assert.Single(viewModel.ActiveAlerts);
        Assert.False(viewModel.ActiveAlerts.First().IsResolved);
    }

    [Fact]
    public async Task MarkAsResolved_UpdatesAlert()
    {
        var options = CreateOptions();
        using var context = new AlertDbContext(options);
        await SeedAlertsAsync(context);

        var factory = new Mock<IDbContextFactory<AlertDbContext>>();
        factory.Setup(f => f.CreateDbContext()).Returns(() => new AlertDbContext(options));

        var viewModel = new AlertViewModel(factory.Object);
        await Task.Delay(500);

        await viewModel.MarkAsResolvedCommand.ExecuteAsync(1);

        using var verifyContext = new AlertDbContext(options);
        var updated = await verifyContext.AlertTable.FindAsync(1);
        Assert.True(updated.IsResolved);
    }


}
