using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EHealthApp.Data;
using EHealthApp.Models;

namespace EHealthApp
{
    public partial class MedicalDocumentsPage : ContentPage, System.ComponentModel.INotifyPropertyChanged
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

        public ICommand SelectCategoryCommand { get; }

        public MedicalDocumentsPage(AppDatabase database)
        {
            InitializeComponent();
            _database = database;
            BindingContext = this;
            SelectCategoryCommand = new Command<CategoryDTO>(cat =>
            {
                if (cat != null && SelectedCategory != cat)
                    SelectedCategory = cat;
            });
            if (Categories.Any())
                SelectedCategory = Categories[0];
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

        private async Task<List<FileResult>> CaptureMultiplePhotosAsync()
        {
            var photos = new List<FileResult>();
            bool addMore = true;
            while (addMore)
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null)
                    break;
                photos.Add(photo);
                addMore = await DisplayAlert("Adaugă poze", "Vrei să faci încă o poză pentru PDF?", "Da", "Nu");
            }
            return photos;
        }

        private async Task<List<string>> CopyPhotoFilesLocallyAsync(List<FileResult> photos)
        {
            List<string> localPaths = new();
            foreach (var file in photos)
            {
                var localPath = Path.Combine(FileSystem.CacheDirectory, file.FileName);
                using var inStream = await file.OpenReadAsync();
                using var outStream = File.OpenWrite(localPath);
                await inStream.CopyToAsync(outStream);
                localPaths.Add(localPath);
            }
            return localPaths;
        }

        private async Task<bool> GeneratePdfFromImageListAsync(List<string> imagePaths, string pdfPath)
        {
            try
            {
                using var document = new PdfDocument();

                foreach (var imagePath in imagePaths)
                {
                    if (!File.Exists(imagePath))
                        continue;

                    using var imageStream = File.OpenRead(imagePath);
                    var image = new PdfBitmap(imageStream);
                    var page = document.Pages.Add();
                    var pageSize = page.GetClientSize();

                    float drawX = 0f;
                    float drawY = 0f;
                    float drawWidth = pageSize.Width;
                    float drawHeight = pageSize.Height;

                    page.Graphics.DrawRectangle(PdfBrushes.White, new Syncfusion.Drawing.RectangleF(0, 0, pageSize.Width, pageSize.Height));
                    page.Graphics.DrawImage(image, drawX, drawY, drawWidth, drawHeight);
                }

                using var outputStream = File.Create(pdfPath);
                document.Save(outputStream);
                return true;
            }
            catch (Exception ex)
            {
                await Toast.Make("Eroare PDF: " + ex.Message).Show();
                return false;
            }
        }


        private async void OnAddPhotoPdfDocumentClicked(object sender, EventArgs e)
        {
            try
            {
                var photoFiles = await CaptureMultiplePhotosAsync();
                if (photoFiles.Count == 0)
                {
                    await Toast.Make("Nu s-a făcut nicio poză!").Show();
                    return;
                }

                var categorie = await DisplayActionSheet("Alege categoria", "Anulează", null, Categories.Select(c => c.Name).ToArray());
                if (string.IsNullOrEmpty(categorie) || categorie == "Anulează")
                    return;

                var titlu = await DisplayPromptAsync("Titlu document", "Introdu un nume pentru fișierul PDF:");
                if (string.IsNullOrWhiteSpace(titlu))
                    titlu = $"Document_{DateTime.Now:yyyyMMdd_HHmmss}";

                string folder = Path.Combine(FileSystem.AppDataDirectory, categorie);
                Directory.CreateDirectory(folder);
                string pdfPath = Path.Combine(folder, $"{titlu}.pdf");

                var photoLocalPaths = await CopyPhotoFilesLocallyAsync(photoFiles);

                bool pdfCreated = await GeneratePdfFromImageListAsync(photoLocalPaths, pdfPath);
                if (!pdfCreated)
                {
                    await Toast.Make("Nu s-a putut crea PDF-ul!").Show();
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

        private async Task LoadDocumentsForCategoryAsync(string category)
        {
            FilteredDocuments.Clear();
            var docs = await _database.GetMedicalDocumentsByCategoryAsync(category);
            if (docs != null)
            {
                foreach (var doc in docs)
                {
                    FilteredDocuments.Add(new DocumentDTO
                    {
                        FileName = Path.GetFileName(doc.FilePath),
                        FilePath = doc.FilePath
                    });
                }
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

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is string filePath)
            {
                bool confirm = await DisplayAlert("Confirmare", "Ești sigur că vrei să ștergi acest document?", "Da", "Nu");
                if (!confirm)
                    return;

                try
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    var doc = await _database.GetMedicalDocumentByFilePathAsync(filePath);
                    if (doc != null)
                        await _database.DeleteMedicalDocumentAsync(doc);

                    await LoadDocumentsForCategoryAsync(SelectedCategory?.Name);

                    await Toast.Make("Document șters cu succes!").Show();
                }
                catch (Exception ex)
                {
                    await Toast.Make($"Eroare la ștergere: {ex.Message}").Show();
                }
            }
            else
            {
                await Toast.Make("Fișier lipsă sau invalid!").Show();
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}