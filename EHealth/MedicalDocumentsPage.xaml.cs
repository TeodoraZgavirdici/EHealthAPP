using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Maui.Storage;
using System.Threading.Tasks;

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
                FileResult? photo = null;
                try
                {
                    photo = await MediaPicker.CapturePhotoAsync();
                }
                catch (Exception ex)
                {
                    await Toast.Make($"Eroare la captură: {ex.Message}").Show();
                }

                if (photo != null)
                {
                    await Toast.Make($"Tip fișier: {photo.ContentType}, extensie: {Path.GetExtension(photo.FileName)}").Show();
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

                await Toast.Make("Începe generarea PDF...").Show();
                await GeneratePdfFromImagesAsync(capturedPhotos, pdfPath);
                await Toast.Make("Document salvat!").Show();

                LoadDocuments(); // Refresh lista
            }
            else
            {
                await Toast.Make("Nu ai selectat o categorie sau nu ai adăugat poze!").Show();
            }
        }

        // Varianta FARA ImageSharp: se insereaza direct JPG-urile in PDF
        private async Task GeneratePdfFromImagesAsync(List<FileResult> photos, string pdfPath)
        {
            try
            {
                var pdf = new PdfDocument();
                foreach (var photo in photos)
                {
                    try
                    {
                        await Toast.Make($"Procesez {photo.FileName}").Show();
                        using var stream = await photo.OpenReadAsync();
                        await Toast.Make($"Stream length: {stream.Length}").Show();
                        await Toast.Make($"ContentType: {photo.ContentType}").Show();

                        using var xImage = XImage.FromStream(() => stream);
                        var page = pdf.AddPage();
                        page.Width = xImage.PixelWidth * 72 / xImage.HorizontalResolution;
                        page.Height = xImage.PixelHeight * 72 / xImage.VerticalResolution;

                        using (var gfx = XGraphics.FromPdfPage(page))
                        {
                            gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                        }

                        await Toast.Make($"Adăugat imaginea!").Show();
                    }
                    catch (Exception ex)
                    {
                        await Toast.Make($"Eroare la procesare imagine: {ex.GetType().Name}: {ex.Message}").Show();
                        return;
                    }
                }
                try
                {
                    using var file = File.Create(pdfPath);
                    pdf.Save(file);
                    await Toast.Make("PDF generat!").Show();
                }
                catch (Exception exSave)
                {
                    await Toast.Make($"Eroare la salvarea PDF: {exSave.GetType().Name}: {exSave.Message}").Show();
                }
            }
            catch (Exception ex)
            {
                await Toast.Make($"Eroare generală la generarea PDF: {ex.GetType().Name}: {ex.Message}").Show();
            }
        }

        private async void OnShareClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string filePath)
            {
                try
                {
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = "Share document",
                        File = new ShareFile(filePath)
                    });
                }
                catch (Exception ex)
                {
                    await Toast.Make($"Eroare la share: {ex.GetType().Name}: {ex.Message}").Show();
                }
            }
        }

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
                catch (Exception ex)
                {
                    await Toast.Make($"Eroare la descărcare: {ex.GetType().Name}: {ex.Message}").Show();
                }
            }
        }
    }
}