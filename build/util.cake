using System;
using System.Collections.Generic;
using System.Xml;

public class Util
{


    public static string CreateStamp()
    {
        var seconds = (long)(DateTime.UtcNow - new DateTime(2017, 1, 1)).TotalSeconds;
        return seconds.ToString();
    }

    public static string[] GetPackageIds(ICakeContext context, FilePathCollection projects)
    {
        var packageIds = new List<string>();
        foreach (var project in projects)
        {
            var projectFile = context.File(project.FullPath);
            var content = System.IO.File.ReadAllText(project.FullPath);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            var packageId = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/PackageId").InnerText;
            packageIds.Add(packageId);
        }
        return packageIds.ToArray();
    }
}