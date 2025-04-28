
using System.Net;
using System.Text.Json;
using ECommmerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommmerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "Lo sentimos error interno del sistema, Intente mas tarde";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                //REFORMA CON SWITCH
                //Revisar si la excepction es por muchas peticiones//429
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Advertencia";
                    message = "Demasiados intentos";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }
                //Si la respues es denegada //401 status code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alerta";
                    message = "No tiene autorización";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // Si la respuesta es denegada
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Fuera de acceso";
                    message = "No tienes permitido entrar";
                    await ModifyHeader(context, title, message, statusCode);
                }


            }
            catch (Exception ex)
            {
                //Creamos un log exception del tipo file debugger y consoles

                LogException.LogExceptions(ex);
                //check id Exception is timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Fuera de tiempo";
                    message = "la peticion fuera de tiempo intenta mas tarde";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }
                //ninguna de la peticiones que especificamos dio entonces mostramos un 
                //mensaje por defecto
                await ModifyHeader(context, title, message, statusCode);
            }

        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // Establecemos eel tipo de contenido como JSON
            context.Response.ContentType = "application/json";

            // Crear el objeto ProblemDetails con los detalles del error
            var problemDetails = new ProblemDetails
            {
                Title = title,       // Título del problema
                Detail = message,    // Descripción detallada del error
                Status = statusCode  // Código de estado HTTP (ej. 500, 404, etc.)
            };

            // Serializamos el objeto ProblemDetails a JSON
            string jsonResponse = JsonSerializer.Serialize(problemDetails);

            // Enviamos la respuesta al cliente de manera asíncrona
            await context.Response.WriteAsync(jsonResponse, CancellationToken.None);
        }


    }
}

