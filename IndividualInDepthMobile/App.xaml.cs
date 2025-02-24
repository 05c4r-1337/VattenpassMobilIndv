using IndividualInDepthMobile.MVVM.Views;

namespace IndividualInDepthMobile;

public partial class App : Application
{
    public App(LevelView levelView)
    {
        InitializeComponent();
        MainPage = levelView;
    }
}