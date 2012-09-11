using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StephenCleary;
using System.Dynamic;

namespace Api.Controllers
{
    public class GuidDecoderController : ApiController
    {
        public dynamic Get(string guid)
        {
            Guid value;
            if (!Guid.TryParse(guid, out value))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not parse GUID."));
            dynamic ret = new ExpandoObject();
            ret.Variant = value.GetVariant();
            if (ret.Variant != GuidVariant.RFC4122)
                return ret;
            ret.Version = value.GetVersion();
            if (ret.Version == GuidVersion.Random || ret.Version == GuidVersion.NameBasedUsingMD5 || ret.Version == GuidVersion.NameBasedUsingSHA1)
                return ret;
            else if (ret.Version == GuidVersion.TimeBased)
            {
                ret.ClockSequence = value.GetClockSequence();
                ret.Timestamp = value.GetCreateTime();
                ret.NodeIsMAC = value.NodeIsMAC();
                ret.Node = value.GetNode().ToArray();
            }
            else if (ret.Version == GuidVersion.DCESecurity)
            {
                ret.ClockSequence = value.GetPartialClockSequence();
                ret.Timestamp = value.GetPartialCreateTime();
                ret.NodeIsMAC = value.NodeIsMAC();
                ret.Node = value.GetNode().ToArray();
                ret.Domain = value.GetDomain();
                ret.LocalIdentifier = (uint)value.GetLocalIdentifier();
            }

            return ret;
        }
    }
}
