//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VanilaBakery.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ADMIN
    {
        public int UserAdmin { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public Nullable<byte> IsAdmin { get; set; }
        public Nullable<byte> Allowed { get; set; }
    }
}