using UniMobileProject.src.Views;

namespace UniMobileProject
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("rooms", typeof(RoomsPage));
        }
    }
}
