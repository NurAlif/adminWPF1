using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace AdminClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public class CustomLoginCredentials : ServiceClientCredentials
        {
            public override void InitializeServiceClient<T>(ServiceClient<T> client){}
            public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request == null) throw new ArgumentNullException("request");
                await base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        public class CraftDataAdapter
        {
            public Craft Craft { set; get; }
            public List<Material> Materials { set; get; }
            public List<Category> Categories { set; get; }

            public CraftDataAdapter(List<Material> materials, List<Category> categories, Craft craft)
            {
                Craft = craft;
                Materials = materials;
                Categories = categories;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            CrocodileHandycraft crocodileHandycraft = new CrocodileHandycraft(new Uri("http://localhost:3000"), new CustomLoginCredentials());


            Task<HttpOperationResponse> resp = crocodileHandycraft.CraftsController.FindAllWithHttpMessagesAsync();
            resp.Wait();
            string data = resp.Result.Response.Content.AsString();
            List<Craft> crafts = JsonConvert.DeserializeObject<List<Craft>>(data);

            Task<HttpOperationResponse> respPhotos = crocodileHandycraft.PhotosController.FindAllWithHttpMessagesAsync();
            respPhotos.Wait();
            string dataPhotos = respPhotos.Result.Response.Content.AsString();
            List<Photo> photos = JsonConvert.DeserializeObject<List<Photo>>(dataPhotos);
            LVPhotosAll.ItemsSource = photos;

            Task<HttpOperationResponse> respCategories = crocodileHandycraft.CategoriesController.FindAllWithHttpMessagesAsync();
            respCategories.Wait();
            string dataCategories = respCategories.Result.Response.Content.AsString();
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(dataCategories);

            Task<HttpOperationResponse> respMaterials = crocodileHandycraft.MaterialsController.FindAllWithHttpMessagesAsync();
            respMaterials.Wait();
            string dataMaterials = respMaterials.Result.Response.Content.AsString();
            List<Material> materials = JsonConvert.DeserializeObject<List<Material>>(dataMaterials);

            List<CraftDataAdapter> craftDatas = new List<CraftDataAdapter>();
            crafts.ForEach(c => {
                craftDatas.Add(new CraftDataAdapter(materials, categories, c));
            });

            LVCrafts.ItemsSource = craftDatas;
        }
        public class User
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public string Mail { get; set; }
        }
    }
}
