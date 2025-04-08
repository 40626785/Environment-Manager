using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Data;
using EnvironmentManager.Models;


namespace EnvironmentManager.ViewModels;

public partial class MaintenanceViewModel : ObservableObject, IQueryAttributable
{
    private Maintenance _maintenance;

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
    }
    [RelayCommand]
    private async Task Save()
    {
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

    //Checks if DueDate is before current date and updates boolean property accordingly
    public void isOverdue() {
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
