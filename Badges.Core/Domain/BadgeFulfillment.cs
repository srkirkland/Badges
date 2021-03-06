﻿using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;

namespace Badges.Core.Domain
{
    /// <summary>
    /// Associations between badge criteria and a student's work/experiences.  Used to fulfill a badge's criteria
    /// </summary>
    public class BadgeFulfillment : DomainObjectGuid
    {
        [Required]
        public virtual BadgeSubmission BadgeSubmission { get; set; }

        [Required]
        public virtual BadgeCriteria BadgeCriteria { get; set; }
        
        public virtual Experience Experience { get; set; }
        public virtual SupportingWork SupportingWork { get; set; }

        [StringLength(140)]
        public virtual string Comment { get; set; }
    }

    public class BadgeFulfillmentMap : ClassMap<BadgeFulfillment>
    {
        public BadgeFulfillmentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Comment).Nullable().Length(140);

            References(x => x.BadgeSubmission).Not.Nullable();
            References(x => x.BadgeCriteria).Not.Nullable();

            References(x => x.Experience).Nullable();
            References(x => x.SupportingWork).Nullable();
        }
    }
}