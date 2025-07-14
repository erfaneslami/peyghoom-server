using Peyghoom.Endpoints;

namespace Peyghoom.Extensions;

public static class EndpointGroupExtensions
{
   public static void MapEndpointsGroup(this IEndpointRouteBuilder endpointRouteBuilder)
   {
      // Reflection-based discovery:
      var endpointGroups = typeof(Program).Assembly
         .GetTypes()
         .Where(t => t is { IsAbstract: false, IsInterface: false }
                     && typeof(IEndpointGroup).IsAssignableFrom(t))
         .Select(Activator.CreateInstance)
         .Cast<IEndpointGroup>();

      foreach (var group in endpointGroups)
      {
         group.MapEndpoints(endpointRouteBuilder);
      }
   } 
}