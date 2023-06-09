﻿namespace Project.V1.Web.MappingProfiles;

public class ODataClassHelper : ODataV4Adaptor
{
    public ODataClassHelper(DataManager dm) : base(dm)
    {

    }

    public override void BeforeSend(HttpRequestMessage request)
    {
        base.BeforeSend(request);   
    }

    public override string OnPredicate(WhereFilter filter, DataManagerRequest query, bool requiresCast = false)
    {
        return base.OnPredicate(filter, query, requiresCast);
    }

    public override object ProcessQuery(DataManagerRequest queries)
    {
        if (queries.Where != null && queries.Where[0].predicates != null)
        {
            if (queries.Where[0].predicates[0].value is DateTime)
                queries.Where[0].predicates[0].value = Convert.ToDateTime(queries.Where[0].predicates[0].value).AddHours(1).ToLocalTime();
        }

        var ActualReturnValue = base.ProcessQuery(queries);

        return ActualReturnValue;
    }
}
