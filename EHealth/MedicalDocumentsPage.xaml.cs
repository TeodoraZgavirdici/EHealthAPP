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
            LoadDocuments();
            DocumentsCollectionView.ItemsSource = DocumentsGrouped;
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

        private async void OnDownloadClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string filePath)
            {
                var fileName = Path.GetFileName(filePath);
#if ANDROID || IOS
                try
                {
                    var downloads = Path.Combine(FileSystem.Current.AppDataDirectory, "..", "Download");
                    Directory.CreateDirectory(downloads);
                    var dest = Path.Combine(downloads, fileName);
                    File.Copy(filePath, dest, overwrite: true);
                    await Toast.Make($"Salvat în Download: {fileName}").Show();
                }
                catch
                {
                    await Toast.Make("Eroare la descărcare!").Show();
                }
#elif WINDOWS
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;
                savePicker.InitializeWithWindow(hwnd);
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
                savePicker.FileTypeChoices.Add("PDF", new List<string>() { ".pdf" });
                savePicker.SuggestedFileName = fileName;

                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    using (var stream = await file.OpenStreamForWriteAsync())
                    using (var src = File.OpenRead(filePath))
                    {
                        await src.CopyToAsync(stream);
                    }
                    await Toast.Make($"PDF descărcat: {fileName}").Show();
                }
#endif
            }
        }
    }
}