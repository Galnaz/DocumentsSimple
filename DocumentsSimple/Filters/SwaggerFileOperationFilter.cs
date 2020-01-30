using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsSimple.Filters
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.OperationId == "Post")
            {
                operation.Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "myFile",
                        Required = true,
                        Schema =
                        {
                            Type = "file",

                        },
                        In = ParameterLocation.Header
                    }
                };
            }
        }
    }
}
