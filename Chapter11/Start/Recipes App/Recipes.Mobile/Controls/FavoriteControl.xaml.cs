
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Recipes.Mobile.Controls;

public partial class FavoriteControl : ContentView
{
    public bool IsInteractive { get; private set; }

    public static readonly BindableProperty ToggledCommandProperty =
        BindableProperty.Create(
            nameof(ToggledCommand),
            typeof(ICommand),
            typeof(FavoriteControl),
            propertyChanged: ToggledCommandChanged);

    private static void ToggledCommandChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as FavoriteControl;
        if (oldValue is ICommand oldCommand)
        {
            oldCommand.CanExecuteChanged -= control.CanExecuteChanged;
        }
        if (newValue is ICommand newCommand)
        {
            newCommand.CanExecuteChanged += control.CanExecuteChanged;
        }
        control.UpdateIsInteractive();
    }

    private void CanExecuteChanged(object sender, EventArgs e) => UpdateIsInteractive();

    public ICommand ToggledCommand
    {
        get => (ICommand)GetValue(ToggledCommandProperty);
        set => SetValue(ToggledCommandProperty, value);
    }

    public static readonly BindableProperty IsFavoriteProperty =
		BindableProperty.Create(nameof(IsFavorite),
            typeof(bool),
            typeof(FavoriteControl),
            defaultBindingMode:
            BindingMode.TwoWay,
            propertyChanged: OnIsFavoriteChanged);

    private static void OnIsFavoriteChanged(BindableObject bindable, object oldValue, object newValue)
    {
        (bindable as FavoriteControl).AnimateChange();
    }

    private async Task AnimateChange()
    {
        await icon.ScaleTo(1.5, 100);
        await icon.ScaleTo(1, 1000);
    }

    public bool IsFavorite
    {
        get { return (bool)GetValue(IsFavoriteProperty); }
        set { SetValue(IsFavoriteProperty, value); }
    }

    private void UpdateIsInteractive()
    {
        IsInteractive = IsEnabled && (ToggledCommand?.CanExecute(IsFavorite) ?? false);
        OnPropertyChanged(nameof(IsInteractive));
    }

    public FavoriteControl()
	{
		InitializeComponent();
	}

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsEnabled))
        {
            UpdateIsInteractive();
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (IsInteractive)
        {
            IsFavorite = !IsFavorite;
            ToggledCommand?.Execute(IsFavorite);
        }
    }
}