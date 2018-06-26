using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS_FEA.Models
{
        public class DMSDBContext : DbContext
    {
        
        public string conn = "DMSDBContext";
        public DMSDBContext() : base("DMSDBContext")
        {
        }

        public virtual DbSet<OUSR> OUSRs { get; set; }
        public virtual DbSet<OCOM> OCOMs { get; set; }
        public virtual DbSet<ODEP> ODEPs { get; set; }
        public virtual DbSet<OFOT> OFOTs { get; set; }



        
    }
    
    
}