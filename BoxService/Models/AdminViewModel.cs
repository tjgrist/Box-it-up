using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoxService.Models
{
    public class GroupedAdminViewModel
    {
        public List<AdminViewModel> Users { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class AdminViewModel
    {
        public FoodBox InvoiceBox { get; set; }
        public string UserName { get; set; }
        public string BoxName { get; set; }
    }
   
}