namespace EHealthApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("signup", typeof(SignupPage));
        }
    }
}