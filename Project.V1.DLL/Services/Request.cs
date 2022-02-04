using Project.V1.Data;
using Project.V1.DLL.Interface;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.V1.Lib.Services
{
    public class SMPRequest : BaseActionOps<RequestViewModel>, IRequest
    {
        private readonly ApplicationDbContext _context;
        private readonly ICLogger _logger;

        public SMPRequest(ApplicationDbContext context, ICLogger logger)
            : base(context, "SA", logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> GetValidRequest(RequestViewModel item)
        {
            if (item == null)
                return false;

            try
            {
                return await Task.FromResult(!_context.Requests.Any(x => x.SiteId.ToUpper() == item.SiteId.ToUpper() && x.SpectrumId == item.SpectrumId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);

                return false;
            }
        }

        public async Task<(IEnumerable<RequestViewModel> Valid, IEnumerable<RequestViewModel> Invalid)> GetValidRequests(IEnumerable<RequestViewModel> items)
        {
            if (items == null)
                return (Enumerable.Empty<RequestViewModel>(), Enumerable.Empty<RequestViewModel>());

            List<RequestViewModel> valid = new();
            List<RequestViewModel> invalid = new();

            try
            {
                foreach (var item in items)
                {
                    if (!_context.Requests.Any(x => x.SiteId == item.SiteId && x.SpectrumId == item.SpectrumId))
                        valid.Add(item);
                    else
                        invalid.Add(item);
                }

                return await Task.Run<(IEnumerable<RequestViewModel>, IEnumerable<RequestViewModel>)>(() =>
                {
                    return (valid, invalid);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);

                return (Enumerable.Empty<RequestViewModel>(), Enumerable.Empty<RequestViewModel>());
            }
        }
    }
}
