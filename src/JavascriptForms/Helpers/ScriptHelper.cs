using System;
using System.IO;
using System.Reflection;

namespace JavascriptForms.Helpers
{
    public class ScriptHelper
    {
        public static string LoadJavascript(Assembly assembly, string resourceName)
        {
            if (assembly == null)
                throw new NullReferenceException("Unable to load assembly");

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new NullReferenceException($"Manifest resource stream was null for resource: {resourceName}");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    return result;
                }
            }
        }
    }
}
