using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Maui.Storage;
using System.Threading.Tasks;
// Rezolvă ambiguitatea pentru Image
using ImageSharpImage = SixLabors.ImageSharp.Image;

namespace EHealthApp
{
    public partial class MedicalDocumentsPage : ContentPage
    {
        public ObservableCollection<CategoryGroup> DocumentsGrouped { get; set; } = new();

        private List<FileResult> capturedPhotos = new();

        public MedicalDocumentsPage()
        {
            InitializeComponent();
            RequestPermissionsAsync();
            LoadDocuments();
            DocumentsCollectionView.ItemsSource = DocumentsGrouped;
        }

        // Metodă async pentru cerere permisiuni
        private async void RequestPermissionsAsync()
        {
#if ANDROID
            var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
            {
                await DisplayAlert("Permisiuni necesare",
                    "Trebuie să acorzi permisiunea de cameră pentru a folosi această funcționalitate.",
                    "OK");
            }
#endif
        }

        // Structuri pentru grupare și afișare documente
        public class CategoryGroup : ObservableCollection<DocumentDTO>
        {
            public string Category { get; set; }
            public CategoryGroup(string category, IEnumerable<DocumentDTO> docs) : base(docs) => Category = category;
        }
        public class DocumentDTO
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        private void LoadDocuments()
        {
            DocumentsGrouped.Clear();
            var root = FileSystem.AppDataDirectory;
            var categorii = new[] { "Retete", "ScrisoriMedicale", "Analize", "Altele" };

            foreach (var cat in categorii)
            {
                var catDir = Path.Combine(root, cat);
                if (Directory.Exists(catDir))
                {
                    var files = Directory.GetFiles(catDir, "*.pdf")
                        .Select(f => new DocumentDTO
                        {
                            FileName = Path.GetFileName(f),
                            FilePath = f
                        }).ToList();

                    if (files.Count > 0)
                        DocumentsGrouped.Add(new CategoryGroup(cat, files));
                }
            }
        }

        private async void OnAddMedicalDocumentClicked(object sender, EventArgs e)
        {
            capturedPhotos.Clear();

            bool adaugaAlta;
            do
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    capturedPhotos.Add(photo);
                }
                adaugaAlta = await DisplayAlert("Pagină nouă?", "Mai adaugi o pagină la acest document?", "DA", "NU");
            } while (adaugaAlta);

            string[] categorii = { "Retete", "ScrisoriMedicale", "Analize", "Altele" };
            string categorie = await DisplayActionSheet("Alege categoria", "Anulează", null, categorii);

            if (capturedPhotos.Count > 0 && !string.IsNullOrEmpty(categorie) && categorii.Contains(categorie))
            {
                string folder = Path.Combine(FileSystem.AppDataDirectory, categorie);
                Directory.CreateDirectory(folder);

                string pdfPath = Path.Combine(folder, $"Document_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                await GeneratePdfFromImagesAsync(capturedPhotos, pdfPath);

                await Toast.Make("Document salvat!").Show();

                LoadDocuments(); // Refresh lista
            }
        }

        private async Task GeneratePdfFromImagesAsync(List<FileResult> photos, string pdfPath)
        {
            var pdf = new PdfDocument();
            foreach (var photo in photos)
            {
                using var stream = await photo.OpenReadAsync();
                using var ms = new MemoryStream();
                // Conversie imagine cu ImageSharp pentru compatibilitate cu PdfSharpCore
                using (var img = ImageSharpImage.Load<Rgba32>(stream))
                {
                    img.SaveAsJpeg(ms);
                }
                ms.Position = 0;

                using var xImage = XImage.FromStream(() => ms);
                var page = pdf.AddPage();
                page.Width = xImage.PixelWidth * 72 / xImage.HorizontalResolution;
                page.Height = xImage.PixelHeight * 72 / xImage.VerticalResolution;

                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                }
            }
            using var file = File.Create(pdfPath);
            pdf.Save(file);
        }

        private async void OnShareClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string filePath)
            {
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Share document",
                    File = new ShareFile(filePath)
                });
            }
        }

        // NOU: Metoda multiplatform pentru download
        private async void OnDownloadClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string filePath)
            {
                var fileName = Path.GetFileName(filePath);

                try
                {
                    using var stream = File.OpenRead(filePath);
                    var result = await FileSaver.Default.SaveAsync(
                        fileName,
                        stream,
                        default);

                    if (result.IsSuccessful)
                        await Toast.Make($"PDF salvat cu succes!").Show();
                    else
                        await Toast.Make("Eroare la salvare!").Show();
                }
                catch
                {
                    await Toast.Make("Eroare la descărcare!").Show();
                }
            }
        }
    }
}