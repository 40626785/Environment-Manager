using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;


namespace EnvironmentManager.ViewModels;

public partial class MaintenanceViewModel : ObservableObject, IQueryAttributable
{
    private Models.Maintenance _maintenance;

    public DateTime DueDate
    {
        get => _maintenance.DueDate;
        set
        {
            if (_maintenance.DueDate.Date != value.Date)
            {
                _maintenance.DueDate = value;
                OnPropertyChanged();
            }
        }
    } 

    public int Id => _maintenance.Id;

    public Boolean Overdue 
    {
        get => _maintenance.Overdue;
        set
        {
            if (_maintenance.Overdue != value)
            {
                _maintenance.Overdue = value;
                OnPropertyChanged();
            }
        }
    }

    public int Priority {
        get => _maintenance.Priority;
        set
        {
            if (_maintenance.Priority != value)
            {
                _maintenance.Priority = value;
                OnPropertyChanged();
            }
        }
    }

    public string Description
    {
        get => _maintenance.Description;
        set
        {
            if (_maintenance.Description != value)
            {
                _maintenance.Description = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime CurrentDate;


    private MaintenanceDbContext _context;

    
    public MaintenanceViewModel(MaintenanceDbContext maintenanceDbContext)
    {
        _context = maintenanceDbContext;
        CurrentDate = DateTime.Now;
        _maintenance = new Maintenance();
        _maintenance.DueDate = CurrentDate;
    }
    public MaintenanceViewModel(MaintenanceDbContext maintenanceDbContext, Maintenance maintenance)
    {
        _maintenance = maintenance;
        _context = maintenanceDbContext;
        CurrentDate = DateTime.Now;
        isOverdue();
    }
    [RelayCommand]
    private async Task Save()
    {
        Trace.WriteLine($"blah blah blah {DueDate}");
        _context.Maintenance.Update(_maintenance);
        _context.SaveChanges();
        await Shell.Current.GoToAsync($"..?saved={_maintenance.Id}");
    }

    [RelayCommand]
    private async Task Delete()
    {
        _context.Remove(_maintenance);
        _context.SaveChanges();
        await Shell.Current.GoToAsync($"..?deleted={_maintenance.Id}");
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("edit"))
        {
            _maintenance = _context.Maintenance.Single(n => n.Id == int.Parse(query["edit"].ToString()));
            RefreshProperties();
        }
    }

    public void isOverdue() {
        Reload();
        _maintenance.Overdue = DueDate.Date < DateTime.Now.Date;
        _context.Maintenance.Update(_maintenance);
        _context.SaveChanges();
    }

    public void Reload()
    {
        _context.Entry(_maintenance).Reload();
        RefreshProperties();
    }

    private void RefreshProperties()
    {
        OnPropertyChanged(nameof(Id));
        OnPropertyChanged(nameof(Overdue));
        OnPropertyChanged(nameof(DueDate));
        OnPropertyChanged(nameof(Priority));
        OnPropertyChanged(nameof(Description));
    }
}
