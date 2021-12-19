using Microsoft.Rest;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AdminClient
{
    /// <summary>
    /// Interaction logic for NewPhoto.xaml
    /// </summary>
    /// 

    public partial class NewPhoto : Window
    {
        private string url;
        private CrocodileHandycraft apiClient;
        public NewPhoto()
        {
            apiClient = ((App)Application.Current).crocodileHandycraft;

            InitializeComponent();
        }

        public void OnUrlChanged(object sender, TextChangedEventArgs e)
        {
            url = TBurl.Text;

            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && uriResult.Scheme == Uri.UriSchemeHttp;

            if (!result) return;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(url, UriKind.Absolute);
            bi3.EndInit();

            PreviewImage.Source = bi3;
        }

        private void OnBTUpload(object sender, RoutedEventArgs e)
        {
            Models.CreatePhotoDto photoDto = new Models.CreatePhotoDto();

            photoDto.Url = url;
            photoDto.Description = TBdesc.Text == null ? "-" : TBdesc.Text;

            Task<HttpOperationResponse> respPhoto = apiClient.PhotosController.CreateWithHttpMessagesAsync(photoDto);

            respPhoto.Wait();
            this.Close();
        }
    }
}
