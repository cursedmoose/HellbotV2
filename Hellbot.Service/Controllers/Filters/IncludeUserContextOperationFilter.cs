using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Hellbot.Service.Controllers.Filters
{
    public sealed class IncludeUserContextOperationFilter : IOperationFilter
    {
        private const string ParamHellbotUserId = "asHellbotUserId";
        private const string ParamTwitchLogin = "asTwitchLogin";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor cad)
                return;

            if (!UsesIncludeUserContext(cad))
                return;

            if (operation.Parameters is null)
                return;

            if (HasQueryParam(operation.Parameters, ParamHellbotUserId)
                || HasQueryParam(operation.Parameters, ParamTwitchLogin))
                return;

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = ParamHellbotUserId,
                In = ParameterLocation.Query,
                Required = false,
                Schema = new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid" },
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = ParamTwitchLogin,
                In = ParameterLocation.Query,
                Required = false,
                Schema = new OpenApiSchema { Type = JsonSchemaType.String },
            });
        }

        private static bool HasQueryParam(IList<IOpenApiParameter> list, string name)
        {
            foreach (var p in list)
            {
                if (p is OpenApiParameter op &&
                    string.Equals(op.Name, name, StringComparison.OrdinalIgnoreCase) &&
                    op.In == ParameterLocation.Query)
                    return true;
            }
            return false;
        }

        private static bool UsesIncludeUserContext(ControllerActionDescriptor cad) =>
            cad.EndpointMetadata.OfType<IncludeUserContextAttribute>().Any()
            || Attribute.IsDefined(cad.ControllerTypeInfo, typeof(IncludeUserContextAttribute), inherit: true)
            || Attribute.IsDefined(cad.MethodInfo, typeof(IncludeUserContextAttribute), inherit: false);
    }
}
