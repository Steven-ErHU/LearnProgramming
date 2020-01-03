using OdeToFood.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OdeToFood.Data
{
  public interface IRestaurantData
  {
    IEnumerable<Restaurant> GetRestaurantsByName(string name);
    Restaurant GetById(int Id);
  }

  public class InMemoryRestaurantData : IRestaurantData
  {
    List<Restaurant> restaurants;
    public InMemoryRestaurantData()
    {
      restaurants = new List<Restaurant>()
      {
        new Restaurant {Id = 1, Name = "Scott''s Pizza", Location = "Maryland", Cuisine = CuisineType.Italian },
        new Restaurant {Id = 2, Name = "jjGumbo''s", Location = "Nashville", Cuisine = CuisineType.Seafood },
        new Restaurant {Id = 3, Name = "Caesar''s", Location = "Tennessee", Cuisine = CuisineType.Italian }
      };
    }

    public Restaurant GetById(int id)
    {
      return restaurants.SingleOrDefault(r => r.Id == id);
    }
    public IEnumerable<Restaurant> GetRestaurantsByName(string name = null)
    {
      return from r in restaurants
             where string.IsNullOrEmpty(name) || r.Name.StartsWith(name)
             orderby r.Name
             select r;
    }

  }
}
