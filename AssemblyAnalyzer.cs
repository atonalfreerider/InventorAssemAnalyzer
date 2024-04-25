using Inventor;

namespace AssemAnalyzer;

class AssemblyAnalyzer
{
    readonly ApprenticeServerComponent? aprServer;
    ApprenticeServerDocument? ActiveAssembly { get; set; }

    List<ApprenticeServerDocument?> parts = [];
    public IReadOnlyList<ApprenticeServerDocument?> Parts => parts.AsReadOnly();
    
    public AssemblyAnalyzer()
    {
        aprServer = new ApprenticeServerComponent();
        if (aprServer == null)
        {
            throw new Exception("Error connecting to Inventor ApprenticeServer");
        }
    }

    public void OpenAssembly(string pathToFile)
    {
        try
        {
            ActiveAssembly = aprServer?.Open(pathToFile);
            if (ActiveAssembly != null && ActiveAssembly.DocumentType != DocumentTypeEnum.kAssemblyDocumentObject)
            {
                throw new Exception("This file is not an assembly file Inventor.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Error when trying to open the file. The assembly file is corrupted or was created in a later version Inventor.");
        }

        GetAssemblyParts();
    }

    void GetAssemblyParts()
    {
        if (ActiveAssembly == null) return;

        parts = new List<ApprenticeServerDocument?>(ActiveAssembly.AllReferencedDocuments.Count);
        GetAllDefinitionParts(ActiveAssembly);
    }

    void GetAllDefinitionParts(ApprenticeServerDocument? assemblyDoc)
    {
        if (assemblyDoc?.AllReferencedDocuments == null) return;

        foreach (ApprenticeServerDocument? curDoc in assemblyDoc.AllReferencedDocuments)
        {
            if (curDoc == null)
            {
                continue;
            }

            string val = curDoc.DisplayName;
            switch (curDoc.DocumentType)
            {
                case DocumentTypeEnum.kAssemblyDocumentObject:
                    GetAllDefinitionParts(curDoc);
                    break;
                case DocumentTypeEnum.kPartDocumentObject:
                {
                    /*Is there a document in the assembly with the same links and geometry (copies of this part)*/
                    ApprenticeServerDocument? copyDoc = parts
                        .Find(x => x?.InternalName == curDoc.InternalName
                                   && x.DatabaseRevisionId == curDoc.DatabaseRevisionId
                                   && x.ComponentDefinition.ModelGeometryVersion ==
                                   curDoc.ComponentDefinition.ModelGeometryVersion
                        );

                    if (copyDoc == null) // if there is no such document, then add
                    {
                        parts.Add(curDoc);
                    }

                    break;
                }
                case DocumentTypeEnum.kUnknownDocumentObject:
                    break;
                case DocumentTypeEnum.kDrawingDocumentObject:
                    break;
                case DocumentTypeEnum.kPresentationDocumentObject:
                    break;
                case DocumentTypeEnum.kDesignElementDocumentObject:
                    break;
                case DocumentTypeEnum.kForeignModelDocumentObject:
                    break;
                case DocumentTypeEnum.kSATFileDocumentObject:
                    break;
                case DocumentTypeEnum.kNoDocument:
                    break;
                case DocumentTypeEnum.kNestingDocument:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static Dictionary<string, string?> GetPartProperties(ApprenticeServerDocument part)
    {
        Dictionary<string, string?> dict = new Dictionary<string, string?>();
        int[] designPropsIds = GetDesignTrackingProperties();
        Property tempProp;
        foreach (int propId in Enum.GetValues(typeof(PropertiesForDocSummaryInformationEnum)))
        {
            try
            {
                //https://modthemachine.typepad.com/my_weblog/2010/02/accessing-iproperties.html
                tempProp = part.PropertySets["Inventor Document Summary Information"].ItemByPropId[propId];
                if (tempProp.Value != null && tempProp.Value.ToString() != "")
                {
                    dict.Add(tempProp.DisplayName, tempProp.Value.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        foreach (int t in designPropsIds)
        {
            try
            {
                tempProp = part.PropertySets["Design Tracking Properties"].ItemByPropId[t];

                if (tempProp.Value != null && tempProp.Value.ToString() != "")
                {
                    dict.Add(tempProp.DisplayName, tempProp.Value.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return dict;
    }

    static int[] GetDesignTrackingProperties()
    {
        int[] propIds =
        [
            4, 5, 20, 29, 32, 37, 36, 48, 58, 59, 60, 61, 67, 72 // basic required properties
        ];
        return propIds;
    }
}