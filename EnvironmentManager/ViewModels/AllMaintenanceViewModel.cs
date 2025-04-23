using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.ViewModels;

/// <summary>
/// Code-behind AllMaintenancePage
/// </summary>
public class AllMaintenanceViewModel : ObservableObject, IQueryAttributable, IErrorHandling
{
    public ObservableCollection<MaintenanceViewModel> AllMaintenance { get; }

    public ICommand RefreshCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand NewTicketCommand { get; }

    private IMaintenanceDataStore _context;

    private string _displayError;
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

    public AllMaintenanceViewModel(IMaintenanceDataStore maintenanceDbContext)
    {
        _context = maintenanceDbContext;
        AllMaintenance = new ObservableCollection<MaintenanceViewModel>(_context.RetrieveAll().Select(n => new MaintenanceViewModel(_context, n)));
        SortCollection();
        RefreshCommand = new Command(CheckOverdue);
        EditCommand = new AsyncRelayCommand<MaintenanceViewModel>(EditMaintenance);
        NewTicketCommand = new AsyncRelayCommand(NewTicket);
        _displayError = "";
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
    /// Calls IsOverdue function in each maintenance object and reloads the instance to update the table.
    /// </summary>
    private void CheckOverdue()
    {
        foreach (MaintenanceViewModel maintenance in AllMaintenance) 
        {
            maintenance.IsOverdue();
            maintenance.Reload();
        }
    }

    /// <summary>
    /// Navigate to maintenance view supplying Maintenance ID to edit an existing entry
    /// </summary>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    private async Task EditMaintenance(MaintenanceViewModel viewModel)
    {
        try
        {
            if (viewModel != null)
            {
                await Shell.Current.GoToAsync($"{nameof(Views.MaintenancePage)}?edit={viewModel.Id}");
            }
        }
        catch(Exception e)
        {
            HandleError(e,"Failed to edit ticket");
        }
    }

    /// <summary>
    /// Navigate to maintenance view to create new ticket
    /// </summary>
    private async Task NewTicket()
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(Views.MaintenancePage));
        }
        catch(Exception e)
        {
            HandleError(e,"Cannot create new ticket");
        }
    }

    /// <summary>
    /// Handles query strings provided when routing to AllMaintenance page.
    /// </summary>
    /// <param name="query"></param>
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("deleted"))
        {
            string deletedId = query["deleted"].ToString();
            MaintenanceViewModel matchedEntry = AllMaintenance.Where((n) => n.Id == int.Parse(deletedId)).FirstOrDefault();

            if (matchedEntry != null)
            {
                AllMaintenance.Remove(matchedEntry);
            }       
        }
        else if (query.ContainsKey("saved"))
        {
            string savedId = query["saved"].ToString();
            MaintenanceViewModel matchedEntry = AllMaintenance.Where((n) => n.Id == int.Parse(savedId)).FirstOrDefault();

            if (matchedEntry != null)
            {
                matchedEntry.Reload();
            }
            else 
            {
                AllMaintenance.Insert(0, new MaintenanceViewModel(_context, _context.QueryById(int.Parse(savedId))));    
            }
            SortCollection();
            CheckOverdue();
        }
    }

    /// <summary>
    /// Repopulates ObservableCollection in Ascending order of Priority
    /// </summary>
    private void SortCollection() 
    {
        ObservableCollection<MaintenanceViewModel> sorted  = new ObservableCollection<MaintenanceViewModel>(AllMaintenance.OrderByDescending(i => i.Priority));
        foreach (MaintenanceViewModel viewModel in sorted) //adds each item back into collection, reverses order of sorted list, resulting in ascending order
        {
            AllMaintenance.Remove(viewModel);
            AllMaintenance.Insert(0, viewModel);
        }
    }

}
