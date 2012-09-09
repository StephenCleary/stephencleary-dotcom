using StephenCleary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class CSharpFormatterController : ApiController
    {
        public string Get(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The \"code\" parameter must be specified."));
            return CSharpFormatter.CSharp(code).ToString();
        }
    }
}