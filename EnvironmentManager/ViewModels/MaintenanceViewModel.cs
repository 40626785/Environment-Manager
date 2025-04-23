using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using System.Diagnostics;
using EnvironmentManager.Interfaces;


namespace EnvironmentManager.ViewModels;

/// <summary>
/// Code behind the MaintenancePage
/// </summary>
public partial class MaintenanceViewModel : ObservableObject, IQueryAttributable, IErrorHandling
{
    private Maintenance _maintenance;

    private IMaintenanceDataStore _context;

    private string _displayError;

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

    public bool Overdue 
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

    public int Priority 
    {
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

    public string DisplayError 
    {
        get => _displayError;
        set
        {
            if (value != _displayError)
            {
                _displayError = value;
                OnPropertyChanged();
            }
        }
    } 

    public DateTime CurrentDate;

    public MaintenanceViewModel(IMaintenanceDataStore maintenanceDbContext)
    {
        _context = maintenanceDbContext;
        CurrentDate = DateTime.Now;
        _maintenance = new Maintenance();
        _maintenance.DueDate = CurrentDate;
        _displayError = "";
    }
    public MaintenanceViewModel(IMaintenanceDataStore maintenanceDbContext, Maintenance maintenance)
    {
        _maintenance = maintenance;
        _context = maintenanceDbContext;
        CurrentDate = DateTime.Now;
        _displayError = "";
    }

    /// <summary>
    /// Adds new or updated maintenance ticket to db context
    /// </summary>
    [RelayCommand]
    private async Task Save()
    {
        try 
        {
            _context.Update(_maintenance);
            _context.Save();
            await Shell.Current.GoToAsync($"..?saved={_maintenance.Id}");
        }
        catch(Exception e)
        {
            HandleError(e, "Cannot save ticket, have all fields been populated?");
        }
    }

    /// <summary>
    /// Removes maintenance ticket from db context
    /// </summary>
    [RelayCommand]
    private async Task Delete()
    {
        try
        {
            _context.Delete(_maintenance);
            _context.Save();
            await Shell.Current.GoToAsync($"..?deleted={_maintenance.Id}");
        }
        catch(Exception e)
        {
            HandleError(e,"Cannot delete ticket");
        }  
    }

    /// <summary>
    /// Handles query strings provided when navigating to Maintenance page
    /// </summary>
    /// <param name="query"></param>
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("edit"))
        {
            _maintenance = _context.QueryById(int.Parse(query["edit"].ToString()));
            RefreshProperties();
        }
    }

    /// <summary>
    /// Checks if DueDate is before current date and updates boolean property accordingly
    /// </summary>
    public void IsOverdue() 
    {
        Reload();
        _maintenance.Overdue = DueDate.Date < DateTime.Now.Date;
        _context.Save();
    }

    /// <summary>
    /// Refreshes instance with DB Context
    /// </summary>
    public void Reload()
    {
        _context.Reload(_maintenance);
        RefreshProperties();
    }

    /// <summary>
    /// Write exception to Trace and set display property to show supplied message in application
    /// </summary>
    /// <param name="e"></param>
    /// <param name="message"></param>
    public void HandleError(Exception e, string message)
    {
        Trace.WriteLine(e.Message);
        DisplayError = message;
    }

    /// <summary>
    /// Triggers property changes events to update table displaying the ObservableCollection
    /// </summary>
    private void RefreshProperties()
    {
        OnPropertyChanged(nameof(Id));
        OnPropertyChanged(nameof(Overdue));
        OnPropertyChanged(nameof(DueDate));
        OnPropertyChanged(nameof(Priority));
        OnPropertyChanged(nameof(Description));
    }
}
