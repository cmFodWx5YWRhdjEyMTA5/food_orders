using System;
using Prism.Mvvm;
using Xamarin.Forms;

namespace HWOrderFood.Views
{
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}
