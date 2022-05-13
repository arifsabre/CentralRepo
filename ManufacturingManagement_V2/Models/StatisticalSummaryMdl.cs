using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public class StatisticalSummaryMdl
    {
        [Display(Name = "CompCode")]
        public int CompCode { get; set; }

        [Display(Name = "CompName")]
        public string CompName { get; set; }

        [Display(Name = "FinYear")]
        public string FinYear { get; set; }

        [Display(Name = "QuarterNo")]
        public string QuarterNo { get; set; }

        [Display(Name = "MonthNo")]
        public string MonthNo { get; set; }

        [Display(Name = "WeekNo")]
        public string WeekNo { get; set; }

        [Display(Name = "AsOnDate")]
        public string AsOnDate { get; set; }

        public List<StatisticalSummaryDetailMdl> SummaryList { get; set; }

    }

    public class StatisticalSummaryDetailMdl
    {
        [System.Web.Mvc.AllowHtml]
        [Display(Name = "AlertDetail")]
        public string AlertDetail { get; set; }//d

        [Display(Name = "RecId")]
        public int RecId { get; set; }

        [Display(Name = "Section")]
        public string SCCode { get; set; }

        [Display(Name = "Section")]
        public string SCName { get; set; }

        [Display(Name = "Segment")]
        public string SegmentCode { get; set; }

        [Display(Name = "Segment")]
        public string SegmentName { get; set; }

        [Display(Name = "Week")]
        public double SSWeek { get; set; }

        [Display(Name = "Month")]
        public double SSMonth { get; set; }

        [Display(Name = "Quarter")]
        public double SSQuarter { get; set; }

        [Display(Name = "FinYear")]
        public double SSFinYear { get; set; }

        [Display(Name = "Unit")]
        public string Unit { get; set; }

    }
}