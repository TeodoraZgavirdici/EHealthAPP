using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Storage;

namespace EHealthApp
{
    public partial class MedicalDocumentsPage : ContentPage
    {
        // Categoriile disponibile
        public ObservableCollection<CategoryDTO> Categories { get; set; } = new()
        {
            new CategoryDTO { Name = "Retete" },
            new CategoryDTO { Name = "ScrisoriMedicale" },
            new CategoryDTO { Name = "Analize" },
            new CategoryDTO { Name = "Altele" }
        };

        // Documentele filtrate pentru categoria selectată
        public ObservableCollection<DocumentDTO> FilteredDocuments { get; set; } = new();

        private List<FileResult> capturedPhotos = new();
        private CategoryDTO _selectedCategory;
        public CategoryDTO SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    LoadDocumentsForCategory(_selectedCategory?.Name);
                    OnPropertyChanged(nameof(SelectedCategory));
                }
            }
        }

        public MedicalDocumentsPage()
        {
            InitializeComponent();
            BindingContext = this;
            RequestPermissionsAsync();

            // Selectează implicit prima categorie
            if (Categories.Count > 0)
                SelectedCategory = Categories[0];
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

        public class CategoryDTO
        {
            public string Name { get; set; }
        }

        public class DocumentDTO
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        private void LoadDocumentsForCategory(string category)
        {
            FilteredDocuments.Clear();
            if (string.IsNullOrEmpty(category)) return;
            var folder = Path.Combine(FileSystem.AppDataDirectory, category);
            if (Directory.Exists(folder))
            {
                // Folosim sortare descrescătoare după dată pentru ca documentele noi să fie primele
                var files = Directory.GetFiles(folder, "*.pdf")
                                     .OrderByDescending(File.GetCreationTime)
                                     .ToList();

                foreach (var file in files)
                {
                    FilteredDocuments.Add(new DocumentDTO
                    {
                        FileName = Path.GetFileName(file),
                        FilePath = file
                    });
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

            var categorii = Categories.Select(c => c.Name).ToArray();
            string categorie = await DisplayActionSheet("Alege categoria", "Anulează", null, categorii);

            if (capturedPhotos.Count > 0 && !string.IsNullOrEmpty(categorie) && categorii.Contains(categorie))
            {
                string folder = Path.Combine(FileSystem.AppDataDirectory, categorie);
                Directory.CreateDirectory(folder);

                string pdfPath = Path.Combine(folder, $"Document_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                await Toast.Make("Începe generarea PDF...").Show();
                await GeneratePdfFromImagesAsync(capturedPhotos, pdfPath);
                await Toast.Make("Document salvat!").Show();

                // Actualizează lista pentru categoria selectată dacă e aceeași cu cea la care ai adăugat
                // Dacă nu, schimbă automat categoria la cea la care ai adăugat documentul nou!
                if (SelectedCategory != null && categorie == SelectedCategory.Name)
                {
                    LoadDocumentsForCategory(SelectedCategory.Name);
                }
                else
                {
                    // Schimbă categoria selectată ca să vadă automat documentul nou adăugat!
                    var found = Categories.FirstOrDefault(c => c.Name == categorie);
                    if (found != null)
                        SelectedCategory = found;
                }
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
                        using var stream = await photo.OpenReadAsync();

                        using var xImage = XImage.FromStream(() => stream);
                        var page = pdf.AddPage();
                        page.Width = xImage.PixelWidth * 72 / xImage.HorizontalResolution;
                        page.Height = xImage.PixelHeight * 72 / xImage.VerticalResolution;

                        using (var gfx = XGraphics.FromPdfPage(page))
                        {
                            gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                        }
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

        // INotifyPropertyChanged pentru binding
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}