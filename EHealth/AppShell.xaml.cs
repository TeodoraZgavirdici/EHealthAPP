using System;
using Microsoft.Maui.Controls;
using EHealthApp;
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