using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssemAnalyzer;

public class AssemblyPart(
    Dictionary<string, string?> properties,
    string internalName,
    string revisionId,
    string databaseRevisionId,
    string modelGeometryVersion,
    string name)
    : INotifyPropertyChanged
{
    public AssemblyPart(
        string name, 
        string internalName,
        string revisionId, 
        string databaseRevisionId,
        string modelGeometryVersion, 
        Dictionary<string, string?> properties) : 
        this(properties, internalName, revisionId, databaseRevisionId, modelGeometryVersion, name)
    {
        IsSaved = false;
        Properties = properties;
    }

    public string InternalName = internalName;
    public string RevisionId = revisionId;
    public string DatabaseRevisionId = databaseRevisionId;
    public string ModelGeometryVersion = modelGeometryVersion;

    public string Name
    {
        get => name;
        set
        {
            name = value;
            OnPropertyChanged("Name");
        }
    }

    bool isSaved;

    public bool IsSaved
    {
        get => isSaved;
        set
        {
            isSaved = value;
            OnPropertyChanged("IsSaved");
        }
    }

    public Dictionary<string, string?> Properties
    {
        get => properties;
        set
        {
            properties = value;
            OnPropertyChanged("Properties");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}