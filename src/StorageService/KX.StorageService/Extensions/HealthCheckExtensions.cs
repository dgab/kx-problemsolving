using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace KX.StorageService.Extensions
{
	public class HealthResult
	{
		public string Name { get; set; }
		public string Status { get; set; }
		public TimeSpan Duration { get; set; }
		public ICollection<HealthInfo> Info { get; set; }
	}

	public class HealthInfo
	{
		public string Key { get; set; }
		public string Description { get; set; }
		public TimeSpan Duration { get; set; }
		public string Status { get; set; }
		public string Error { get; set; }
	}

	public static class HealthCheckExtensions
	{
		public static IEndpointConventionBuilder MapCustomHealthChecks(
			this IEndpointRouteBuilder endpoints, string path, string serviceName)
		{
			return endpoints.MapHealthChecks(path, new HealthCheckOptions
			{
				ResponseWriter = async (context, report) =>
				{
					var result = JsonConvert.SerializeObject(
						new HealthResult
						{
							Name = serviceName,
							Status = report.Status.ToString(),
							Duration = report.TotalDuration,
							Info = report.Entries.Select(e => new HealthInfo
							{
								Key = e.Key,
								Description = e.Value.Description,
								Duration = e.Value.Duration,
								Status = Enum.GetName(typeof(HealthStatus),
														e.Value.Status),
								Error = e.Value.Exception?.Message
							}).ToList()
						}, Formatting.None,
						new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
					context.Response.ContentType = MediaTypeNames.Application.Json;
					await context.Response.WriteAsync(result);
				}
			});
		}
	}
}
