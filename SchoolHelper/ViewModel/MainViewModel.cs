using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.ApplicationModel.Communication;

namespace SchoolHelper;

public partial class MainViewModel : ObservableObject
{
    private readonly string _filePath;

    [ObservableProperty]
    private ObservableCollection<ShoppingList> _shoppingLists;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddSchoolItemCommand))]
    private ShoppingList _currentShoppingList;

    public MainViewModel()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "shopping_lists.json");
        LoadLists();
    }

    [RelayCommand]
    private async Task AddShoppingList()
    {
        string listName = await App.Current.MainPage.DisplayPromptAsync("Новый список", "Введите название списка (например, 'Канцелярия'):", "ОК", "Отмена");
        if (!string.IsNullOrWhiteSpace(listName))
        {
            var newList = new ShoppingList { Name = listName };
            ShoppingLists.Add(newList);
        }
    }

    [RelayCommand]
    private void SelectShoppingList(ShoppingList list)
    {
        CurrentShoppingList = list;
    }

    private bool CanAddSchoolItem() => CurrentShoppingList != null;

    [RelayCommand(CanExecute = nameof(CanAddSchoolItem))]
    private async Task AddSchoolItem()
    {
        string itemName = await App.Current.MainPage.DisplayPromptAsync("Новый предмет", "Введите название предмета (например, 'Ручка'):", "ОК", "Отмена");
        if (!string.IsNullOrWhiteSpace(itemName))
        {
            CurrentShoppingList.Items.Add(new Item { Name = itemName });
        }
    }

    [RelayCommand]
    private async Task RenameItem(Item itemToRename)
    {
        if (itemToRename == null) return;
        string newName = await App.Current.MainPage.DisplayPromptAsync("Переименовать", "Введите новое название:", "ОК", "Отмена", initialValue: itemToRename.Name);
        if (!string.IsNullOrWhiteSpace(newName))
        {
            itemToRename.Name = newName;
            SaveLists();
        }
    }

    [RelayCommand]
    private async Task DeleteItem(Item itemToDelete)
    {
        if (itemToDelete == null) return;
        bool confirmed = await App.Current.MainPage.DisplayAlert("Подтверждение", $"Вы уверены, что хотите удалить '{itemToDelete.Name}'?", "Да", "Нет");
        if (confirmed)
        {
            itemToDelete.IsPurchased = false;
            CurrentShoppingList?.Items.Remove(itemToDelete);
            SaveLists();
        }
    }

    [RelayCommand]
    private async Task RenameShoppingList(ShoppingList listToRename)
    {
        if (listToRename == null) return;

        string newName = await App.Current.MainPage.DisplayPromptAsync("Переименовать список", "Введите новое название:", "ОК", "Отмена", initialValue: listToRename.Name);

        if (!string.IsNullOrWhiteSpace(newName))
        {
            listToRename.Name = newName;
            SaveLists();
        }
    }

    [RelayCommand]
    private async Task DeleteShoppingList(ShoppingList listToDelete)
    {
        if (listToDelete == null) return;

        bool confirmed = await App.Current.MainPage.DisplayAlert("Подтверждение", $"Вы уверены, что хотите удалить список '{listToDelete.Name}'?", "Да", "Нет");

        if (confirmed)
        {
            if (CurrentShoppingList == listToDelete)
                CurrentShoppingList = null;

            ShoppingLists.Remove(listToDelete);
            SaveLists();
        }
    }

    [RelayCommand]
    private async Task ShareDataFile()
    {
        if (!File.Exists(_filePath))
        {
            await App.Current.MainPage.DisplayAlert("Ошибка", "Файл данных еще не создан. Добавьте хотя бы один список, чтобы он появился.", "OK");
            return;
        }

        try
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Файл данных SchoolHelper",
                File = new ShareFile(_filePath)
            });
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось поделиться файлом: {ex.Message}", "OK");
        }
    }

    private void LoadLists()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                var savedLists = JsonSerializer.Deserialize<List<ShoppingList>>(json);
                ShoppingLists = new ObservableCollection<ShoppingList>(savedLists);
                foreach (var list in ShoppingLists)
                {
                    list.ReattachItemEventHandlers();
                    list.PropertyChanged += ShoppingList_PropertyChanged;
                }
            }
            else
            {
                ShoppingLists = new ObservableCollection<ShoppingList>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading lists: {ex.Message}");
            ShoppingLists = new ObservableCollection<ShoppingList>();
        }

        ShoppingLists.CollectionChanged += OnShoppingListsChanged;
    }

    private void OnShoppingListsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (ShoppingList newList in e.NewItems)
            {
                newList.PropertyChanged += ShoppingList_PropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (ShoppingList oldList in e.OldItems)
            {
                oldList.PropertyChanged -= ShoppingList_PropertyChanged;
            }
        }
        SaveLists();
    }

    private void ShoppingList_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        SaveLists();
    }

    private void SaveLists()
    {
        try
        {
            string json = JsonSerializer.Serialize(ShoppingLists);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving lists: {ex.Message}");
        }
    }
}