// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using SharpAbp.Abp.Identity;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

// ReSharper disable once CheckNamespace
namespace SharpAbp.Abp.Identity;

public class IdentitySecurityLogPagedRequestDto : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }

    public string ApplicationName { get; set; }

    public string Identity { get; set; }

    public string Action { get; set; }

    public Guid? UserId { get; set; }

    public string UserName { get; set; }

    public string ClientId { get; set; }

    public string CorrelationId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
