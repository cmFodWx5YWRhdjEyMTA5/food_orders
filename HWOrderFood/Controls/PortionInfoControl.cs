using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using HWOrderFood.Converters;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.ModelLayers.OrderDish.Models;
using HWOrderFood.ModelLayers.Portition.Models;
using HWOrderFood.ViewModels;
using Xamarin.Forms;

namespace HWOrderFood.Controls
{
    public class PortionInfoControl : StackLayout
    {
        public PortionInfoControl()
        {

        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<PortionModel>), typeof(PortionInfoControl), default(ObservableCollection<PortionModel>));
        public ObservableCollection<PortionModel> ItemsSource
        {
            get { return (ObservableCollection<PortionModel>)GetValue(ItemsSourceProperty); }
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
                DrawPortitionContent();
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
            DrawPortitionContent();
        }

        private void DrawPortitionContent()
        {
            if (ItemsSource != null)
            {
                this.Children.Clear();
                foreach (var item in ItemsSource)
                {
                    this.Children.Add(GetPortitionView(item));
                }
            }
        }

        private ContentView GetPortitionView(PortionModel model)
        {
            var mainContant = new ContentView();
            var template = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Margin = new Thickness(0, 10)
            };

            var titleTemplate = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            var titleLabel = new Label()
            {
                Text = "Порция:",
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            var checkImage = new Image()
            {
                WidthRequest = 20,
                HeightRequest = 20,
                Source = "delete",
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BindingContext = model
            };

            var checkClickableView = new ClickableContentView()
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                CommandParameter = model,
                BindingContext = model,
                Content = checkImage
            };

            checkClickableView.SetBinding(ClickableContentView.CommandProperty, nameof(model.DeletePortionCommand));

            titleTemplate.Children.Add(titleLabel);
            titleTemplate.Children.Add(checkClickableView);

            var bodyTemplate = new StackLayout()
            {
                Orientation = StackOrientation.Vertical
            };


            var garnishLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BindingContext = model.PortionInfo,
            };

            garnishLabel.SetBinding(Label.TextProperty, new Binding(nameof(model.PortionInfo.GarnishName)));
            garnishLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(model.PortionInfo.IsGarnishVisible)));

            var meatLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BindingContext = model.PortionInfo
            };
            meatLabel.SetBinding(Label.TextProperty, new Binding(nameof(model.PortionInfo.MeatName)));
            meatLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(model.PortionInfo.IsMeatVisible)));

            var firstSaladLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BindingContext = model.PortionInfo
            };
            firstSaladLabel.SetBinding(Label.TextProperty, new Binding(nameof(model.PortionInfo.FirstSaladName)));
            firstSaladLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(model.PortionInfo.IsFirstSaladVisible)));

            var secondSaladLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BindingContext = model.PortionInfo
            };
            secondSaladLabel.SetBinding(Label.TextProperty, new Binding(nameof(model.PortionInfo.SecondSaladName)));
            secondSaladLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(model.PortionInfo.IsSecondSaladVisible)));

            var breadLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BindingContext = model.PortionInfo
            };
            breadLabel.SetBinding(Label.TextProperty, new Binding(nameof(model.PortionInfo.BreadName)));
            breadLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(model.PortionInfo.IsBreadVisible)));

            var sauseLabel = new Label() 
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BindingContext = model,
                Text = "Подлива"
            };

            sauseLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(model.IsToggled)));

            var sumTempalte = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            var sumLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = "Сумма:",
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Bold
            };

            var dublePriceLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                Text = "2x",
                BindingContext = model
            };
            dublePriceLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(model.IsDuplicate)));

            var priceLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.FromHex("#47cfa5"),
                FontAttributes = FontAttributes.Bold,
                BindingContext = model
            };

            priceLabel.SetBinding(Label.TextProperty, new Binding(nameof(model.PriceStr)));

            var commentEntry = new Entry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Placeholder = "Введите свой коментарий",
                BindingContext = model.Comment,
                FontSize = 14
            };
            commentEntry.SetBinding(Entry.TextProperty, new Binding(nameof(model.Comment)));

            sumTempalte.Children.Add(sumLabel);
            sumTempalte.Children.Add(dublePriceLabel);
            sumTempalte.Children.Add(priceLabel);

            bodyTemplate.Children.Add(garnishLabel);
            bodyTemplate.Children.Add(meatLabel);
            bodyTemplate.Children.Add(firstSaladLabel);
            bodyTemplate.Children.Add(secondSaladLabel);
            bodyTemplate.Children.Add(breadLabel);
            bodyTemplate.Children.Add(sauseLabel);
            bodyTemplate.Children.Add(sumTempalte);
            bodyTemplate.Children.Add(commentEntry);

            template.Children.Add(titleTemplate);
            template.Children.Add(bodyTemplate);

            mainContant.Content = template;
            return mainContant;
        }

        #endregion
    }
}
