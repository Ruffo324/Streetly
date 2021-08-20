using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OsmSharp;
using OsmSharp.API;
using OsmSharp.IO.API;

namespace Streetly
{

	public class Programmmmm
	{
		public static async Task Main()
		{

			var clientFactory = new ClientsFactory(null, new HttpClient(), "https://master.apis.dev.openstreetmap.org/api/");
			var client = clientFactory.CreateNonAuthClient();


			Console.WriteLine("City name: ");
			string cityName;
			while (!string.IsNullOrEmpty(cityName = Console.ReadLine()?.ToLower()))
			{
				var cityResult = await CityResult.SearchByName(cityName);
				var mapNode = await client.GetMap(cityResult.Boundingbox);
				var ways = mapNode.Ways.Where(w => w.Type == OsmGeoType.Way);
				
				
				var wayStrings = ways.Select(JsonConvert.SerializeObject).ToImmutableArray();
				foreach (var asdf in wayStrings)
				{
					Console.WriteLine(asdf);
				}
			}
		}
	}


	public readonly struct CityResult
	{
		[JsonConstructor]
		public CityResult(
			long placeId, string licence, string osmType, long osmId, string[] boundingbox, string lat, string lon, string displayName, long placeRank, string category,
			string type, double importance, Uri icon)
		{
			var box = boundingbox.Select(float.Parse).ToImmutableArray();
			Boundingbox = new Bounds
			{
				MinLatitude = box[0],
				MaxLatitude = box[1],
				MinLongitude = box[2],
				MaxLongitude = box[3]
			};
			
			PlaceId = placeId;
			Licence = licence;
			OsmType = osmType;
			OsmId = osmId;
			Lat = lat;
			Lon = lon;
			DisplayName = displayName;
			PlaceRank = placeRank;
			Category = category;
			Type = type;
			Importance = importance;
			Icon = icon;
		}

		public static async Task<CityResult> SearchByName(string cityName)
		{
			// https://nominatim.openstreetmap.org/ui/search.html
			var escapedCity = Uri.EscapeUriString(cityName);
			var searchUri = $"https://nominatim.openstreetmap.org/search.php?city={escapedCity}&format=jsonv2;";

			using var webclient = new WebClient();
			var jsonReply = await webclient.DownloadStringTaskAsync(new Uri(searchUri));
			return JsonConvert.DeserializeObject<CityResult>(jsonReply);
		}

		[JsonProperty("place_id")]
		public long PlaceId { get; }

		[JsonProperty("licence")]
		public string Licence { get; }

		[JsonProperty("osm_type")]
		public string OsmType { get; }

		[JsonProperty("osm_id")]
		public long OsmId { get; }

		[JsonProperty("boundingbox")]
		public Bounds Boundingbox { get; }

		[JsonProperty("lat")]
		public string Lat { get; }

		[JsonProperty("lon")]
		public string Lon { get; }

		[JsonProperty("display_name")]
		public string DisplayName { get; }

		[JsonProperty("place_rank")]
		public long PlaceRank { get; }

		[JsonProperty("category")]
		public string Category { get; }

		[JsonProperty("type")]
		public string Type { get; }

		[JsonProperty("importance")]
		public double Importance { get; }

		[JsonProperty("icon")]
		public Uri Icon { get; }
	}
}
