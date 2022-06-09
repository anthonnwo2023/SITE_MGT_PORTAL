namespace Project.V1.Web.Requests;

public static class Helpers
{
    public static bool ShouldRequireSSV(List<ProjectTypeModel> ProjectTypes, string spectrum, string tech, string projectTypeid)
    {
        if (string.IsNullOrWhiteSpace(spectrum)) return true;
        if (string.IsNullOrWhiteSpace(tech)) return true;
        if (string.IsNullOrWhiteSpace(projectTypeid)) return true;

        var projectTypeName = ProjectTypes.FirstOrDefault(x => x.Id == projectTypeid)?.Name;

        if (spectrum.Contains("RRU"))
            return false;

        if (tech == "3G" && spectrum?.ToUpper() == "U900" && projectTypeName?.ToUpper() == "UPGRADE")
            return false;

        if (tech == "4G" && spectrum?.ToUpper() == "L800" && projectTypeName?.ToUpper() == "RT DONOR")
            return false;

        return true;
    }
}
