using System.Text;

namespace ProjectsWebApi.Models;

public record Project
{
    ///<summary>Unique id of the project, used for the database, and as a key for iterating in React.</summary>
    public Guid Id { get; set; }
    ///<summary>The name of the project.</summary>
    public string Name { get; set; }
    ///<summary>A dictionary of short descriptions of the project, by language. {language, description}</summary>
    public Dictionary<string, string> ShortDescriptions { get; set; }
    ///<summary>A dictionary of long descriptions of the project, by language. {language, description}</summary>
    public Dictionary<string, string> LongDescriptions { get; set; }
    ///<summary>A list of what technologies the project uses.</summary>
    public string[] Technologies { get; set; }
    ///<summary>Link to the website of the project.</summary>
    public string Site { get; set; }
    ///<summary>Link to the source code for the project.</summary>
    public string Source { get; set; }
    ///<summary>Whether the project is marked favorite or not.</summary>
    public bool IsFavorite { get; set; }

    public override string ToString()
    {
        return Name;
        // Todo: rebuild this with multiple descriptions.
        // StringBuilder sb = new();
        // sb.Append($"ID: {Id.ToString()}\nNAME: {Name}\nSHORT_DESCRIPTION: {ShortDescription}\nLONG_DESCRIPTION: {LongDescription}\nTECHNOLOGIES: [");
        
        // foreach (string tech in Technologies)
        // {
        //     sb.Append(tech + ", ");
        // }
        // sb.Length -= 2;
        // sb.Append("]\n");
        
        // sb.Append($"SITE: {Site}\nSOURCE: {Source}\nIS_FAVORITE: {IsFavorite.ToString()}");
        
        // return sb.ToString();
    }
}