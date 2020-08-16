using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public enum PriceRuleType
    {
        PERCENT_AS,     // 현재 금액을 지정한 비율로 계산
        PERCENT_PLUS,   // 현재 금액에 지정한 비율을 더함
        PERCENT_MINUS,  // 현재 금액에 지정한 비율을 뺌
        STATIC_PLUS,    // 현재 금액에 지정한 금액을 더함
        STATIC_MINUS    // 현재 금액에 지정한 금액을 뺌
    }
    public partial class PriceRule
    {
        public PriceRule()
        {
            Option = new HashSet<Option>();
            ProductPriceRule = new HashSet<ProductPriceRule>();
        }

        public int Id { get; set; }
        public PriceRuleType RuleType { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public virtual ICollection<Option> Option { get; set; }
        public virtual ICollection<ProductPriceRule> ProductPriceRule { get; set; }
    }
}
