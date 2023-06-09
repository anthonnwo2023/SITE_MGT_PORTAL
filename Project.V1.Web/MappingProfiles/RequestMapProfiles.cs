﻿using AutoMapper;

namespace Project.V1.Web.MappingProfiles;

public class RequestMapProfiles : Profile
{
    public RequestMapProfiles()
    {
        CreateMap<RequestViewModelDTO, RequestViewModelDTO>();
        CreateProjection<RequestViewModel, RequestViewModelDTO>()
            .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.Requester.Name))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Requester.Vendor.Name))
            .ForMember(dest => dest.EngineerComment, opt => opt.MapFrom(src => src.EngineerAssigned.ApproverComment))
            .ForMember(dest => dest.SummerConfig, opt => opt.MapFrom(src => src.SummerConfig.Name))
            .ForMember(dest => dest.IntegratedDate, opt => opt.MapFrom(src => src.IntegratedDate.Date))
            .ForMember(dest => dest.DateSubmitted, opt => opt.MapFrom(src => src.DateSubmitted.Date))
            .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.ProjectType.Name))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.ProjectName.Name))
            .ForMember(dest => dest.TechType, opt => opt.MapFrom(src => src.TechType.Name))
            .ForMember(dest => dest.AntennaType, opt => opt.MapFrom(src => src.AntennaType.Name))
            .ForMember(dest => dest.AntennaMake, opt => opt.MapFrom(src => src.AntennaMake.Name))
            .ForMember(dest => dest.Spectrum, opt => opt.MapFrom(src => src.Spectrum.Name))
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region.Name))
            .ForMember(dest => dest.EngineerAssignedIsApproved, opt => opt.MapFrom(src => src.EngineerAssigned.IsApproved))
            .ForMember(dest => dest.EngineerAssignedDateApproved, opt => opt.MapFrom(src => src.EngineerAssigned.DateApproved.Date))
            .ForMember(dest => dest.EngineerAssignedDateActioned, opt => opt.MapFrom(src => src.EngineerAssigned.DateActioned.Date))
            .ForMember(dest => dest.EngineerAssigned, opt => opt.MapFrom(src => src.EngineerAssigned.Fullname));

        CreateProjection<SiteHUDRequestModel, SiteHUDRequestModelDTO>()
            .ForMember(dest => dest.TechTypes, opt => opt.MapFrom(src => string.Join(",", src.TechTypes.Select(x => x.Name))))
            .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.Requester.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.FirstApproverName, opt => opt.MapFrom(src => src.FirstApprover.Fullname))
            .ForMember(dest => dest.SecondApproverName, opt => opt.MapFrom(src => src.SecondApprover.Fullname))
            .ForMember(dest => dest.ThirdApproverName, opt => opt.MapFrom(src => src.ThirdApprover.Fullname))
            .ForMember(dest => dest.FirstApproverComment, opt => opt.MapFrom(src => src.FirstApprover.ApproverComment))
            .ForMember(dest => dest.SecondApproverComment, opt => opt.MapFrom(src => src.SecondApprover.ApproverComment))
            .ForMember(dest => dest.ThirdApproverComment, opt => opt.MapFrom(src => src.ThirdApprover.ApproverComment))
            ;
    }
}
