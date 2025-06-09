using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
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

        public ObservableCollection<CategoryDTO> Categories { get; } = new()
        {
            new CategoryDTO { Name = "Retete" },
            new CategoryDTO { Name = "ScrisoriMedicale" },
            new CategoryDTO { Name = "Analize" },
            new CategoryDTO { Name = "Altele" }
        };

        public ObservableCollection<DocumentDTO> FilteredDocuments { get; } = new();

        private CategoryDTO _selectedCategory;
        public CategoryDTO SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                    _ = LoadDocumentsForCategoryAsync(value?.Name);
                }
            }
        }

        public MedicalDocumentsPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database;
            BindingContext = this;

            if (Categories.Any())
            {
                SelectedCategory = Categories[0];
                _ = LoadDocumentsForCategoryAsync(SelectedCategory.Name);
            }
        }

        public class CategoryDTO
        {
            public string Name { get; set; }
        }

        public class DocumentDTO
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

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
                if (photo == null)
                    return;

                var categorie = await DisplayActionSheet("Alege categoria", "Anulează", null, Categories.Select(c => c.Name).ToArray());
                if (string.IsNullOrEmpty(categorie))
                    return;

                var titlu = await DisplayPromptAsync("Titlu document", "Introdu un nume pentru fișierul PDF:");
                if (string.IsNullOrWhiteSpace(titlu))
                    titlu = $"Document_{DateTime.Now:yyyyMMdd_HHmmss}";

                string folder = Path.Combine(FileSystem.AppDataDirectory, categorie);
                Directory.CreateDirectory(folder);

                string pdfPath = Path.Combine(folder, $"{titlu}.pdf");

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

                await Toast.Make("Document salvat cu succes!").Show();
            }
            catch (Exception ex)
            {
                await Toast.Make($"Eroare: {ex.Message}").Show();
            }
        }

        /// <summary>
        /// Generează un PDF dintr-o imagine, adaptând dimensiunea imaginii să încapă pe pagina implicită A4, păstrând proporțiile.
        /// </summary>
        private async Task<bool> GeneratePdfFromImageAsync(string imagePath, string pdfPath)
        {
            try
            {
                using var document = new PdfDocument();

                // Adaugă pagina implicită (A4)
                var page = document.Pages.Add();

                using var imageStream = File.OpenRead(imagePath);
                var image = new PdfBitmap(imageStream);

                // Obține dimensiunea paginii (A4 implicit)
                var pageSize = page.GetClientSize();

                // Dimensiunea imaginii
                float imgWidth = image.Width;
                float imgHeight = image.Height;

                // Calculează raportul de scalare astfel încât imaginea să încapă pe pagină păstrând proporțiile
                float ratioX = pageSize.Width / imgWidth;
                float ratioY = pageSize.Height / imgHeight;
                float ratio = Math.Min(ratioX, ratioY);

                float width = imgWidth * ratio;
                float height = imgHeight * ratio;

                // Desenează imaginea scalată în colțul din stânga sus al paginii
                page.Graphics.DrawImage(image, 0, 0, width, height);

                using var outputStream = File.Create(pdfPath);
                document.Save(outputStream);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare generare PDF Syncfusion: {ex.Message}");
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
                var result = await CommunityToolkit.Maui.Storage.FileSaver.Default.SaveAsync(fileName, stream);
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
