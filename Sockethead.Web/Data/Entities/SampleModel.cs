using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sockethead.Web.Data.Entities
{
    public enum SampleEnum
    {
        [Display(Name = "Zilch")]
        Zero, 
        
        One, 
        
        Two,
    }

    public class SampleModel
    {
        [Display(Name = "Unique ID", Order = 1)]
        public int Id { get; set; } = NextId++;

        [Display(Order = 3)]
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Yo, we need a first name here")]
        [MaxLength(20, ErrorMessage = "First name is tooooo long...")]
        public string First { get; set; }

        [Display(Name = "Last Name", Order = 2)]
        [MaxLength(20)]
        public string Last { get; set; }

        public string JobTitle { get; set; }

        //[DataType(DataType.Date)]
        public DateTime RandomDate { get; set; } = GetRandomDate();

        [Display(Order = 5)]
        public bool Flag { get; set; } = GetRandomBool();

        public SampleEnum SampleEnum { get; set; } = GetRandomEnum();

        [DataType(DataType.MultilineText)]
        public string FooBarBazBBBlah { get; set; }

        private static int NextId = 1001;

        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        [Display(AutoGenerateField = false)]
        public string SecretField { get; set; } = "I'm secret";

        private static DateTime GetRandomDate()
            => DateTime.UtcNow
                .AddDays(-Random.NextDouble() * 10000)
                .AddMinutes(Random.NextDouble() * 24 * 60);

        private static bool GetRandomBool() => Random.NextDouble() > 0.5;

        private static SampleEnum GetRandomEnum() => (SampleEnum)Random.Next((int)SampleEnum.Zero, (int)SampleEnum.Two + 2);

        public override string ToString() => $"Sample Data for {First} {Last}, {JobTitle}";
        
        [DataType(DataType.Date)]
        public DateTime RandomOtherDate { get; set; } = GetRandomDate();
        
        [RegularExpression(@"^\d+\.\d{0,2}$", ErrorMessage = "Only two decimal places allowed")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double RandomDouble { get; set; } = Random.NextDouble() * 100;
        
        [RegularExpression(@"^\d+\.\d{0,2}$", ErrorMessage = "Only two decimal places allowed")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal RandomDecimal { get; set; } = (decimal)(Random.NextDouble() * 100);
        public float Float { get; set; } = (float)0.12;
        public double? NullableDouble { get; set; }
        [DataType(DataType.EmailAddress)] 
        public string Email { get; set; } = "myemail@sockethead.com";

        [DataType(DataType.Password)] 
        public string Password { get; set; } = "password";
        
        public string City { get; set; }
        public string OtherCity { get; set; }

    }
}
