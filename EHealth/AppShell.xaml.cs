using System;
using Microsoft.Maui.Controls;

namespace EHealth
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