﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Badges.Controllers;
using Badges.Core.Domain;
using Badges.Core.Repositories;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Badges.Services;

namespace Badges.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the BadgeSubmission class
    /// </summary>
    [Authorize(Roles=RoleNames.Administrator)]
    public class BadgeSubmissionController : ApplicationController
    {
        private readonly INotificationService _notificationService;

        //
        // GET: /Admin/BadgeSubmission/
        public BadgeSubmissionController(IRepositoryFactory repositoryFactory, INotificationService notificationService) : base(repositoryFactory)
        {
            _notificationService = notificationService;
        }

        public ActionResult Index()
        {
            //Get all submitted by unapproved badges
            var badgeSubmissionList =
                RepositoryFactory.BadgeSubmissionRepository.Queryable.Where(x => x.Submitted && !x.Approved)
                                 .Fetch(x => x.Badge)
                                 .Fetch(x => x.Creator);

            return View(badgeSubmissionList.ToList());
        }

        //
        // GET: /Admin/BadgeSubmission/Details/5
        public ActionResult Review(Guid id)
        {
            var badgeSubmission = RepositoryFactory.BadgeSubmissionRepository.GetNullableById(id);

            if (badgeSubmission == null) return HttpNotFound();

            var model = new BadgeSubmissionViewModel
                {
                    Badge = badgeSubmission.Badge,
                    Submission = badgeSubmission,
                    BadgeCriteria = badgeSubmission.Badge.BadgeCriterias.ToList(),
                    BadgeFulfillments = badgeSubmission.BadgeFulfillments.ToList()
                };

            return View(model);
        }

        [HttpPost]
        public ActionResult Approve(Guid id)
        {
            var badgeSubmission = RepositoryFactory.BadgeSubmissionRepository.GetNullableById(id);

            if (badgeSubmission == null) return HttpNotFound();

            badgeSubmission.Approved = true;
            badgeSubmission.AwardedOn = DateTime.UtcNow;

            _notificationService.Notify(badgeSubmission.Creator, "Congratulations, you have been awarded a new badge");
            RepositoryFactory.BadgeSubmissionRepository.EnsurePersistent(badgeSubmission);

            Message = "The badge has been awarded and some day the student will be notified....";
            return RedirectToAction("Index");
        }

        public ActionResult Deny(Guid id, string reason)
        {
            var badgeSubmission = RepositoryFactory.BadgeSubmissionRepository.GetNullableById(id);

            if (badgeSubmission == null) return HttpNotFound();

            badgeSubmission.Submitted = false;

            _notificationService.Notify(badgeSubmission.Creator, reason);
            RepositoryFactory.BadgeSubmissionRepository.EnsurePersistent(badgeSubmission);

            Message = "The badge has been sent back for changes and eventually the student will be notified....";
            return RedirectToAction("Index");
        }
    }

	/// <summary>
    /// ViewModel for the BadgeSubmission class
    /// </summary>
    public class BadgeSubmissionViewModel
	{
	    public Badge Badge { get; set; }
	    public IList<BadgeCriteria> BadgeCriteria { get; set; }
	    public IList<BadgeFulfillment> BadgeFulfillments { get; set; }
	    public BadgeSubmission Submission { get; set; }
	}
}
