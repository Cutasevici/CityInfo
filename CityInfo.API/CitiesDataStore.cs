using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }

        //public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public CitiesDataStore() 
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "The one with that big park.",
                    PointOfInterest = new List<PointOfInterestDtio>()
                    {
                        new PointOfInterestDtio()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited urban park" +
                            " in the US" },
                        new PointOfInterestDtio()
                        {
                            Id = 2,
                            Name = "Empire state building",
                            Description = "A 102-story skyscraper located in" +
                            "midtown manhattan." },
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with the big tower. ",
                    PointOfInterest = new List<PointOfInterestDtio>()
                    {
                        new PointOfInterestDtio()
                        {
                            Id = 5,
                            Name = "Eiffel Tower",
                            Description = "A wrought iron latice tower " +
                            "on the Champ de Mars, named after "
                        },
                        new PointOfInterestDtio()
                        {
                            Id = 6,
                            Name = " Eiffel tower",
                            Description = "The world's largest museum."
                        },
                    }
                }
                   
            };
        }
    }
}
