﻿using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace SharpAbp.MinId
{
    /// <summary>
    /// Represents an authentication token for accessing MinId services.
    /// This aggregate root manages access tokens that are associated with specific business types
    /// and provides authorization for ID generation operations.
    /// </summary>
    public class MinIdToken : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Gets or sets the authentication token string.
        /// This token is used for authorizing access to MinId generation services.
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Gets or sets the business type identifier that this token authorizes access to.
        /// This ensures tokens are scoped to specific business contexts.
        /// </summary>
        public virtual string BizType { get; set; }

        /// <summary>
        /// Gets or sets an optional remark or description for this token.
        /// This can be used for administrative purposes to describe the token's purpose.
        /// </summary>
        public virtual string Remark { get; set; }

        /// <summary>
        /// Initializes a new instance of the MinIdToken class.
        /// </summary>
        public MinIdToken()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MinIdToken class with specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier for this token.</param>
        /// <param name="bizType">The business type identifier this token authorizes.</param>
        /// <param name="token">The authentication token string.</param>
        /// <param name="remark">An optional remark or description for this token.</param>
        public MinIdToken(Guid id, string bizType, string token, string remark)
        {
            Id = id;
            BizType = bizType;
            Token = token;
            Remark = remark;
        }

        /// <summary>
        /// Updates the token configuration with new values.
        /// </summary>
        /// <param name="bizType">The business type identifier this token authorizes.</param>
        /// <param name="token">The authentication token string.</param>
        /// <param name="remark">An optional remark or description for this token.</param>
        public void Update(string bizType, string token, string remark)
        {
            BizType = bizType;
            Token = token;
            Remark = remark;
        }
    }
}
