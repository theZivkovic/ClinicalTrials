using ClinicalTrials.Tests.MockModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalTrials.Tests.Helpers
{
    public static class FormFileHelper
    {
        public static FormFile CreateMockFile(string filename, string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, "id_from_form", filename);
        }
    }
}
