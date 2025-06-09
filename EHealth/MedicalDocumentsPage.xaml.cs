using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace EHealthApp
{
    public partial class MedicalDocumentsPage : ContentPage
    {
        public ObservableCollection<CategoryDTO> Categories { get; set; } = new()
        {
            new CategoryDTO { Name = "Retete" },
            new CategoryDTO { Name = "ScrisoriMedicale" },
            new CategoryDTO { Name = "Analize" },
            new CategoryDTO { Name = "Altele" }
        };

        public ObservableCollection<DocumentDTO> FilteredDocuments { get; set; } = new();

        private CategoryDTO _selectedCategory;
        public CategoryDTO SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                LoadDocumentsForCategory(value?.Name);
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public MedicalDocumentsPage()
        {
            InitializeComponent();
            BindingContext = this;
            if (Categories.Count > 0)
                SelectedCategory = Categories[0];
        }

        public class CategoryDTO { public string Name { get; set; } }
        public class DocumentDTO { public string FileName { get; set; } public string FilePath { get; set; } }

        private void LoadDocumentsForCategory(string category)
        {
            FilteredDocuments.Clear();
            if (string.IsNullOrWhiteSpace(category)) return;

            string folder = Path.Combine(FileSystem.AppDataDirectory, category);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var files = Directory.GetFiles(folder).Where(f => f.EndsWith(".pdf")).OrderByDescending(File.GetCreationTime);
            foreach (var file in files)
            {
                FilteredDocuments.Add(new DocumentDTO { FileName = Path.GetFileName(file), FilePath = file });
            }
        }

        private async void OnAddMedicalDocumentClicked(object sender, EventArgs e)
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo == null) return;

            var categorie = await DisplayActionSheet("Alege categoria", "Anulează", null, Categories.Select(c => c.Name).ToArray());
            if (string.IsNullOrEmpty(categorie)) return;

            var titlu = await DisplayPromptAsync("Titlu document", "Introdu un nume pentru fișierul PDF:");
            if (string.IsNullOrWhiteSpace(titlu))
                titlu = $"Document_{DateTime.Now:yyyyMMdd_HHmmss}";

            string folder = Path.Combine(FileSystem.AppDataDirectory, categorie);
            Directory.CreateDirectory(folder);

            string pdfPath = Path.Combine(folder, titlu + ".pdf");
            await GeneratePdfFromImageAsync(photo.FullPath, pdfPath);

            var foundCategory = Categories.FirstOrDefault(c => c.Name == categorie);
            if (foundCategory != null)
                SelectedCategory = foundCategory;
        }

        private async Task GeneratePdfFromImageAsync(string imagePath, string pdfPath)
        {
            try
            {
                using var stream = File.OpenRead(imagePath);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;

                using var image = XImage.FromStream(() => new MemoryStream(ms.ToArray()));
                var document = new PdfDocument();
                var page = document.AddPage();

                double dpiFactor = 72.0 / image.HorizontalResolution;
                page.Width = image.PixelWidth * dpiFactor;
                page.Height = image.PixelHeight * dpiFactor;

                using var gfx = XGraphics.FromPdfPage(page);
                gfx.DrawImage(image, 0, 0, page.Width, page.Height);

                using var output = File.Create(pdfPath);
                document.Save(output);
            }
            catch (Exception ex)
            {
                await Toast.Make($"Eroare PDF: {ex.Message}").Show();
            }
        }

        private async void OnShareClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string path)
            {
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Distribuie documentul",
                    File = new ShareFile(path)
                });
            }
        }

        private async void OnDownloadClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string path)
            {
                var fileName = Path.GetFileName(path);
                using var stream = File.OpenRead(path);
                var result = await CommunityToolkit.Maui.Storage.FileSaver.Default.SaveAsync(fileName, stream, default);
                await Toast.Make(result.IsSuccessful ? "PDF salvat!" : "Eroare la salvare!").Show();
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}