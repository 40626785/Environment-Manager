using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;

namespace EnvironmentManager.ViewModels;

public class AllMaintenanceViewModel : IQueryAttributable
{
    public ObservableCollection<ViewModels.MaintenanceViewModel> AllMaintenance { get; }
    public ICommand RefreshCommand { get; }

    public ICommand EditCommand { get; }

    public ICommand NewTicketCommand { get; }

    private MaintenanceDbContext _context;
    public AllMaintenanceViewModel(MaintenanceDbContext maintenanceDbContext)
    {
        _context = maintenanceDbContext;
        AllMaintenance = new ObservableCollection<MaintenanceViewModel>(_context.Maintenance.ToList().Select(n => new MaintenanceViewModel(_context, n)));
        sortCollection();
        RefreshCommand = new Command(checkOverdue);
        EditCommand = new AsyncRelayCommand<MaintenanceViewModel>(EditMaintenance);
        NewTicketCommand = new AsyncRelayCommand(NewTicket);
    }
    
    private void checkOverdue()
    {
        foreach (MaintenanceViewModel maintenance in AllMaintenance) 
        {
            maintenance.isOverdue();
            maintenance.Reload();
        }
    }

    private async Task EditMaintenance(MaintenanceViewModel viewModel)
    {
        if (viewModel != null)
        {
            await Shell.Current.GoToAsync($"{nameof(Views.MaintenancePage)}?edit={viewModel.Id}");
        }
    }

    private async Task NewTicket()
    {
        await Shell.Current.GoToAsync(nameof(Views.MaintenancePage));
    }

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
            sortCollection();
            checkOverdue();
        }
    }

    private void sortCollection() 
    {
        ObservableCollection<MaintenanceViewModel> sorted  = new ObservableCollection<MaintenanceViewModel>(AllMaintenance.OrderByDescending(i => i.Priority));
        foreach (MaintenanceViewModel viewModel in sorted)
        {
            AllMaintenance.Remove(viewModel);
            AllMaintenance.Insert(0, viewModel);
        }
    }

}
