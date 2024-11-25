namespace RunningApplicationNew.Entity.Dtos
{
    
   
        public class UpdateUserDto
        {
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public int? Age { get; set; } // Optional
            public double? Height { get; set; } // Optional
            public double? Weight { get; set; } // Optional
        }
    

}
