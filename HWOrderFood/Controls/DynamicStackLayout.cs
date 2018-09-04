using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using HWOrderFood.Converters;
using HWOrderFood.Enums;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.ViewModels;
using Xamarin.Forms;

namespace HWOrderFood.Controls
{
    public class DynamicStackLayout : StackLayout
    {
        public DynamicStackLayout()
        {

        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<DishModel>), typeof(DynamicStackLayout), default(ObservableCollection<DishModel>));
        public ObservableCollection<DishModel> ItemsSource
        {
            get { return (ObservableCollection<DishModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(ItemsSource) && ItemsSource != null)
            {
                ItemsSource.CollectionChanged += ItemSource_CollectionChanged;
                DrawContent();
            }
        }

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);
            if (propertyName == nameof(ItemsSource) && ItemsSource != null)
            {
                ItemsSource.CollectionChanged -= ItemSource_CollectionChanged;
            }
        }

        #endregion

        #region -- Private helpers --

        void ItemSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DrawContent();
        }

        private void DrawContent()
        {
            if (ItemsSource != null)
            {
                this.Children.Clear();
                foreach (var item in ItemsSource)
                {
                    this.Children.Add(GetView(item));
                }
            }
        }

        private ContentView GetView(DishModel model)
        {
            var mainContent = new ContentView();
            var template = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(0, 10),
                //BackgroundColor = Color.LimeGreen
            };

            var checkImage = new Image()
            {
                WidthRequest = 20,
                HeightRequest = 20,
                BindingContext = model,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            checkImage.SetBinding(Image.SourceProperty, new Binding(nameof(model.IsSelected), BindingMode.OneWay, new IsAvailableUpdateToCheckBoxIconConverter(Constants.CheckboxOnImageSource, Constants.CheckboxOffImageSource)));

            var checkClickableView = new ClickableContentView()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                CommandParameter = model,
                BindingContext = model,
                Content = checkImage,
                WidthRequest = 30,
                //BackgroundColor = Color.Red,
                Margin = 0
            };
            checkClickableView.SetBinding(ClickableContentView.CommandProperty, nameof(model.ChangeCheckboxCommand));

            var titleLabel = new Label()
            {
                Margin = 0,
                Text = model.Name,
                //VerticalTextAlignment = TextAlignment.Start,
                //HorizontalTextAlignment = TextAlignment.Start,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                //BackgroundColor = Color.Blue
            };

            var deleteImage = new Image()
            {
                Source = "delete",
                WidthRequest = 20,
                HeightRequest = 20,
            };

            var deleteClickableView = new ClickableContentView()
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                CommandParameter = model.Id,
                Content = deleteImage,
                //BackgroundColor = Color.Red
            };

            switch(model.CategoryType)
            {
                case DishCategory.Garnish:
                    deleteClickableView.SetBinding(ClickableContentView.CommandProperty, nameof(DishViewModel.DeleteGarnishDishCommand));
                    break;
                case DishCategory.Meat:
                    deleteClickableView.SetBinding(ClickableContentView.CommandProperty, nameof(DishViewModel.DeleteMeatDishCommand));
                    break;
                case DishCategory.Salad:
                    deleteClickableView.SetBinding(ClickableContentView.CommandProperty, nameof(DishViewModel.DeleteSaladDishCommand));
                    break;
                case DishCategory.Bread:
                    deleteClickableView.SetBinding(ClickableContentView.CommandProperty, nameof(DishViewModel.DeleteBreadDishCommand));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            template.Children.Add(checkClickableView);
            template.Children.Add(titleLabel);
            template.Children.Add(deleteClickableView);

            mainContent.Content = template;

            return mainContent;
        }

        #endregion
    }
}
