namespace UniMobileProject.src.Views;

public partial class MainTabbedPage : TabbedPage
{
	public MainTabbedPage()
	{
		InitializeComponent();

        // Встановлюємо активну сторінку UserProfilePage
        CurrentPage = Children.FirstOrDefault(page => page is UserProfilePage);
    }
}