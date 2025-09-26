﻿using System;
using Volo.Abp.Application.Dtos;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Data transfer object representing a MinId token.
    /// Used for authentication and authorization in MinId operations.
    /// Inherits audit properties from AuditedEntityDto.
    /// </summary>
    public class MinIdTokenDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// Gets or sets the token value used for authentication.
        /// This is a unique string that validates access to MinId services.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the business type associated with this token.
        /// Identifies the specific business context or application that uses this token.
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// Gets or sets additional information or description about this token.
        /// Can be used to store notes or metadata about the token's purpose or usage.
        /// </summary>
        public string Remark { get; set; }
    }
}