using AssemAnalyzer;
using Inventor;

public class Program
{
    public static void Main(string[] args)
    {
        string filePath = args[0];
        AssemblyAnalyzer assemblyAnalyzer = new AssemblyAnalyzer();
        
        assemblyAnalyzer.OpenAssembly(filePath);
        List<AssemblyPart> assemblyParts = [];
        foreach (ApprenticeServerDocument? part in assemblyAnalyzer.Parts)
        {
            if (part == null)
            {
                continue;
            }

            // the part is still a _ComObject at this point. The properties are not known until they are read.
            assemblyParts.Add(new AssemblyPart(
                part.DisplayName,
                part.InternalName,
                part.RevisionId,
                part.DatabaseRevisionId,
                part.ComponentDefinition.ModelGeometryVersion,
                AssemblyAnalyzer.GetPartProperties(part)));
        }

        var x = 1;
    }
}