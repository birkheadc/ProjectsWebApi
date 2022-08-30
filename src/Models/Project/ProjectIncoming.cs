namespace ProjectsWebApi.Models;

public record ProjectIncoming
{
    ///<summary>The name of the project.</summary>
    public string Name { get; set; }
    ///<summary>A short description of the project.</summary>
    public string ShortDescription { get; set; }
    ///<summary>A full description of the project.</summary>
    public string LongDescription { get; set; }
    ///<summary>A list of what technologies the project uses.</summary>
    public string[] Technologies { get; set; }
    ///<summary>Link to the website of the project.</summary>
    public string Site { get; set; }
    ///<summary>Link to the source code for the project.</summary>
    public string Source { get; set; }

    
}