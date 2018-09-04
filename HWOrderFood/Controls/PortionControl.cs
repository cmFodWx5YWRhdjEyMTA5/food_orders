using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using HWOrderFood.Converters;
using HWOrderFood.Enums;
using HWOrderFood.ModelLayers.Dish;
using HWOrderFood.ModelLayers.Portition.Models;
using HWOrderFood.ViewModels;
using Xamarin.Forms;

namespace HWOrderFood.Controls
{
    public class PortionControl : StackLayout
    {
        public PortionControl()
        {
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<PortionModel>), typeof(PortionControl), default(ObservableCollection<PortionModel>));

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
                ItemsSource.CollectionChanged += ItemsSource_CollectionChanged;
                DrawContent();
            }
        }

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);
            if (propertyName == nameof(ItemsSource) && ItemsSource != null)
            {
                ItemsSource.CollectionChanged -= ItemsSource_CollectionChanged;
            }
        }

        #endregion

        #region -- Private helpers --

        void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DrawContent();
        }

        private void DrawContent()
        {
            if(ItemsSource != null)
            {
                this.Children.Clear();
                foreach(var item in ItemsSource)
                {
                    this.Children.Add(GetView(item));
                }
            }
        }

        private ContentView GetView(PortionModel model)
        {
            var mainContent = new ContentView();
            var template = new StackLayout()
            {
                Orientation = StackOrientation.Vertical
            };

            var templateShowPortition = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.LightCoral,
                HeightRequest = 40,
                Margin = new Thickness(10)
            };

            var titleShowPortitionLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = "Порция",
            };

            var showPortitionImage = new Image()
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BindingContext = model,
                Margin = new Thickness(0, 0, 30, 0)
            };

            showPortitionImage.SetBinding(Image.SourceProperty, new Binding(nameof(model.IsVisible), BindingMode.OneWay, new IsAvailableUpdateToCheckBoxIconConverter(Constants.MoreImageSource, Constants.LessImageSource)));

            templateShowPortition.Children.Add(titleShowPortitionLabel);
            templateShowPortition.Children.Add(showPortitionImage);

            var contentPortitionView = new ContentView()
            {
                BindingContext = model,
            };
            var templatePotition = new StackLayout()
            {
                BindingContext = model
            };
            contentPortitionView.SetBinding(ContentView.IsVisibleProperty, new Binding(nameof(model.IsVisible)));

            var showPortitionClickableView = new ClickableContentView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = templateShowPortition,
                CommandParameter = model,
                BindingContext = model
            };

            showPortitionClickableView.SetBinding(ClickableContentView.CommandProperty, nameof(model.ChangeVisiblePortitionCommand));


            templatePotition.Children.Add(GetDishFream(model, "side_dish", "Гарнир", DishCategory.Garnish));
            templatePotition.Children.Add(GetDishFream(model, "meat", "Мясное", DishCategory.Meat));
            templatePotition.Children.Add(GetDishFream(model, "salad", "Салат", DishCategory.Salad));
            templatePotition.Children.Add(GetDishFream(model, "bread", "Хлеб", DishCategory.Bread));
            templatePotition.Children.Add(GetFrameIsDuplicate(model));


            template.Children.Add(showPortitionClickableView);
            contentPortitionView.Content = templatePotition;
            template.Children.Add(contentPortitionView);
            mainContent.Content = template;
            return mainContent;
        }

        private Frame GetDishFream(PortionModel model, string imageSource, string titleLable, DishCategory category)
        {
            var frame = new Frame()
            {
                Margin = new Thickness(10, 10),
            };

            var template = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
            };

            var header = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var titleImage = new Image()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Source = imageSource,
            };

            var titleLabel = new Label()
            {
                Text = titleLable,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            header.Children.Add(titleImage);
            header.Children.Add(titleLabel);

            var itemSource = new ObservableCollection<DishModel>();

            template.Children.Add(header);

            switch(category)
            {
                case DishCategory.Garnish:
                    itemSource = model.GarnishList;

                    template.Children.Add(GetDishMenuView(itemSource, model));

                    var sauceTemplete = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal
                    };

                    var sauceTitleLabel = new Label()
                    {
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Text = "Подлива"
                    };

                    var sauceSwith = new Switch()
                    {
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        BindingContext = model,
                        BackgroundColor = Color.LightGreen
                    };
                    sauceSwith.SetBinding(Switch.IsToggledProperty, new Binding(nameof(model.IsToggled)));

                    sauceTemplete.Children.Add(sauceTitleLabel);
                    sauceTemplete.Children.Add(sauceSwith);

                    template.Children.Add(sauceTemplete);
                    break;
                case DishCategory.Meat:
                    itemSource = model.MeatList;
                    template.Children.Add(GetDishMenuView(itemSource, model));

                    break;
                case DishCategory.Salad:
                    itemSource = model.SaladList;
                    template.Children.Add(GetDishMenuView(itemSource, model));

                    break;
                case DishCategory.Bread:
                    itemSource = model.BreadList;
                    template.Children.Add(GetDishMenuView(itemSource, model));

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //addDishStackLayout.ItemsSource = itemSource;
            frame.Content = template;
            return frame;
        }

        private StackLayout GetDishMenuView(ObservableCollection<DishModel> dishModels, PortionModel portition)
        {
            var template = new StackLayout()
            {
                Orientation = StackOrientation.Vertical
            };

            foreach(var item in dishModels)
            {
                template.Children.Add(GetDishItemView(new PortionWithCheckDish()
                {
                    Portion = portition,
                    Dish = item
                }));
            }
            return template;
        }

        private StackLayout GetDishItemView(PortionWithCheckDish model)
        {
            var template = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(0, 10)
            };

            var titleLabel = new Label()
            {
                Text = model.Dish.Name,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            var checkImage = new Image()
            {
                WidthRequest = 20,
                HeightRequest = 20,
                BindingContext = model.Dish
            };

            checkImage.SetBinding(Image.SourceProperty, new Binding(nameof(model.Dish.IsSelected), BindingMode.OneWay, new IsAvailableUpdateToCheckBoxIconConverter(Constants.CheckboxOnImageSource, Constants.CheckboxOffImageSource)));

            var checkClickableView = new ClickableContentView()
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                CommandParameter = model,
                BindingContext = model.Dish,
                Content = checkImage
            };

            checkClickableView.SetBinding(ClickableContentView.CommandProperty, nameof(model.Dish.ChangeCheckboxCommand));

            template.Children.Add(titleLabel);
            template.Children.Add(checkClickableView);

            return template;
        }

        public Frame GetFrameIsDuplicate(PortionModel model)
        {
            var frame = new Frame()
            {
                Margin = new Thickness(10, 10)
            };

            var stackTemplate = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            var checkImage = new Image()
            {
                WidthRequest = 20,
                HeightRequest = 20,
                BindingContext = model
            };

            checkImage.SetBinding(Image.SourceProperty, new Binding(nameof(model.IsDuplicate), BindingMode.OneWay, new IsAvailableUpdateToCheckBoxIconConverter(Constants.CheckboxOnImageSource, Constants.CheckboxOffImageSource)));

            var checkClickableView = new ClickableContentView()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                CommandParameter = model,
                BindingContext = model,
                Content = checkImage
            };

            checkClickableView.SetBinding(ClickableContentView.CommandProperty, nameof(model.ChangeCheckboxIsDuplicateCommand));

            var titleLable = new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = "Дублировать порцию",
            };

            stackTemplate.Children.Add(checkClickableView);
            stackTemplate.Children.Add(titleLable);

            frame.Content = stackTemplate;
            return frame;
        }


        #endregion

    }
}
