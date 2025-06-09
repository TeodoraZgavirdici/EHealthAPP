using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EHealthApp.Data;
using EHealthApp.Models;

namespace EHealthApp
{
    public partial class MedicalDocumentsPage : ContentPage
    {
        private readonly AppDatabase _database;

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
                OnPropertyChanged(nameof(SelectedCategory));
                _ = LoadDocumentsForCategoryAsync(value?.Name);
            }
        }

        public MedicalDocumentsPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database;

            BindingContext = this;

            if (Categories.Count > 0)
                SelectedCategory = Categories[0];
        }

        public class CategoryDTO { public string Name { get; set; } }
        public class DocumentDTO { public string FileName { get; set; } public string FilePath { get; set; } }

        private async Task LoadDocumentsForCategoryAsync(string category)
        {
            FilteredDocuments.Clear();
            if (string.IsNullOrWhiteSpace(category))
                return;

            var documents = await _database.GetMedicalDocumentsByCategoryAsync(category);

            foreach (var doc in documents.OrderByDescending(d => d.DateAdded))
            {
                if (File.Exists(doc.FilePath))
                {
                    FilteredDocuments.Add(new DocumentDTO
                    {
                        FileName = doc.Title,
                        FilePath = doc.FilePath
                    });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Fișier lipsă: {doc.FilePath}");
                    await _database.DeleteMedicalDocumentAsync(doc);
                }
            }
        }

        private async void OnAddMedicalDocumentClicked(object sender, EventArgs e)
        {
            try
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

                bool pdfCreated = await GeneratePdfFromImageAsync(photo.FullPath, pdfPath);
                if (!pdfCreated)
                {
                    await Toast.Make("Nu s-a putut crea PDF-ul").Show();
                    return;
                }

                var medicalDoc = new MedicalDocument
                {
                    Title = titlu,
                    FilePath = pdfPath,
                    DateAdded = DateTime.Now,
                    Category = categorie
                };

                await _database.SaveMedicalDocumentAsync(medicalDoc);

                SelectedCategory = Categories.FirstOrDefault(c => c.Name == categorie);
            }
            catch (Exception ex)
            {
                await Toast.Make($"Eroare: {ex.Message}").Show();
            }
        }

        private async Task<bool> GeneratePdfFromImageAsync(string imagePath, string pdfPath)
        {
            try
            {
                using var stream = File.OpenRead(imagePath);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;

                using var imageStream = new MemoryStream(ms.ToArray());
                using var image = XImage.FromStream(() => imageStream);

                var document = new PdfDocument();
                var page = document.AddPage();

                double dpiFactor = 72.0 / image.HorizontalResolution;
                page.Width = image.PixelWidth * dpiFactor;
                page.Height = image.PixelHeight * dpiFactor;

                using var gfx = XGraphics.FromPdfPage(page);
                gfx.DrawImage(image, 0, 0, page.Width, page.Height);

                using var output = File.Create(pdfPath);
                document.Save(output);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare generare PDF: {ex}");
                return false;
            }
        }

        private async void OnShareClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string path && File.Exists(path))
            {
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Distribuie documentul",
                    File = new ShareFile(path)
                });
            }
            else
            {
                await Toast.Make("Fișier inexistent pentru partajare").Show();
            }
        }

        private async void OnDownloadClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string path && File.Exists(path))
            {
                var fileName = Path.GetFileName(path);
                using var stream = File.OpenRead(path);
                var result = await CommunityToolkit.Maui.Storage.FileSaver.Default.SaveAsync(fileName, stream, default);
                await Toast.Make(result.IsSuccessful ? "PDF salvat!" : "Eroare la salvare!").Show();
            }
            else
            {
                await Toast.Make("Fișier inexistent pentru salvare").Show();
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}
