using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EnvironmentManager.ViewModels;

public class AllMaintenanceViewModel : IQueryAttributable
{
    public ObservableCollection<MaintenanceViewModel> AllMaintenance { get; }
    public ICommand RefreshCommand { get; }

    public ICommand EditCommand { get; }

    public ICommand NewTicketCommand { get; }

    private MaintenanceDbContext _context;
    public AllMaintenanceViewModel(MaintenanceDbContext maintenanceDbContext)
    {
        _context = maintenanceDbContext;
        AllMaintenance = new ObservableCollection<MaintenanceViewModel>(_context.Maintenance.ToList().Select(n => new MaintenanceViewModel(_context, n)));
        SortCollection();
        RefreshCommand = new Command(CheckOverdue);
        EditCommand = new AsyncRelayCommand<MaintenanceViewModel>(EditMaintenance);
        NewTicketCommand = new AsyncRelayCommand(NewTicket);
    }
    
    //Calls IsOverdue function in each maintenance object and reloads the instance to update the table.
    private void CheckOverdue()
    {
        foreach (MaintenanceViewModel maintenance in AllMaintenance) 
        {
            maintenance.IsOverdue();
            maintenance.Reload();
        }
    }

    //Navigate to maintenance view supplying Maintenance ID to edit an existing entry
    private async Task EditMaintenance(MaintenanceViewModel viewModel)
    {
        if (viewModel != null)
        {
            await Shell.Current.GoToAsync($"{nameof(Views.MaintenancePage)}?edit={viewModel.Id}");
        }
    }

    //Navigate to maintenance view to create new ticket
    private async Task NewTicket()
    {
        await Shell.Current.GoToAsync(nameof(Views.MaintenancePage));
    }

    //Handles query strings provided when routing to AllMaintenance page.
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
            else {
                AllMaintenance.Insert(0, new MaintenanceViewModel(_context, _context.Maintenance.Single(n => n.Id == int.Parse(savedId))));    
            }
            SortCollection();
            CheckOverdue();
        }
    }

    //Repopulates ObservableCollection in Ascending order of Priority
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
