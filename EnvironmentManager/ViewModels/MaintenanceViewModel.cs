using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using System.Diagnostics;
using EnvironmentManager.Interfaces;


namespace EnvironmentManager.ViewModels;

public partial class MaintenanceViewModel : ObservableObject, IQueryAttributable, IErrorHandling
{
    private Maintenance _maintenance;

    private MaintenanceDbContext _context;

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

    public MaintenanceViewModel(MaintenanceDbContext maintenanceDbContext)
    {
        _context = maintenanceDbContext;
        CurrentDate = DateTime.Now;
        _maintenance = new Maintenance();
        _maintenance.DueDate = CurrentDate;
        _displayError = "";
    }
    public MaintenanceViewModel(MaintenanceDbContext maintenanceDbContext, Maintenance maintenance)
    {
        _maintenance = maintenance;
        _context = maintenanceDbContext;
        CurrentDate = DateTime.Now;
        _displayError = "";
    }

    //Adds new or updated maintenance ticket to db context
    [RelayCommand]
    private async Task Save()
    {
        try 
        {
            _context.Maintenance.Update(_maintenance);
            _context.SaveChanges();
            await Shell.Current.GoToAsync($"..?saved={_maintenance.Id}");
        }
        catch(Exception e)
        {
            HandleError(e, "Cannot save ticket, have all fields been populated?");
        }
    }

    //Removes maintenance ticket from db context
    [RelayCommand]
    private async Task Delete()
    {
        try
        {
            _context.Remove(_maintenance);
            _context.SaveChanges();
            await Shell.Current.GoToAsync($"..?deleted={_maintenance.Id}");
        }
        catch(Exception e)
        {
            HandleError(e,"Cannot delete ticket");
        }  
    }

    //Handles query strings provided when navigating to Maintenance page
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("edit"))
        {
            _maintenance = _context.Maintenance.Single(n => n.Id == int.Parse(query["edit"].ToString()));
            RefreshProperties();
        }
    }

    //Checks if DueDate is before current date and updates boolean property accordingly
    public void IsOverdue() 
    {
        Reload();
        _maintenance.Overdue = DueDate.Date < DateTime.Now.Date;
        _context.SaveChanges();
    }

    //Refreshes instance with DB Context
    public void Reload()
    {
        _context.Entry(_maintenance).Reload();
        RefreshProperties();
    }

    //Write exception to Trace and set display property to show supplied message in application
    public void HandleError(Exception e, string message)
    {
        Trace.WriteLine(e.Message);
        DisplayError = message;
    }

    //Triggers property changes events to update table displaying the ObservableCollection
    private void RefreshProperties()
    {
        OnPropertyChanged(nameof(Id));
        OnPropertyChanged(nameof(Overdue));
        OnPropertyChanged(nameof(DueDate));
        OnPropertyChanged(nameof(Priority));
        OnPropertyChanged(nameof(Description));
    }
}
