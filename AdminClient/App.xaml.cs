using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AdminClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>


    public partial class App : Application
    {
        public class CustomLoginCredentials : ServiceClientCredentials
        {
            public override void InitializeServiceClient<T>(ServiceClient<T> client) { }
            public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request == null) throw new ArgumentNullException("request");
                await base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        public CrocodileHandycraft crocodileHandycraft;


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            crocodileHandycraft = new CrocodileHandycraft(new Uri("http://localhost:3000"), new CustomLoginCredentials());

            

            MainWindow wnd = new MainWindow();
            wnd.Title = "Something else";
            wnd.Show();
        }
    }
}
