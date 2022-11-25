using Realms.Sync;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TodoMaui.Models;
using CommunityToolkit.Mvvm.Input;
using TodoMaui.Helpers;

namespace TodoMaui.ViewModels
{


    public partial class DashboardVM : BaseViewModel
    {
        private Realm realm;
        private PartitionSyncConfiguration config;
        public DashboardVM()
        {
            todoList = new ObservableCollection<Todo>();
            EmptyText = "No todos here. Add new Todo to get started 💪";


        }

        [ObservableProperty]
        ObservableCollection<Todo> todoList;

        [ObservableProperty]
        string emptyText;

        [ObservableProperty]
        string todoEntryText;

        [ObservableProperty]
        bool isRefreshing;

        public async Task InitialiseRealm()
        {
            config = new PartitionSyncConfiguration($"{App.RealmApp.CurrentUser.Id}", App.RealmApp.CurrentUser);
            realm = Realm.GetInstance(config);

            GetTodos();
            if (TodoList.Count == 0)
            {
                EmptyText = "Loading projects..";
                await Task.Delay(2000);
                GetTodos();
                EmptyText = "No todos here. Add new Todo to get started 💪";
            }

        }

        [RelayCommand]
        public async void GetTodos()
        {
            IsRefreshing = true;
            IsBusy = true;

            try
            {
                var tlist = realm.All<Todo>().ToList().Reverse<Todo>().OrderBy(t => t.Completed);
                TodoList = new ObservableCollection<Todo>(tlist);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayPromptAsync("Error", ex.Message);
            }

            IsRefreshing = false;
            IsBusy = false;
        }

        [RelayCommand]
        public async void EditTodo(Todo td)
        {
            string newString = await App.Current.MainPage.DisplayPromptAsync("Edit", td.Name);

            if (newString is null || string.IsNullOrWhiteSpace(newString.ToString()))
                return;
            try
            {

                realm.Write(() =>
                {
                    var foundTodo = realm.Find<Todo>(td.Id);

                    foundTodo.Name = GeneralHelper.UppercaseFirst(newString.ToString());
                });

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayPromptAsync("Error", ex.Message);

            }
        }

        [RelayCommand]
        public async void CheckTodo(Todo todo)
        {
            IsBusy = true;

            try
            {
                realm.Write(() =>
                {
                    var foundTodo = realm.Find<Todo>(todo.Id);

                    foundTodo.Completed = !foundTodo.Completed;
                });

                await Task.Delay(2);
                var sortedlist = TodoList.ToList().OrderBy(t => t.Completed);
                TodoList = new ObservableCollection<Todo>(sortedlist);



            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayPromptAsync("Error", ex.Message);

            }
            IsBusy = false;

        }


        [RelayCommand]
        async Task SignOut()
        {
            IsBusy = true;
            try
            {
                var currentuserId = App.RealmApp.CurrentUser.Id;

                await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);

                var noMoreCurrentUser = App.RealmApp.AllUsers.FirstOrDefault(u => u.Id == currentuserId);

                await Shell.Current.GoToAsync("///Login");



            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayPromptAsync("Error", ex.Message);

            }
            IsBusy = false;


        }


        [RelayCommand]
        async Task AddTodo()
        {
            if (string.IsNullOrWhiteSpace(TodoEntryText))
                return;
            IsBusy = true;
            try
            {
                var todo =
                    new Todo
                    {
                        Name = GeneralHelper.UppercaseFirst(TodoEntryText),
                        Partition = App.RealmApp.CurrentUser.Id,
                        Owner = App.RealmApp.CurrentUser.Profile.Email
                    };
                realm.Write(() =>
                {
                    realm.Add(todo);
                });

                TodoEntryText = "";
                GetTodos();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayPromptAsync("Error", ex.Message);

            }
            IsBusy = false;
        }

        [RelayCommand]
        async Task DeleteTask(Todo todo)
        {
            IsBusy = true;
            try
            {
                realm.Write(() =>
                {
                    realm.Remove(todo);
                });

                todoList.Remove(todo);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayPromptAsync("Error", ex.Message);
            }
            IsBusy = false;
        }

    }
}
