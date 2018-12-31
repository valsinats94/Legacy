using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMImage.Enums
{
    public class Enums
    {
        public enum SortedListsOfImagesNames
        {
            InteriorObjects,
            NatureLandscape,
            BeachesSeaside,
            EventsParties,
            FoodDrinks,
            PaintingsArt,
            PetsAnimals,
            TextVisuals,
            SunrisesSunsets,
            CarsVehicles,
            MacroFlowers,
            StreetviewArchitecture,
            PeoplePortraits,
        }

        public enum ExportTypes
        {
            ExportToZip
        }
    }
}
