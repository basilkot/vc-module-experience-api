﻿using System.Collections.Generic;
using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Security
{
    public class Role : CloneableEntity
    {
        public string Name { get; set; }

        public IList<string> Permissions { get; set; } = new List<string>();
    }
}