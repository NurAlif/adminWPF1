using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AdminClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



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

        bool loaded = false;

        private Craft selectedCraft;
        private Craft newCraft = new Craft();
        private Photo selectedPhotoAll;
        private Photo selectedPhotoCurrent;

        private List<Craft> crafts;
        private List<Photo> photos;
        private List<Photo> photosNew = new List<Photo>();
        private List<Photo> photosCurrent;
        private List<Category> categories;
        private List<Material> materials;

        private CrocodileHandycraft apiClient;

        public MainWindow()
        {
            InitializeComponent();

            apiClient = ((App)Application.Current).crocodileHandycraft;
            
            LoadCrafts();
        }

        private void LoadAllPhotos()
        {
            Task<HttpOperationResponse> respPhotos = apiClient.PhotosController.FindAllWithHttpMessagesAsync();
            respPhotos.Wait();
            string dataPhotos = respPhotos.Result.Response.Content.AsString();
            photos = JsonConvert.DeserializeObject<List<Photo>>(dataPhotos);
            LVPhotosAll.ItemsSource = photos;
        }

        private void LoadCrafts()
        {
            Task<HttpOperationResponse> respCategories = apiClient.CategoriesController.FindAllWithHttpMessagesAsync();
            respCategories.Wait();
            string dataCategories = respCategories.Result.Response.Content.AsString();
            categories = JsonConvert.DeserializeObject<List<Category>>(dataCategories);

            Task<HttpOperationResponse> respMaterials = apiClient.MaterialsController.FindAllWithHttpMessagesAsync();
            respMaterials.Wait();
            string dataMaterials = respMaterials.Result.Response.Content.AsString();
            materials = JsonConvert.DeserializeObject<List<Material>>(dataMaterials);

            CBCategory.ItemsSource = categories;
            CBMaterial.ItemsSource = materials;

            Task<HttpOperationResponse> resp = apiClient.CraftsController.FindAllWithHttpMessagesAsync();
            resp.Wait();
            string data = resp.Result.Response.Content.AsString();
            crafts = JsonConvert.DeserializeObject<List<Craft>>(data);

            List<CraftDataAdapter> craftDatas = new List<CraftDataAdapter>();
            crafts.ForEach(c =>
            {
                craftDatas.Add(new CraftDataAdapter(materials, categories, c));
            });

            LVCrafts.ItemsSource = craftDatas;

            LoadAllPhotos();
        }

        private void LoadCurrentPhotos(long id)
        {
            if (id < 0)
            {
                LVPhotosCurrent.ItemsSource = photosNew;
                LVPhotosCurrent.ItemsSource = newCraft.Photos;
                return;
            }

            Task<HttpOperationResponse> respUpdate = apiClient.CraftsController.FindOneWithHttpMessagesAsync(id.ToString());
            respUpdate.Wait();
            Craft craft = JsonConvert.DeserializeObject<Craft>(respUpdate.Result.Response.Content.AsString());
            crafts[LVCrafts.SelectedIndex] = craft;

            LVPhotosCurrent.ItemsSource = craft.Photos;
        }

        public void OnSelectedCraftChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LVCrafts.SelectedIndex < 0) return;
            selectedCraft = crafts[LVCrafts.SelectedIndex];

            LVPhotosCurrent.ItemsSource = selectedCraft.Photos;
            photosCurrent = selectedCraft.Photos;
        }

        public void OnSelectedPhotoAll(object sender, SelectionChangedEventArgs e)
        {
            if (LVPhotosAll.SelectedIndex < 0) return;
            selectedPhotoAll = photos[LVPhotosAll.SelectedIndex];
        }

        public void OnSelectedPhotoCurrent(object sender, SelectionChangedEventArgs e)
        {
            if (LVPhotosCurrent.SelectedIndex < 0) return;
            selectedPhotoCurrent = photosCurrent[LVPhotosCurrent.SelectedIndex];
        }

        private void OnBtNewPhoto(object sender, RoutedEventArgs e)
        {
            NewPhoto win2 = new NewPhoto();
            win2.Closed += OnUploadClosed;
            win2.Show();
        }

        public void OnUploadClosed(object sender, EventArgs e)
        {
            LoadAllPhotos();
        }

        public void OnAddPhoto(object sender, RoutedEventArgs e)
        {
            if (selectedPhotoAll == null) return;
            if (tab.SelectedIndex == 1)
            {
                newCraft.Photos.Add(selectedPhotoAll);
                LoadCurrentPhotos(-1);
                return;
            }

            if (selectedCraft == null) return;

            Models.AddPhotoCraftDto addPhotoCraftDto = new Models.AddPhotoCraftDto();
            addPhotoCraftDto.PhotoId = selectedPhotoAll.Id;
            addPhotoCraftDto.CraftId = selectedCraft.Id;
            Task<HttpOperationResponse> resp = apiClient.CraftsController.AddphotoWithHttpMessagesAsync(addPhotoCraftDto);
            resp.Wait();

            LoadCurrentPhotos(selectedCraft.Id);
        }

        public void OnRemovePhoto(object sender, RoutedEventArgs e)
        {
            if (selectedPhotoCurrent == null) return;

            if (tab.SelectedIndex == 1)
            {
                newCraft.Photos.Remove(selectedPhotoCurrent);
                LoadCurrentPhotos(-1);
                return;
            }

            if (selectedCraft == null) return;

            Models.AddPhotoCraftDto addPhotoCraftDto = new Models.AddPhotoCraftDto();
            addPhotoCraftDto.PhotoId = selectedPhotoCurrent.Id;
            addPhotoCraftDto.CraftId = selectedCraft.Id;
            Task<HttpOperationResponse> resp = apiClient.CraftsController.RemovephotoWithHttpMessagesAsync(addPhotoCraftDto);
            resp.Wait();

            LoadCurrentPhotos(selectedCraft.Id);
        }

        public void OnDeletePhoto(object sender, RoutedEventArgs e)
        {
            if (selectedPhotoAll == null) return;

            Task<HttpOperationResponse> resp = apiClient.PhotosController.RemoveWithHttpMessagesAsync(selectedPhotoAll.Id);
            resp.Wait();

            LoadAllPhotos();
        }

        public void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if(e.Source is not TabControl) return;

            if (tab.SelectedIndex == 0)
            {
                if (!loaded)
                {
                    loaded = true;
                    return;
                }
                LoadCrafts();
            }
            if (tab.SelectedIndex == 1)
            {
                newCraft.Photos = new List<Photo>();
                LoadCurrentPhotos(-1);
            }
        }

        public void OnNewCraft(object sender, RoutedEventArgs e)
        {
            if (newCraft == null) return;

            Models.CreateCraftDto craftDto = new Models.CreateCraftDto();

            craftDto.Name = TBName.Text;
            craftDto.Description = TBDescription.Text;
            craftDto.Category = categories[CBCategory.SelectedIndex].Id;
            craftDto.Material = materials[CBMaterial.SelectedIndex].Id;
            craftDto.Thumbnail = newCraft.Thumbnail.Id;
            craftDto.Author = 4;

            IList<double?> photos = new List<double?>();
            newCraft.Photos.ForEach(photo => photos.Add(photo.Id));

            craftDto.Photos = photos;

            Task<HttpOperationResponse> resp = apiClient.CraftsController.CreateWithHttpMessagesAsync(craftDto);
            resp.Wait();

            tab.SelectedIndex = 0;
        }

        public void OnNewCraftSetPhoto(object sender, RoutedEventArgs e)
        {
            if (newCraft == null) return;
            if (selectedPhotoAll == null) return;

            newCraft.Thumbnail = selectedPhotoAll;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = selectedPhotoAll.Url;
            bi3.EndInit();

            NewPhoto.Source = bi3;
        }

        public void OnCraftSetPhoto(object sender, RoutedEventArgs e)
        {
            if (selectedCraft == null) return;
            if (selectedPhotoAll == null) return;

            selectedCraft.Thumbnail = selectedPhotoAll;

            IList<double?> photos = new List<double?>();
            selectedCraft.Photos.ForEach(photo => photos.Add(photo.Id));

            Models.UpdateCraftDto craftDto = new Models.UpdateCraftDto(
                selectedCraft.Id,
                selectedCraft.Name,
                selectedCraft.Description,
                selectedCraft.Material.Id,
                selectedCraft.Category.Id,
                selectedCraft.Thumbnail.Id
                );

            Task<HttpOperationResponse> resp = apiClient.CraftsController.UpdateWithHttpMessagesAsync(selectedCraft.Id.ToString(), craftDto);
            resp.Wait();

            LoadCrafts();
        }

        public void OnCraftDelete(object sender, RoutedEventArgs e)
        {
            if (selectedCraft == null) return;
            Task<HttpOperationResponse> resp = apiClient.CraftsController.RemoveWithHttpMessagesAsync(selectedCraft.Id);
            resp.Wait();

            LoadCrafts();
        }

        public void OnCraftUpdate(object sender, RoutedEventArgs e)
        {
            if (selectedCraft == null) return;


            IList<double?> photos = new List<double?>();
            selectedCraft.Photos.ForEach(photo => photos.Add(photo.Id));

            Models.UpdateCraftDto craftDto = new Models.UpdateCraftDto(
                selectedCraft.Id,
                selectedCraft.Name,
                selectedCraft.Description,
                selectedCraft.Material.Id,
                selectedCraft.Category.Id,
                selectedCraft.Thumbnail.Id
                );

            Task<HttpOperationResponse> resp = apiClient.CraftsController.UpdateWithHttpMessagesAsync(selectedCraft.Id.ToString() ,craftDto);
            resp.Wait();

            LoadCrafts();
        }
    }
}
