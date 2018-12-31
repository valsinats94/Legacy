using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using SortMImage.Exceptions;
using SortMImage.Models;
using SortMImage.Services.ExportServices;
using static SortMImage.Enums.Enums;

namespace SortMImage.ViewModels
{
    public class SortedImagesViewModel : BaseViewModel
    {
        #region Declarations

        private ObservableCollection<ImageModel> interiorObjects;
        private ObservableCollection<ImageModel> natureLandscape;
        private ObservableCollection<ImageModel> beachesSeaside;
        private ObservableCollection<ImageModel> eventsParties;
        private ObservableCollection<ImageModel> foodDrinks;
        private ObservableCollection<ImageModel> paintingsArt;
        private ObservableCollection<ImageModel> petsAnimals;
        private ObservableCollection<ImageModel> textVisuals;
        private ObservableCollection<ImageModel> sunrisesSunsets;
        private ObservableCollection<ImageModel> carsVehicles;
        private ObservableCollection<ImageModel> macroFlowers;
        private ObservableCollection<ImageModel> streetviewArchitecture;
        private ObservableCollection<ImageModel> peoplePortraits;

        private DelegateCommand<object> exportCommand;

        #endregion

        #region Constructors
        #endregion

        #region Properties

        #region Collections

        public ObservableCollection<ImageModel> InteriorObjects
        {
            get
            {
                if (interiorObjects == null)
                    interiorObjects = new ObservableCollection<ImageModel>();

                return interiorObjects;
            }
        }

        public ObservableCollection<ImageModel> NatureLandscape
        {
            get
            {
                if (natureLandscape == null)
                    natureLandscape = new ObservableCollection<ImageModel>();

                return natureLandscape;
            }
        }

        public ObservableCollection<ImageModel> BeachesSeaside
        {
            get
            {
                if (beachesSeaside == null)
                    beachesSeaside = new ObservableCollection<ImageModel>();

                return beachesSeaside;
            }
        }

        public ObservableCollection<ImageModel> EventsParties
        {
            get
            {
                if (eventsParties == null)
                    eventsParties = new ObservableCollection<ImageModel>();

                return eventsParties;
            }
        }

        public ObservableCollection<ImageModel> FoodDrinks
        {
            get
            {
                if (foodDrinks == null)
                    foodDrinks = new ObservableCollection<ImageModel>();

                return foodDrinks;
            }
        }

        public ObservableCollection<ImageModel> PaintingsArt
        {
            get
            {
                if (paintingsArt == null)
                    paintingsArt = new ObservableCollection<ImageModel>();

                return paintingsArt;
            }
        }

        public ObservableCollection<ImageModel> PetsAnimals
        {
            get
            {
                if (petsAnimals == null)
                    petsAnimals = new ObservableCollection<ImageModel>();

                return petsAnimals;
            }
        }

        public ObservableCollection<ImageModel> TextVisuals
        {
            get
            {
                if (textVisuals == null)
                    textVisuals = new ObservableCollection<ImageModel>();

                return textVisuals;
            }
        }

        public ObservableCollection<ImageModel> SunrisesSunsets
        {
            get
            {
                if (sunrisesSunsets == null)
                    sunrisesSunsets = new ObservableCollection<ImageModel>();

                return sunrisesSunsets;
            }
        }

        public ObservableCollection<ImageModel> CarsVehicles
        {
            get
            {
                if (carsVehicles == null)
                    carsVehicles = new ObservableCollection<ImageModel>();

                return carsVehicles;
            }
        }

        public ObservableCollection<ImageModel> MacroFlowers
        {
            get
            {
                if (macroFlowers == null)
                    macroFlowers = new ObservableCollection<ImageModel>();

                return macroFlowers;
            }
        }

        public ObservableCollection<ImageModel> StreetviewArchitecture
        {
            get
            {
                if (streetviewArchitecture == null)
                    streetviewArchitecture = new ObservableCollection<ImageModel>();

                return streetviewArchitecture;
            }
        }

        public ObservableCollection<ImageModel> PeoplePortraits
        {
            get
            {
                if (peoplePortraits == null)
                    peoplePortraits = new ObservableCollection<ImageModel>();

                return peoplePortraits;
            }
        }

        #endregion

        public DelegateCommand<object> ExportCommand
        {
            get
            {
                if (exportCommand == null)
                    exportCommand = new DelegateCommand<object>(Export);

                return exportCommand;
            }
        }

        #endregion

        #region Methods

        private void Export(object obj)
        {
            SortedListsOfImagesNames imagesCollectionType = (SortedListsOfImagesNames)obj;
            ExportService exportService = ExportServiceFactory.GetExportService(ExportTypes.ExportToZip);
            switch (imagesCollectionType)
            {
                case SortedListsOfImagesNames.BeachesSeaside:
                    exportService.Export(BeachesSeaside, imagesCollectionType.ToString());
                    BeachesSeaside.Clear();
                    break;
                case SortedListsOfImagesNames.CarsVehicles:
                    exportService.Export(CarsVehicles, imagesCollectionType.ToString());
                    CarsVehicles.Clear();
                    break;
                case SortedListsOfImagesNames.EventsParties:
                    exportService.Export(EventsParties, imagesCollectionType.ToString());
                    EventsParties.Clear();
                    break;
                case SortedListsOfImagesNames.FoodDrinks:
                    exportService.Export(FoodDrinks, imagesCollectionType.ToString());
                    FoodDrinks.Clear();
                    break;
                case SortedListsOfImagesNames.InteriorObjects:
                    exportService.Export(InteriorObjects, imagesCollectionType.ToString());
                    InteriorObjects.Clear();
                    break;
                case SortedListsOfImagesNames.MacroFlowers:
                    exportService.Export(MacroFlowers, imagesCollectionType.ToString());
                    MacroFlowers.Clear();
                    break;
                case SortedListsOfImagesNames.NatureLandscape:
                    exportService.Export(NatureLandscape, imagesCollectionType.ToString());
                    NatureLandscape.Clear();
                    break;
                case SortedListsOfImagesNames.PaintingsArt:
                    exportService.Export(PaintingsArt, imagesCollectionType.ToString());
                    PaintingsArt.Clear();
                    break;
                case SortedListsOfImagesNames.PeoplePortraits:
                    exportService.Export(PeoplePortraits, imagesCollectionType.ToString());
                    PeoplePortraits.Clear();
                    break;
                case SortedListsOfImagesNames.PetsAnimals:
                    exportService.Export(PetsAnimals, imagesCollectionType.ToString());
                    PetsAnimals.Clear();
                    break;
                case SortedListsOfImagesNames.StreetviewArchitecture:
                    exportService.Export(StreetviewArchitecture, imagesCollectionType.ToString());
                    StreetviewArchitecture.Clear();
                    break;
                case SortedListsOfImagesNames.SunrisesSunsets:
                    exportService.Export(SunrisesSunsets, imagesCollectionType.ToString());
                    SunrisesSunsets.Clear();
                    break;
                case SortedListsOfImagesNames.TextVisuals:
                    exportService.Export(TextVisuals, imagesCollectionType.ToString());
                    TextVisuals.Clear();
                    break;
                default:
                    throw new NoSuchCategoryException("There is no such images category! Contact the application manufacturer!");
            }
        }

        #endregion
    }
}
