namespace UniMobileProject.src.Views;

public partial class MainTabbedPage : TabbedPage
{
	public MainTabbedPage()
	{
		InitializeComponent();

        // ������������ ������� ������� UserProfilePage
        CurrentPage = Children.FirstOrDefault(page => page is UserProfilePage);
    }
}