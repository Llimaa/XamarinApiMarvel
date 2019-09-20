using MarvelHerois.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MarvelHerois.Services
{
    public class NavigationService
    {
        static Lazy<NavigationService> LazyNavi = new Lazy<NavigationService>(() => new NavigationService());
        public static NavigationService Current => LazyNavi.Value;

        private Page GetViewModelLocator<TViewModel>(params object[] args) where TViewModel : BaseViewModel
        {
            var viewModelType = typeof(TViewModel);
            var viewModelTypeName = viewModelType.Name;
            var viewModelWordLenght = "ViewModel".Length;

            var namespaceName = typeof(BaseViewModel).AssemblyQualifiedName.Split('.')[0];

            var viewTypeName = $"{namespaceName}.{viewModelTypeName.Substring(0, viewModelTypeName.Length - viewModelWordLenght)}Page";
            var viewType = Type.GetType(viewTypeName);

            var page = Activator.CreateInstance(viewType) as Page;

            var viewModel = Activator.CreateInstance(viewModelType, args);
            if (page != null)
            {
                page.BindingContext = viewModel;
            }
            return page;
        }

        public async Task PushAsync<TvViewModel>(bool modal = false, params object[] args) where TvViewModel : BaseViewModel
        {
            var page = GetViewModelLocator<TvViewModel>(args);

            if (modal)
                await Application.Current.MainPage.Navigation.PushModalAsync(page);

            else
                await Application.Current.MainPage.Navigation.PushAsync(page);

            await (page.BindingContext as BaseViewModel).LoadAsync(args);
        }

        public async Task PopAsync() => await Application.Current.MainPage.Navigation.PopAsync();

        public async Task PopToRootAsync() => await Application.Current.MainPage.Navigation.PopToRootAsync();
    }
}
